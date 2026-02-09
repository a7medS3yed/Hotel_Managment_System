using HMS.Core.Entities.ServiceModule ;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.Data.Configurations.ServiceConfig
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Core.Entities.ServiceModule .Service>
    {
        public void Configure(EntityTypeBuilder<Core.Entities.ServiceModule.Service> builder)
        {
            builder.ToTable("Services");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                   .UseIdentityColumn(100, 1);

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.Description)
                   .HasMaxLength(500);

            builder.Property(s => s.IsActive)
                   .HasDefaultValue(true);

            builder.Property(s => s.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
