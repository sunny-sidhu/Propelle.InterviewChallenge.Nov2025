namespace Propelle.InterviewChallenge.Application.Domain
{
    public class Deposit
    {
        public Guid Id { get; }

        public Guid UserId { get; }

        public decimal Amount { get; }
        
        public Guid IdempotencyKey { get; }

        public Deposit(Guid userId, decimal amount, Guid idempotencyKey) : this(Guid.NewGuid(), userId, amount, idempotencyKey) { }

        public Deposit(Guid id, Guid userId, decimal amount, Guid idempotencyKey)
        {
            Id = id;
            UserId = userId;
            Amount = amount;
            IdempotencyKey = idempotencyKey;
        }
    }
}
