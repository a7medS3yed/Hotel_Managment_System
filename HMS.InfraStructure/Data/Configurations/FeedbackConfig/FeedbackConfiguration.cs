using HMS.Core.Entities.FeedbackModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.Data.Configurations.FeedbackConfig
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedbacks");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Content)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(f => f.Status)
                   .IsRequired();

            builder.Property(f => f.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(f => f.Guest)
                   .WithMany()
                   .HasForeignKey(f => f.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
