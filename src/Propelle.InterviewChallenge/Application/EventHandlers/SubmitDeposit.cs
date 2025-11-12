using Propelle.InterviewChallenge.Application.Domain;
using Propelle.InterviewChallenge.Application.Domain.Events;
using Propelle.InterviewChallenge.EventHandling;

namespace Propelle.InterviewChallenge.Application.EventHandlers
{
    public class SubmitDeposit : IEventHandler<DepositMade>
    {
        private readonly PaymentsContext _context;
        private readonly ISmartInvestClient _smartInvestClient;

        public SubmitDeposit(PaymentsContext context, ISmartInvestClient smartInvestClient)
        {
            _context = context;
            _smartInvestClient = smartInvestClient;
        }

        public async Task Handle(DepositMade @event)
        {
            var deposit = await _context.Deposits.FindAsync(@event.Id);
            
            /*
             * 4. Looks like is where the SmartInvest API is called.
             *
             * I've implemented a simple retry mechanism to ensure that the deposit is submitted successfully.
             * In a Production environment I would probably define a more robust retry policy using a tool like Polly.
             * 
             * If after the retry limit is reached the deposit is still not submitted successfully,
             *  it should be unstored from the database and an error should be returned to the client.
             */
            
            var retryCount = 0;
            
            while (retryCount++ < 5)
            {
                try
                {
                    await _smartInvestClient.SubmitDeposit(deposit.UserId, deposit.Amount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Retrying due to error: {ex}");
                    
                    await Task.Delay(1000);
                    
                    continue;
                }

                break;
            }
        }
    }
}
