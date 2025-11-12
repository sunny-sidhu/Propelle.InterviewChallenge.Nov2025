using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Propelle.InterviewChallenge.Application;
using Propelle.InterviewChallenge.Application.Domain;
using Propelle.InterviewChallenge.Application.Domain.Events;

namespace Propelle.InterviewChallenge.Endpoints
{
    public static class MakeDeposit
    {
        public class Request
        {
            public Guid UserId { get; set; }

            public decimal Amount { get; set; }
            
            public Guid IdempotencyKey { get; set; }
        }

        public class Response
        {
            public Guid DepositId { get; set; }
        }

        public class Endpoint : Endpoint<Request, Response>
        {
            private readonly PaymentsContext _paymentsContext;
            private readonly Application.EventBus.IEventBus _eventBus;

            public Endpoint(
                PaymentsContext paymentsContext,
                Application.EventBus.IEventBus eventBus)
            {
                _paymentsContext = paymentsContext;
                _eventBus = eventBus;
            }

            public override void Configure()
            {
                Post("/api/deposits/{UserId}");
            }

            public override async Task HandleAsync(Request req, CancellationToken ct)
            {
                var deposit = new Deposit(req.UserId, req.Amount, req.IdempotencyKey);
                
                /*
                 * 2. Here I see that a new deposit is instantiated with the UserId and Amount from the request.
                 * 
                 * I've extended the request and deposit entity to include an IdempotencyKey property which is used to ensure that the same deposit is not stored multiple times.
                 */
                
                var existingDeposit = await _paymentsContext.Deposits
                    .FirstOrDefaultAsync(d => d.IdempotencyKey == deposit.IdempotencyKey, cancellationToken: ct);

                if (existingDeposit is null)
                {
                    _paymentsContext.Deposits.Add(deposit);
                    
                    await _paymentsContext.SaveChangesAsync(ct);

                    await _eventBus.Publish(new DepositMade
                    {
                        Id = deposit.Id
                    });
                }
                else
                {
                    deposit = existingDeposit;
                }

                await SendAsync(new Response { DepositId = deposit.Id }, 201, ct);
            }
        }
    }
}
