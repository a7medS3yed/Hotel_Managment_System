using HMS.Core.Entities.ServiceModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.Data.Configurations.ServiceConfig
{
    public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            builder.ToTable("ServiceRequests");

            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.Id)
                   .UseIdentityColumn(100, 1);

            builder.Property(sr => sr.Status)
                   .IsRequired();

            builder.Property(sr => sr.Notes)
                   .HasMaxLength(500);

            builder.Property(sr => sr.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(sr => sr.Service)
                   .WithMany()
                   .HasForeignKey(sr => sr.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sr => sr.Guest)
                   .WithMany()
                   .HasForeignKey(sr => sr.GuestId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sr => sr.Staff)
                   .WithMany()
                   .HasForeignKey(sr => sr.StaffId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

}
