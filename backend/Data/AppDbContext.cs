using Microsoft.EntityFrameworkCore;
using Lab2dotnet3.Models;

namespace Lab2dotnet3;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<Card> Cards { get; set; } = null!;
    public virtual DbSet<Deck> Decks { get; set; } = null!;
    public virtual DbSet<DeckCard> DeckCards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Database collation (matches what your logs show)
        modelBuilder.UseCollation("Finnish_Swedish_CI_AS");

        // ---- Card ----
        modelBuilder.Entity<Card>(entity =>
        {
            entity.ToTable("Cards");
            entity.HasKey(e => e.CardId).HasName("PK__Cards__55FECDAE7607C791");

            entity.Property(e => e.CardId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ManaCost).HasMaxLength(50).IsRequired();

            // IMPORTANT: precision for decimal to avoid truncation warning
            entity.Property(e => e.Price).HasPrecision(18, 2);

            // Don't map the convenience property Id => CardId
            entity.Ignore(e => e.Id);
        });

        // ---- Deck ----
        modelBuilder.Entity<Deck>(entity =>
        {
            entity.ToTable("Decks");
            entity.HasKey(e => e.DeckId).HasName("PK__Decks__76B5446CE0194D97");

            entity.Property(e => e.DeckId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

            // Many-to-many via join entity DeckCard
            entity.HasMany(d => d.Cards).WithMany(c => c.Decks)
                .UsingEntity<DeckCard>(
                    j =>
                        j.HasOne(dc => dc.Card)
                         .WithMany()
                         .HasForeignKey(dc => dc.CardId)
                         .OnDelete(DeleteBehavior.Cascade),
                    j =>
                        j.HasOne(dc => dc.Deck)
                         .WithMany()
                         .HasForeignKey(dc => dc.DeckId)
                         .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.ToTable("DeckCards");
                        j.HasKey(dc => new { dc.DeckId, dc.CardId }).HasName("PK__DeckCard__83EAA8B630652EB9");
                        j.HasIndex(dc => dc.CardId).HasDatabaseName("IX_DeckCards_CardId");
                    });
        });

        // If you also want to configure the join entity explicitly (optional since we did it above):
        modelBuilder.Entity<DeckCard>(entity =>
        {
            entity.ToTable("DeckCards");
            entity.HasKey(e => new { e.DeckId, e.CardId }).HasName("PK__DeckCard__83EAA8B630652EB9");
            entity.HasIndex(e => e.CardId).HasDatabaseName("IX_DeckCards_CardId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
