using System;

namespace SolarSystemCore.WebApi.Helpers.CircuitBreaker
{
    public class OpenState : CircuitBreakerState
    {
        private readonly DateTime _openDateTime; //last time something went wrong, or breaker was initialized
        public OpenState(CircuitBreaker circuitBreaker)
            : base(circuitBreaker)
        {
            //initialize openDateTime
            _openDateTime = DateTime.UtcNow;
        }

        public override CircuitBreaker ExecutionStart()
        {
            //kickoff execution
            base.ExecutionStart();
            this.Update();
            return base.CircuitBreaker;
        }

        public override CircuitBreakerState Update()
        {
            base.Update();

            return DateTime.UtcNow >= _openDateTime + base.CircuitBreaker.OpenTimeout ? CircuitBreaker.MoveToHalfOpenState() : this;
        }
    }
}
