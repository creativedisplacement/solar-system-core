using System;

namespace SolarSystemCore.WebApi.Helpers.CircuitBreaker
{
    public abstract class CircuitBreakerState
    {
        protected readonly CircuitBreaker circuitBreaker;

        protected CircuitBreakerState(CircuitBreaker circuitBreaker)
        {
            this.circuitBreaker = circuitBreaker;
        }

        public virtual CircuitBreaker ExecutionStart()
        {
            return this.circuitBreaker;
        }
        public virtual void ExecutionComplete() { }
        public virtual void ExecutionFail(Exception e)
        {
            circuitBreaker.IncreaseFailureCount();
        }

        public virtual CircuitBreakerState Update()
        {
            return this;
        }
    }
}
