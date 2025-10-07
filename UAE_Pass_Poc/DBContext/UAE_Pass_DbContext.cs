using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UAE_Pass_Poc.Entities;

namespace UAE_Pass_Poc.DBContext;

public class UaePassDbContext : DbContext
{
    public UaePassDbContext(DbContextOptions<UaePassDbContext> options) : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UaePassDbContext).Assembly);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("Users");

            entity.Property(e => e.FullName).HasMaxLength(25);
            entity.Property(e => e.Mobile).HasMaxLength(25);
        });
    }

    public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
    {
        if (entity is Entity e)
        {
            e.CreatedAt = DateTime.UtcNow;
            e.UpdatedAt = DateTime.UtcNow;
        }
        return base.Add(entity);
    }

    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
    {
        if (entity is Entity e)
        {
            e.UpdatedAt = DateTime.UtcNow;
        }
        return base.Update(entity);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        try
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        catch
        {
            ChangeTracker.Clear();
            throw;
        }
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch
        {
            ChangeTracker.Clear();
            throw;
        }
    }

    public bool IsUnchanged<TEntity>(TEntity entity)
    {
        return Entry(entity!).State == EntityState.Unchanged;
    }

    public void ClearChangeTracker()
    {
        ChangeTracker.Clear();
    }
}