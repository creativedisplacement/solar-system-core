using System;

namespace SolarSystemCore.WebApi.Helpers.CircuitBreaker
{
    public class HalfOpenState : CircuitBreakerState
    {
        public HalfOpenState(CircuitBreaker circuitBreaker) : base(circuitBreaker) { }

        public override void ExecutionFail(Exception e)
        {
            //FAIL, set back to "open"
            base.ExecutionFail(e);
            circuitBreaker.MoveToOpenState();
        }

        public override void ExecutionComplete()
        {
            //SUCCESS, set to "closed"
            base.ExecutionComplete();

            //Decrease count by one
            //if count = 0 then everything is hunky dory and set the circuit breaker to closed state
            if (circuitBreaker.Failures > 0)
            {
                circuitBreaker.DecreaseFailureCount();
            }
            else
            {
                circuitBreaker.MoveToClosedState();
            }
        }
    }
}
