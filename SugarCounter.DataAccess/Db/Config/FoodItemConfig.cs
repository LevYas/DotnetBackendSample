using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SugarCounter.Core.Food;

namespace SugarCounter.DataAccess.Db.Config
{
    internal class FoodItemConfig : IEntityTypeConfiguration<FoodItem>
    {
        public void Configure(EntityTypeBuilder<FoodItem> builder)
        {
            builder.HasIndex(u => u.UserInfoId);
            builder.Property(p => p.Description).HasMaxLength(FoodItemLimits.MaxDescriptionLength);
        }
    }
}
