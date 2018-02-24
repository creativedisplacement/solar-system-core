using System;

namespace SolarSystemCore.WebApi.Helpers.CircuitBreaker
{
    public abstract class CircuitBreakerState
    {
        protected readonly CircuitBreaker CircuitBreaker;

        protected CircuitBreakerState(CircuitBreaker circuitBreaker)
        {
            this.CircuitBreaker = circuitBreaker;
        }

        public virtual CircuitBreaker ExecutionStart()
        {
            return this.CircuitBreaker;
        }
        public virtual void ExecutionComplete() { }
        public virtual void ExecutionFail(Exception e)
        {
            CircuitBreaker.IncreaseFailureCount();
        }

        public virtual CircuitBreakerState Update()
        {
            return this;
        }
    }
}
