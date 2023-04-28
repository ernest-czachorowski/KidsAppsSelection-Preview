namespace Database;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(app => app.Apps)
            .WithOne(usr => usr.AddedBy);

        modelBuilder.Entity<App>()
            .HasOne(usr => usr.AddedBy)
            .WithMany(app => app.Apps)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<App>()
            .Property(e => e.Images)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries),
                new ValueComparer<string[]>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToArray())
                );
    }

    public DbSet<App> Apps { get; set; }
    public DbSet<User> Users { get; set; }
}