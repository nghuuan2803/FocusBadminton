using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<Account, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<Facility> Facilities { get; set; }
        public virtual DbSet<Court> Courts { get; set; }
        public virtual DbSet<TeamTier> TeamTiers { get; set; }
        public virtual DbSet<VoucherTemplate> VoucherTemplates { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }

        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingDetail> BookingDetails { get; set; }
        public virtual DbSet<BusinessRule> BusinessRules { get; set; }
        public virtual DbSet<Member> Members { get; set; }

        public virtual DbSet<TimeSlot> TimeSlots { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<LeftHistory> LeftHistories { get; set; }
        public virtual DbSet<ContributionHistory> ContributionHistories { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<BookingHold> BookingHolds { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ConfigEntities(builder);
            SeedData(builder);
        }
        private void ConfigEntities(ModelBuilder builder)
        {
            builder.Entity<Member>().HasOne(p => p.Account).WithOne().HasForeignKey<Member>(p => p.AccountId);
            builder.Entity<Member>().HasOne(p => p.CurrentTeam).WithMany().HasForeignKey(p=>p.CurrentTeamId);
            builder.Entity<Team>().HasOne(p => p.Leader).WithOne().HasForeignKey<Team>(p => p.LeaderId);
            builder.Entity<Booking>().HasOne(p => p.Voucher).WithOne().HasForeignKey<Booking>(p => p.VoucherId);
            builder.Entity<LeftHistory>().HasOne(p => p.Team).WithMany().HasForeignKey(p => p.TeamId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ContributionHistory>().HasOne(p => p.Team).WithMany().HasForeignKey(p => p.TeamId).OnDelete(DeleteBehavior.Restrict);
            
            
            builder.Entity<BusinessRule>().Property(p => p.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<Account>().Property(p => p.Id).HasMaxLength(36).IsUnicode(false);
            builder.Entity<Role>().Property(p => p.Id).HasMaxLength(36).IsUnicode(false);
            builder.Entity<BookingDetail>().Property(p => p.DayOfWeek).HasMaxLength(10).IsUnicode(false);
            builder.Entity<BookingHold>().Property(p => p.DayOfWeek).HasMaxLength(10).IsUnicode(false);
            builder.Entity<Account>().Property(p => p.PhoneNumber).HasMaxLength(15).IsUnicode(false);
            builder.Entity<IdentityUserLogin<string>>().Property(p => p.ProviderDisplayName).HasMaxLength(50).IsUnicode(false);
            builder.Entity<IdentityUserClaim<string>>().Property(p => p.ClaimType).HasMaxLength(100).IsUnicode(true);
            builder.Entity<IdentityUserClaim<string>>().Property(p => p.ClaimValue).HasMaxLength(50).IsUnicode(true);
            builder.Entity<IdentityRoleClaim<string>>().Property(p => p.ClaimType).HasMaxLength(100).IsUnicode(true);
            builder.Entity<IdentityRoleClaim<string>>().Property(p => p.ClaimValue).HasMaxLength(50).IsUnicode(true);
            builder.Entity<IdentityUserToken<string>>().Property(p => p.Name).HasMaxLength(50).IsUnicode(false);
        }
        private void SeedData(ModelBuilder builder)
        {
            AddRoles(builder);
            AddAdmin(builder);
            AddBusinesRules(builder);
            AddTeamTiers(builder);
            AddVoucherTemplates(builder);
            AddTimeSlots(builder);
            AddFacilities(builder);
            AddCourts(builder);
        }
        private void AddRoles(ModelBuilder builder)
        {
            string adminRoleId = "bdd06cc1-4b82-48ce-9aa2-2f574bd1896c";
            string customerRoleId = "9af7d912-ca02-41ce-a82f-86e859573129";
            var customerRole = new Role
            {
                Id = customerRoleId,
                Name = "Customer",
                NormalizedName = "CUSTOMER"
            };
            var adminRole = new Role
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            builder.Entity<Role>().HasData(customerRole);
            builder.Entity<Role>().HasData(adminRole);
        }

        private void AddAdmin(ModelBuilder builder)
        {
            var adminUser = new Account
            {
                Id = "8c18473e-f0be-4202-bc37-38ced67318cb",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "nghuuan2803@gmail.com",
                NormalizedEmail = "nghuuan2803@gmail.com".ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "System"
            };

            // Hash mật khẩu admin
            var passwordHasher = new PasswordHasher<Account>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");
            builder.Entity<Account>().HasData(adminUser);

            var adminUserRole = new IdentityUserRole<string>
            {
                UserId = "8c18473e-f0be-4202-bc37-38ced67318cb",
                RoleId = "bdd06cc1-4b82-48ce-9aa2-2f574bd1896c"
            };
            builder.Entity<IdentityUserRole<string>>().HasData(adminUserRole);
        }
        private void AddBusinesRules(ModelBuilder builder)
        {
            var openTimeRule = new BusinessRule
            {
                Id = "open_time",
                Name = "Giờ mở cửa",
                Value = "05:00",
                IsApplied = true,
            };
            var closeTimeRule = new BusinessRule
            {
                Id = "close_time",
                Name = "Giờ đóng cửa",
                Value = "22:00",
                IsApplied = true,
            };

            var priceOnFridayRule = new BusinessRule
            {
                Id = "price_friday",
                Name = "Chỉnh giá thứ 6",
                Value = "1.1",
                IsApplied = true,
            };
            var priceOnSaturdayRule = new BusinessRule
            {
                Id = "price_saturday",
                Name = "Chỉnh giá thứ 7",
                Value = "1.2",
                IsApplied = true,
            };
            var priceOnSundayRule = new BusinessRule
            {
                Id = "price_sunday",
                Name = "Chỉnh giá Chủ nhật",
                Value = "1.2",
                IsApplied = true,
            };

            var depositRateHourly = new BusinessRule
            {
                Id = "deposit_rate_hourly",
                Name = "Yêu cầu đặt cọc khi đặt theo giờ",
                Value = "0.2 p",
                IsApplied = false,
            };
            var depositRateFixed = new BusinessRule
            {
                Id = "deposit_rate_fixed",
                Name = "Yêu cầu đặt cọc khi đặt cố định",
                Value = "1 d",
                IsApplied = true,
            };

            var paymentRequireHourly = new BusinessRule
            {
                Id = "payment_hourly",
                Name = "Yêu cầu thanh toán khi đặt theo giờ",
                Value = string.Empty,
                IsApplied = true,
            };

            var requireLoginHourly = new BusinessRule
            {
                Id = "login_hourly",
                Name = "Yêu cầu đăng nhập khi đặt theo giờ",
                Value = string.Empty,
                IsApplied = false,
            };
            var requireLoginFixed = new BusinessRule
            {
                Id = "login_fixed",
                Name = "Yêu cầu đăng nhập khi đặt cố định",
                Value = string.Empty,
                IsApplied = true,
            };

            var releaseSlotTime = new BusinessRule
            {
                Id = "release_slot",
                Name = "Thời gian tự nhả lịch nếu không đặt",
                Value = "5:00",
                IsApplied = true,
            };

            builder.Entity<BusinessRule>().HasData(openTimeRule);
            builder.Entity<BusinessRule>().HasData(closeTimeRule);
            builder.Entity<BusinessRule>().HasData(priceOnFridayRule);
            builder.Entity<BusinessRule>().HasData(priceOnSaturdayRule);
            builder.Entity<BusinessRule>().HasData(priceOnSundayRule);
            builder.Entity<BusinessRule>().HasData(depositRateHourly);
            builder.Entity<BusinessRule>().HasData(depositRateFixed);
            builder.Entity<BusinessRule>().HasData(paymentRequireHourly);
            builder.Entity<BusinessRule>().HasData(requireLoginHourly);
            builder.Entity<BusinessRule>().HasData(requireLoginFixed);
            builder.Entity<BusinessRule>().HasData(releaseSlotTime);
        }
        private void AddTeamTiers(ModelBuilder builder)
        {
            var tier1 = new TeamTier
            {
                Id = 1,
                Name = "Đồng",
                DiscountPercent = 0.05,
                MinPoints = 500000,
            };
            var tier2 = new TeamTier
            {
                Id = 2,
                Name = "Bạc",
                DiscountPercent = 0.1,
                MinPoints = 1500000,
            };
            var tier3 = new TeamTier
            {
                Id = 3,
                Name = "Vàng",
                DiscountPercent = 0.15,
                MinPoints = 5000000,
            };
            var tier4 = new TeamTier
            {
                Id = 4,
                Name = "Kim Cương",
                DiscountPercent = 0.20,
                MinPoints = 15000000,
            };

            builder.Entity<TeamTier>().HasData(tier1);
            builder.Entity<TeamTier>().HasData(tier2);
            builder.Entity<TeamTier>().HasData(tier3);
            builder.Entity<TeamTier>().HasData(tier4);
        }
        private void AddTimeSlots(ModelBuilder builder)
        {
            int minute = 0;
            double duration = 1;
            int i = 0;
            while (true)
            {
                i++;
                TimeSpan startTime = new TimeSpan(5, minute, 0);
                TimeSpan endTime = new TimeSpan(5, minute + (int)(duration * 60), 0);
                var time = new TimeSlot
                {
                    Id = i,
                    StartTime = startTime,
                    EndTime = endTime,
                    Duration = duration,
                    CreatedAt = DateTimeOffset.Now,
                };
                if (time.StartTime.Days > 0 || endTime.Hours > 22)
                    break;
                builder.Entity<TimeSlot>().HasData(time);
                minute += (int)(60 * duration);
            }
        }
        private void AddVoucherTemplates(ModelBuilder builder)
        {
            var v15 = new VoucherTemplate
            {
                Id = 2,
                Name = "Phiếu giảm giá 15%",
                DiscountType = DiscountType.Percent,
                Value = 15,
                MaximumValue = 0,
                Duration = 30,
            };
            var v10 = new VoucherTemplate
            {
                Id = 1,
                Name = "Phiếu giảm giá 10%",
                DiscountType = DiscountType.Percent,
                Value = 10,
                MaximumValue = 0,
                Duration = 30,
            };
            var v20 = new VoucherTemplate
            {
                Id = 3,
                Name = "Phiếu giảm giá 20%",
                DiscountType = DiscountType.Percent,
                Value = 20,
                MaximumValue = 0,
                Duration = 15,
            };
            var v20k = new VoucherTemplate
            {
                Id = 4,
                Name = "Phiếu giảm giá 20.000đ",
                DiscountType = DiscountType.Point,
                Value = 20000,
                MaximumValue = 0,
                Duration = 15,
            };
            var v30k = new VoucherTemplate
            {
                Id = 5,
                Name = "Phiếu giảm giá 30.000đ",
                DiscountType = DiscountType.Point,
                Value = 30000,
                MaximumValue = 0,
                Duration = 15,
            };
            builder.Entity<VoucherTemplate>().HasData(v10);
            builder.Entity<VoucherTemplate>().HasData(v15);
            builder.Entity<VoucherTemplate>().HasData(v20);
            builder.Entity<VoucherTemplate>().HasData(v20k);
            builder.Entity<VoucherTemplate>().HasData(v30k);
        }
        private void AddFacilities(ModelBuilder builder)
        {
            var f1 = new Facility
            {
                Id = 1,
                Name = "Sân cầu Focus",
                Address = "16 Đ. 53, Phường 14, Gò Vấp, Hồ Chí Minh",
                Latitude = "10.850212299999999",
                Longitude = "106.64369049999999",
            };           
            builder.Entity<Facility>().HasData(f1);
        }
        private void AddCourts(ModelBuilder builder)
        {
            var c1 = new Court
            {
                Id = 1,
                Name = "Sân 1 (VIP)",
                FacilityId = 1,
                Coofficient = 1.2,
                Type = CourtType.VIP,
            };
            var c2 = new Court
            {
                Id = 2,
                Name = "Sân 2 (VIP)",
                FacilityId = 1,
                Coofficient = 1.2,
                Type = CourtType.VIP,
            };
            var c3 = new Court
            {
                Id = 3,
                Name = "Sân 3",
                FacilityId = 1,
            };
            var c4 = new Court
            {
                Id = 4,
                Name = "Sân 4",
                FacilityId = 1,
            };

            var c5 = new Court
            {
                Id = 5,
                Name = "Sân 5",
                FacilityId = 1,
            };

            builder.Entity<Court>().HasData(c1);
            builder.Entity<Court>().HasData(c2);
            builder.Entity<Court>().HasData(c3);
            builder.Entity<Court>().HasData(c4);
            builder.Entity<Court>().HasData(c5);
        }
    }
}
