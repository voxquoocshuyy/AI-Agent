using AI.Agent.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace AI.Agent.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.Content)
            .IsRequired();

        builder.Property(d => d.FileType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.LastModifiedAt);

        builder.Property(d => d.IsProcessed)
            .IsRequired();

        builder.Property(d => d.ProcessingError)
            .HasMaxLength(1000);

        // Add full-text search index as a shadow property
        builder.Property<NpgsqlTypes.NpgsqlTsVector>("SearchVector").HasColumnType("tsvector");
        builder.HasIndex("SearchVector").HasMethod("GIN");
        // TODO: Add HasGeneratedTsVectorColumn when Npgsql.EntityFrameworkCore.PostgreSQL supports shadow property overload
    }
} 