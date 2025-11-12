using Propelle.InterviewChallenge.EventHandling;

namespace Propelle.InterviewChallenge.Application.EventBus
{
    public class SimpleEventBus : IEventBus
    {
        private readonly InMemoryEventExchange _exchange;

        public SimpleEventBus(InMemoryEventExchange exchange)
        {
            _exchange = exchange;
        }

        public async Task Publish<TEvent>(TEvent @event)
            where TEvent : class
        {
            /* If you've found this, you're eagled-eyed! Let us know in your notes if you see this, and have a think about the ramifications of 
             * changing SimulatePotentialFailure() to have a higher than zero chance of throwing an exception (i.e. simulating a real event-bus being unavailable at times) */
            PointOfFailure.SimulatePotentialFailure(0);
            
            /*
             *  5. Found this. The ramifications of this are a deposit will be stored in the database but not submitted to SmartInvest.
             */

            await _exchange.Publish(@event);
        }
    }
}
