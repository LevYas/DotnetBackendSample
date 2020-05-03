using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SugarCounter.Core.Users;
using SugarCounter.DataAccess.Utils;

namespace SugarCounter.DataAccess.Db.Config
{
    internal class UserInfoConfig : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.HasIndex(u => u.Login).IsUnique();
            builder.Property(p => p.Login).HasMaxLength(UserInfoLimits.MaxLoginLength);
            builder.Property(p => p.PasswordHash).HasMaxLength(PasswordHasher.HashedStringLength);
        }
    }
}
