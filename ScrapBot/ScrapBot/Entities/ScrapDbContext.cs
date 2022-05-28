using Microsoft.EntityFrameworkCore;

namespace ScrapBot.Entities
{
    public class ScrapDbContext : DbContext
    {
        public DbSet<Notifier> Notifiers { get; set; }

        public ScrapDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public ScrapDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Notifier>(b =>
               {
                   b.Property(x => x.Id);
                   b.HasKey(x => x.Id);

                   b.Property(x => x.DiscordId);
                   b.HasIndex(x => x.DiscordId);

                   b.Property(x => x.NextTrigger);
                   b.Property(x => x.NextTrigger);

                   b.Property(x => x.Interval);

                   b.Property(x => x.TriggerCount);

                   b.Property(x => x.CompactDisplay);

                   b.Property(x => x.Type);

                   b.ToTable("Notifiers");
               });
    }
}
