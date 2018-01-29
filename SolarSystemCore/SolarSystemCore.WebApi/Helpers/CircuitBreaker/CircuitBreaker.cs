using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SolarSystemCore.WebApi.Helpers.CircuitBreaker
{
    public class CircuitBreaker
    {
        private static ConcurrentDictionary<string, object> CircuitBreakerLocks = new ConcurrentDictionary<string, object>();
        private static ConcurrentDictionary<string, int> CircuitBreakerExceptionCounts = new ConcurrentDictionary<string, int>();

        public string CircuitId { get; set; }

        public object CircuitBreakLock
        {
            get
            {
                return CircuitBreakerLocks.GetOrAdd(CircuitId, new object());
            }
        }

        private CircuitBreakerState state;
        private Exception lastExecutionException = null;

        public int Failures
        {
            get
            {
                return CircuitBreakerExceptionCounts.GetOrAdd(CircuitId, 0);
            }
            set
            {
                CircuitBreakerExceptionCounts.AddOrUpdate(CircuitId, value, (key, oldValue) => value);
            }
        }
        public int Threshold { get; private set; }
        public TimeSpan OpenTimeout { get; private set; }

        public CircuitBreaker(
            string circuitId,
            int failureThreshold = 3,
            TimeSpan? openTimeout = null
            )
        {
            if (openTimeout == null)
                openTimeout = TimeSpan.FromSeconds(5);

            CircuitId = circuitId;

            Threshold = failureThreshold;
            OpenTimeout = (TimeSpan)openTimeout;

            //Initialize
            MoveToClosedState();
        }


        /// <summary>
        /// Executes a specified Func<T> within the confines of the Circuit Breaker Pattern (https://msdn.microsoft.com/en-us/library/dn589784.aspx)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcToIvoke"></param>
        /// <returns>Object of type T of default(T)</returns>
        public T Execute<T>(Func<T> funcToInvoke)
        {
            object circuitBreakerLock = CircuitBreakLock;

            T response = default(T);
            this.lastExecutionException = null;

            lock (circuitBreakerLock)
            {
                state.ExecutionStart();
                if (state is OpenState)
                {
                    throw new Exception(System.Net.HttpStatusCode.ServiceUnavailable.ToString());
                }
            }

            try
            {
                lock (circuitBreakerLock)
                {
                    //do the work
                    response = funcToInvoke();
                }
            }
            catch (Exception e)
            {
                lock (circuitBreakerLock)
                {
                    lastExecutionException = e;
                    state.ExecutionFail(e);
                }
                //TODO:implement logging Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                throw;
            }
            finally
            {
                lock (circuitBreakerLock)
                {
                    state.ExecutionComplete();
                }
            }

            return response;
        }

        /// <summary>
        /// Executes a specified Func<T> within the confines of the Circuit Breaker Pattern (https://msdn.microsoft.com/en-us/library/dn589784.aspx)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="funcToIvoke"></param>
        /// <returns>Object of type T of default(T)</returns>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> funcToInvoke)
        {
            T response = default(T);
            this.lastExecutionException = null;

            state.ExecutionStart();
            if (state is OpenState)
            {
                //Stop execution of this method
                throw new Exception(System.Net.HttpStatusCode.ServiceUnavailable.ToString()); 
            }

            try
            {
                //do the work
                response = await funcToInvoke();
                //Access Without Cache
            }
            catch (Exception e)
            {
                lastExecutionException = e;
                state.ExecutionFail(e);
                //TODO:implement logging Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                throw;
            }
            finally
            {
                state.ExecutionComplete();
            }

            return response;
        }

        /// <summary>
        /// Executes a specified Func within the confines of the Circuit Breaker Pattern (https://msdn.microsoft.com/en-us/library/dn589784.aspx)
        /// </summary>
        /// <param name="funcToIvoke"></param>
        public async Task ExecuteAsync(Func<Task> funcToInvoke)
        {
            this.lastExecutionException = null;

            state.ExecutionStart();
            if (state is OpenState)
            {
                //Stop execution of this method
                throw new Exception(System.Net.HttpStatusCode.ServiceUnavailable.ToString());
            }

            try
            {
                //do the work
                await funcToInvoke();
                //Access Without Cache
            }
            catch (Exception e)
            {
                lastExecutionException = e;
                state.ExecutionFail(e);
                //TODO:implement logging Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                throw;
            }
            finally
            {
                state.ExecutionComplete();
            }
        }

        #region State Management
        public bool IsClosed
        {
            get { return state.Update() is ClosedState; }
        }

        public bool IsOpen
        {
            get { return state.Update() is OpenState; }
        }

        public bool IsHalfOpen
        {
            get { return state.Update() is HalfOpenState; }
        }

        public bool IsThresholdReached()
        {
            return Failures >= Threshold;
        }

        public Exception GetLastExecutionException()
        {
            return lastExecutionException;
        }

        void Close()
        {
            MoveToClosedState();
        }

        void Open()
        {
            MoveToOpenState();
        }

        internal CircuitBreakerState MoveToClosedState()
        {
            state = new ClosedState(this);
            return state;
        }

        internal CircuitBreakerState MoveToOpenState()
        {
            state = new OpenState(this);
            return state;
        }

        internal CircuitBreakerState MoveToHalfOpenState()
        {
            state = new HalfOpenState(this);
            return state;
        }

        internal void IncreaseFailureCount()
        {
            Failures++;
        }

        internal void DecreaseFailureCount()
        {
            Failures--;
        }

        internal void ResetFailureCount()
        {
            Failures = 0;
        }
        #endregion
    }
}
