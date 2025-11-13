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
    public virtual DbSet<DocInstance> DocInstances { get; set; } = null!;
    public virtual DbSet<Document> Documents { get; set; } = null!;
    public virtual DbSet<ReceivePresentation> ReceivePresentations { get; set; } = null!;
    public virtual DbSet<ReceivePresentationResponse> ReceivePresentationResponses { get; set; } = null!;
    public virtual DbSet<RequestPresentation> RequestPresentations { get; set; } = null!;
    public virtual DbSet<RequestPresentationResponseMapping> RequestPresentationResponseMappings { get; set; } = null!;
    public virtual DbSet<ReceiveVisualization> ReceiveVisualizations { get; set; } = null!;
    public virtual DbSet<ReceiveVisualizationResponse> ReceiveVisualizationResponse { get; set; } = null!;
    public virtual DbSet<VisualizationFile> VisualizationFile { get; set; }
    public virtual DbSet<RejectNotification> RejectNotifications { get; set; }

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
            entity.Property(e => e.Email).HasMaxLength(25);
        });

        modelBuilder.Entity<DocInstance>(entity =>
        {
            entity.ToTable("DocumentInstance");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Value).HasMaxLength(255);

            entity.HasOne(di => di.Document)
                    .WithMany(d => d.DocumentInstances)
                    .HasForeignKey(di => di.DocumentId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Document");
            entity.HasKey(e => e.Id);


            entity.Property(e => e.CustomDocumentTypeEN).HasMaxLength(35);
            entity.Property(e => e.CustomDocumentTypeAR).HasMaxLength(35);

            entity.Property(e => e.Required).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.RequiredAttested).IsRequired(false);
            entity.Property(e => e.AllowExpired).IsRequired(false);
            entity.Property(e => e.SelfSignedAccepted).IsRequired(false);

            entity.Property(e => e.DocumentType)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(e => e.Emirate)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.HasOne(d => d.RequestPresentation)
                .WithMany(rp => rp.RequestedDocuments)
                .HasForeignKey(d => d.RequestPresentationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(d => d.RequestPresentationId);
        });

        modelBuilder.Entity<RequestPresentation>(entity =>
        {
            entity.ToTable("RequestPresentation");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PurposeEN).HasMaxLength(255);
            entity.Property(e => e.PurposeAR).HasMaxLength(255);
            entity.Property(e => e.Request).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.ExpiryDate).IsRequired();
            entity.Property(e => e.Origin)
                .HasConversion<string>()
                .IsRequired();
            entity.Property(e => e.RequestedVerifiedAttributes).HasMaxLength(1000);

            entity.HasMany(rp => rp.RequestedDocuments)
                .WithOne(d => d.RequestPresentation)
                .HasForeignKey(d => d.RequestPresentationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RequestPresentationResponseMapping>(entity =>
        {
            entity.ToTable("RequestPresentationResponseMapping");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.RequestId).IsRequired();
            entity.Property(e => e.ProofOfPresentationId).IsRequired();
            entity.Property(e => e.RequestPresentationId).IsRequired();
        });

        modelBuilder.Entity<ReceivePresentation>(entity =>
        {
            entity.ToTable("ReceivePresentation");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ProofOfPresentationId).IsRequired();
            entity.Property(e => e.ProofOfPresentationRequestId).IsRequired(false);
            entity.Property(e => e.QrId).IsRequired(false);
            entity.Property(e => e.SignedPresentation).IsRequired(false);
            entity.Property(e => e.CitizenSignature).IsRequired(false);
            entity.Property(e => e.IsPresentationValid).HasDefaultValue(false);
        });

        modelBuilder.Entity<ReceivePresentationResponse>(entity =>
        {
            entity.ToTable("ReceivePresentationResponse");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ReceivePresentationId).IsRequired();
            entity.Property(e => e.PresentationReceiptId).IsRequired();
            entity.Property(e => e.RequestPresentationId).IsRequired();
            entity.Property(e => e.ProofOfPresentationId).IsRequired();
        });

        modelBuilder.Entity<ReceiveVisualization>(entity =>
        {
            entity.ToTable("ReceiveVisualization");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.RequestId).IsRequired();
            entity.Property(e => e.ProofOfPresentationId).IsRequired();
            entity.Property(e => e.VCId).IsRequired();
            entity.Property(e => e.FileType).IsRequired();
            entity.Property(e => e.Status).IsRequired();
        });

        modelBuilder.Entity<ReceiveVisualizationResponse>(entity =>
        {
            entity.ToTable("ReceiveVisualizationResponse");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ReceiveVisualizationId).IsRequired();
            entity.Property(e => e.EvidenceVisualizationReceiptID).IsRequired();
            entity.Property(e => e.ReceivePresentationId).IsRequired();
            entity.Property(e => e.ProofOfPresentationId).IsRequired();
        });

        modelBuilder.Entity<VisualizationFile>(entity =>
        {
            entity.ToTable("VisualizationFile");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.File).IsRequired();
            entity.Property(e => e.FileName).IsRequired();
            entity.Property(e => e.ProofOfPresentationId).IsRequired();
            entity.Property(e => e.VisualizationId).IsRequired();
        });

        modelBuilder.Entity<RejectNotification>(entity =>
        {
            entity.ToTable("RejectNotification");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ProofOfPresentationRequestId).IsRequired();
            entity.Property(e => e.RejectReason).IsRequired();
            entity.Property(e => e.PresentationRejectId).IsRequired();
            entity.Property(e => e.RequestPresentationId).IsRequired();
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