using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Propelle.InterviewChallenge.Application.Domain;

namespace Propelle.InterviewChallenge.Application
{
    public class PaymentsContext : DbContext
    {
        public DbSet<Deposit> Deposits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var depositConfig = modelBuilder.Entity<Deposit>();
            depositConfig.HasKey(x => x.Id);
            depositConfig.Property(x => x.UserId).IsRequired();
            depositConfig.Property(x => x.Amount).IsRequired();
            depositConfig.Property(x => x.IdempotencyKey).IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Payments");
            optionsBuilder.AddInterceptors(new FlakyDatabaseTransactionSimulator());
        }

        public class FlakyDatabaseTransactionSimulator : SaveChangesInterceptor
        {
            public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
            {
                PointOfFailure.SimulatePotentialFailure();

                return base.SavingChanges(eventData, result);
            }

            public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
            {
                PointOfFailure.SimulatePotentialFailure();

                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }
        }
    }
}
