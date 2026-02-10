using HMS.Core.Entities.RoomModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.Data.Configurations.RoomConfig
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.Property(r => r.Id)
                .UseIdentityColumn(100, 1);

            builder.Property(r => r.Description)
                .HasMaxLength(150);

            builder.Property(r => r.PricePerNight)
                .HasColumnType("decimal(18,2)");

            builder.Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            


        }
    }
}
