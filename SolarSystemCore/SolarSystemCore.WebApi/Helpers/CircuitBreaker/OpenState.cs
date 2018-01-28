using System;

namespace SolarSystemCore.WebApi.Helpers.CircuitBreaker
{
    public class OpenState : CircuitBreakerState
    {
        private readonly DateTime openDateTime; //last time something went wrong, or breaker was initialized
        public OpenState(CircuitBreaker circuitBreaker)
            : base(circuitBreaker)
        {
            //initialize openDateTime
            openDateTime = DateTime.UtcNow;
        }

        public override CircuitBreaker ExecutionStart()
        {
            //kickoff execution
            base.ExecutionStart();
            this.Update();
            return base.circuitBreaker;
        }

        public override CircuitBreakerState Update()
        {
            base.Update();

            if (DateTime.UtcNow >= openDateTime + base.circuitBreaker.OpenTimeout)
            {
                //timeout has passed, progress state to "half-open"
                return circuitBreaker.MoveToHalfOpenState();
            }

            return this;
        }
    }
}
