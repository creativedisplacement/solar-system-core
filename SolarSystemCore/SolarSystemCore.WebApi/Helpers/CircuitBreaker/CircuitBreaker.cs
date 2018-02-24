using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SolarSystemCore.WebApi.Helpers.CircuitBreaker
{
    public class CircuitBreaker
    {
        private static readonly ConcurrentDictionary<string, object> CircuitBreakerLocks = new ConcurrentDictionary<string, object>();
        private static readonly ConcurrentDictionary<string, int> CircuitBreakerExceptionCounts = new ConcurrentDictionary<string, int>();

        public string CircuitId { get; set; }

        public object CircuitBreakLock => CircuitBreakerLocks.GetOrAdd(CircuitId, new object());

        private CircuitBreakerState _state;
        private Exception _lastExecutionException = null;

        public int Failures
        {
            get => CircuitBreakerExceptionCounts.GetOrAdd(CircuitId, 0);
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
            var circuitBreakerLock = CircuitBreakLock;

            T response;
            this._lastExecutionException = null;

            lock (circuitBreakerLock)
            {
                _state.ExecutionStart();
                if (_state is OpenState)
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
                    _lastExecutionException = e;
                    _state.ExecutionFail(e);
                }
                //TODO:implement logging Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                throw;
            }
            finally
            {
                lock (circuitBreakerLock)
                {
                    _state.ExecutionComplete();
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
            T response;
            this._lastExecutionException = null;

            _state.ExecutionStart();
            if (_state is OpenState)
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
                _lastExecutionException = e;
                _state.ExecutionFail(e);
                //TODO:implement logging Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                throw;
            }
            finally
            {
                _state.ExecutionComplete();
            }

            return response;
        }

        /// <summary>
        /// Executes a specified Func within the confines of the Circuit Breaker Pattern (https://msdn.microsoft.com/en-us/library/dn589784.aspx)
        /// </summary>
        /// <param name="funcToIvoke"></param>
        public async Task ExecuteAsync(Func<Task> funcToInvoke)
        {
            this._lastExecutionException = null;

            _state.ExecutionStart();
            if (_state is OpenState)
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
                _lastExecutionException = e;
                _state.ExecutionFail(e);
                //TODO:implement logging Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                throw;
            }
            finally
            {
                _state.ExecutionComplete();
            }
        }

        #region State Management
        public bool IsClosed => _state.Update() is ClosedState;

        public bool IsOpen => _state.Update() is OpenState;

        public bool IsHalfOpen => _state.Update() is HalfOpenState;

        public bool IsThresholdReached()
        {
            return Failures >= Threshold;
        }

        public Exception GetLastExecutionException()
        {
            return _lastExecutionException;
        }

        internal CircuitBreakerState MoveToClosedState()
        {
            _state = new ClosedState(this);
            return _state;
        }

        internal CircuitBreakerState MoveToOpenState()
        {
            _state = new OpenState(this);
            return _state;
        }

        internal CircuitBreakerState MoveToHalfOpenState()
        {
            _state = new HalfOpenState(this);
            return _state;
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
