using System;

namespace SolarSystemCore.WebApi.Helpers.CircuitBreaker
{
    public class ClosedState : CircuitBreakerState
    {
        public ClosedState(CircuitBreaker circuitBreaker)
            : base(circuitBreaker)
        {
            //Decrement fail count as soon as we have a success
            circuitBreaker.ResetFailureCount();
        }

        public override void ExecutionFail(Exception e)
        {
            base.ExecutionFail(e);
            if (circuitBreaker.IsThresholdReached())
            {
                //if we've reached the specified fail threshold, set to "open state"
                circuitBreaker.MoveToOpenState();
            }
        }
    }
}
