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
    public class RoomImageConfiguration : IEntityTypeConfiguration<RoomImage>
    {
        public void Configure(EntityTypeBuilder<RoomImage> builder)
        {
            builder.Property(ri => ri.ImageUrl)
                .HasMaxLength(500);

            builder.Property(ri => ri.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            //    // Relationships
            //    builder.HasOne<Room>()
            //        .WithMany(r => r.RoomImages)
            //        .HasForeignKey(ri => ri.RoomId)
            //        .OnDelete(DeleteBehavior.Cascade);
            //}
        }
    }
}
