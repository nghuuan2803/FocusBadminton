using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    PersonalPoints = table.Column<double>(type: "float", nullable: false),
                    RewardPoints = table.Column<double>(type: "float", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessRules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsApplied = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Latitude = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Longitude = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PlaceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Layout = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Coofficient = table.Column<double>(type: "float", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ManagerId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Receiver = table.Column<string>(type: "nvarchar(46)", maxLength: 46, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Data = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsChecked = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<double>(type: "float", nullable: false),
                    Facilities = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TiersRequired = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaximunValue = table.Column<double>(type: "float", nullable: true),
                    BeginAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Images = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamTiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinPoints = table.Column<double>(type: "float", nullable: false),
                    DiscountPercent = table.Column<double>(type: "float", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamTiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Duration = table.Column<double>(type: "float", nullable: false),
                    IsApplied = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    MaximumValue = table.Column<double>(type: "float", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "varchar(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "varchar(36)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    UserId = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(36)", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(36)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FacilityId = table.Column<int>(type: "int", nullable: true),
                    Coofficient = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courts_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    MaximumValue = table.Column<double>(type: "float", nullable: false),
                    VoucherTemplateId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<string>(type: "varchar(36)", nullable: true),
                    Expiry = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vouchers_AspNetUsers_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vouchers_VoucherTemplates_VoucherTemplateId",
                        column: x => x.VoucherTemplateId,
                        principalTable: "VoucherTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BookingHolds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    TimeSlotId = table.Column<int>(type: "int", nullable: false),
                    HeldBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeldAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    BookingType = table.Column<int>(type: "int", nullable: false),
                    BeginAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DayOfWeek = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingHolds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingHolds_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingHolds_TimeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: false),
                    TimeSlotId = table.Column<int>(type: "int", nullable: false),
                    BeginAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DayOfWeek = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amonut = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "Courts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingDetails_TimeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    EstimateCost = table.Column<double>(type: "float", nullable: false),
                    Deposit = table.Column<double>(type: "float", nullable: false),
                    VoucherId = table.Column<int>(type: "int", nullable: true),
                    PromotionId = table.Column<int>(type: "int", nullable: true),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    PausedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ResumeDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AdminNote = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: true),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Info = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContributionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeftHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LeftAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeftHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Contributed = table.Column<double>(type: "float", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrentTeamId = table.Column<int>(type: "int", nullable: true),
                    JoinedTeamAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OldTeam = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    DoB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AccountId = table.Column<string>(type: "varchar(36)", nullable: true),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_AspNetUsers_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TeamTierId = table.Column<int>(type: "int", nullable: true),
                    LeaderId = table.Column<int>(type: "int", nullable: false),
                    TeamPoints = table.Column<double>(type: "float", nullable: false),
                    RewardPoints = table.Column<double>(type: "float", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Members_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "Name", "NormalizedName", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { "1ce72cac-5134-437d-bd03-0f76c5180afe", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", "sys_admin", "SYS_ADMIN", null, null },
                    { "225a8b8f-6895-4429-8e64-76355415fa94", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", "manager", "MANAGER", null, null },
                    { "3a8535c5-15c8-4d21-b9f4-d47cf0b4ef0b", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", "coach", "COACH", null, null },
                    { "40632c41-b76e-4a46-b74a-4f2f4d42661d", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", "student", "STUDENT", null, null },
                    { "7a4dff7f-3d6a-4883-800b-7103ce57af94", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", "member", "MEMBER", null, null },
                    { "9af7d912-ca02-41ce-a82f-86e859573129", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", "guest", "GUEST", null, null },
                    { "bdd06cc1-4b82-48ce-9aa2-2f574bd1896c", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", "admin", "ADMIN", null, null },
                    { "be4ab03e-0ce3-4ee0-ba17-2a666a185455", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "System", "team_leader", "TEAM_LEADER", null, null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedAt", "CreatedBy", "DeleteDate", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PersonalPoints", "PhoneNumber", "PhoneNumberConfirmed", "RewardPoints", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UpdatedBy", "UserName" },
                values: new object[,]
                {
                    { "0cb31850-6e26-43b0-bd66-ae58e99cad7d", 0, null, "90a111b3-0909-4785-8233-4cc1699ef392", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 51, DateTimeKind.Unspecified).AddTicks(7710), new TimeSpan(0, 0, 0, 0, 0)), "System", null, "anhuu2803@gmail.com", true, false, null, "ANHUU2803@GMAIL.COM", "manager", "AQAAAAIAAYagAAAAEIPlDXpCNVZx6+lVpKlCydBDkUHB0wYoWaTU6eHL4BQQ/NB7Z/rec8RcEWnkEqWeRA==", 0.0, null, false, 0.0, null, false, null, null, "manager" },
                    { "845d2699-2419-4cd6-96ac-1d592f143e41", 0, null, "6c5af02b-2215-492d-b39f-b4da57beb62a", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 51, DateTimeKind.Unspecified).AddTicks(7707), new TimeSpan(0, 0, 0, 0, 0)), "System", null, "anhuu2803@gmail.com", true, false, null, "ANHUU2803@GMAIL.COM", "SYS_ADMIN", "AQAAAAIAAYagAAAAEKg/WJQn6sP4pUDB5/UP2wEN08hhcH0ShlMIG4717/3Y0qCVjqc66onMuDggcxZsxg==", 0.0, null, false, 0.0, null, false, null, null, "sys_admin" },
                    { "8c18473e-f0be-4202-bc37-38ced67318cb", 0, null, "760ec17b-0bef-4d93-8775-eb4e3fcaf196", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 51, DateTimeKind.Unspecified).AddTicks(7700), new TimeSpan(0, 0, 0, 0, 0)), "System", null, "nghuuan2803@gmail.com", true, false, null, "NGHUUAN2803@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAECte86TXvA06vCnFejPOSjIljRJKWWwzSbqXT2zPAATIrGvhmhRgm62eDLEh/AtfSw==", 0.0, null, false, 0.0, null, false, null, null, "admin" }
                });

            migrationBuilder.InsertData(
                table: "BusinessRules",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsApplied", "Name", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[,]
                {
                    { "close_time", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2592), new TimeSpan(0, 0, 0, 0, 0)), "system", null, true, "Giờ đóng cửa", null, null, "22:00" },
                    { "deposit_rate_fixed", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2598), new TimeSpan(0, 0, 0, 0, 0)), "system", null, true, "Yêu cầu đặt cọc khi đặt cố định", null, null, "1 d" },
                    { "deposit_rate_hourly", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2597), new TimeSpan(0, 0, 0, 0, 0)), "system", null, false, "Yêu cầu đặt cọc khi đặt theo giờ", null, null, "0.2 p" },
                    { "login_fixed", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2667), new TimeSpan(0, 0, 0, 0, 0)), "system", null, true, "Yêu cầu đăng nhập khi đặt cố định", null, null, "" },
                    { "login_hourly", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2600), new TimeSpan(0, 0, 0, 0, 0)), "system", null, false, "Yêu cầu đăng nhập khi đặt theo giờ", null, null, "" },
                    { "open_time", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2588), new TimeSpan(0, 0, 0, 0, 0)), "system", null, true, "Giờ mở cửa", null, null, "05:00" },
                    { "payment_hourly", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2599), new TimeSpan(0, 0, 0, 0, 0)), "system", null, true, "Yêu cầu thanh toán khi đặt theo giờ", null, null, "" },
                    { "price_friday", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2594), new TimeSpan(0, 0, 0, 0, 0)), "system", null, true, "Chỉnh giá thứ 6", null, null, "1.1" },
                    { "price_saturday", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2595), new TimeSpan(0, 0, 0, 0, 0)), "system", null, true, "Chỉnh giá thứ 7", null, null, "1.2" },
                    { "price_sunday", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2596), new TimeSpan(0, 0, 0, 0, 0)), "system", null, true, "Chỉnh giá Chủ nhật", null, null, "1.2" },
                    { "release_slot", new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(2668), new TimeSpan(0, 0, 0, 0, 0)), "system", null, true, "Thời gian tự nhả lịch nếu không đặt", null, null, "5:00" }
                });

            migrationBuilder.InsertData(
                table: "Facilities",
                columns: new[] { "Id", "Address", "Coofficient", "CreatedAt", "CreatedBy", "Images", "Latitude", "Layout", "Longitude", "ManagerId", "Name", "PlaceId", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, "16 Đ. 53, Phường 14, Gò Vấp, Hồ Chí Minh", 1.0, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(7025), new TimeSpan(0, 0, 0, 0, 0)), "system", null, "10.850212299999999", null, "106.64369049999999", null, "Sân cầu Focus", null, 1, null, null });

            migrationBuilder.InsertData(
                table: "TeamTiers",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DiscountPercent", "Image", "MinPoints", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5520), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 0.050000000000000003, null, 500000.0, "Đồng", null, null },
                    { 2, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5524), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 0.10000000000000001, null, 1500000.0, "Bạc", null, null },
                    { 3, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5525), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 0.14999999999999999, null, 5000000.0, "Vàng", null, null },
                    { 4, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5526), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 0.20000000000000001, null, 15000000.0, "Kim Cương", null, null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Duration", "EndTime", "IsApplied", "IsDeleted", "Price", "StartTime", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5788), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 5, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 5, 0, 0, 0), null, null },
                    { 2, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5857), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 6, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 5, 30, 0, 0), null, null },
                    { 3, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5877), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 6, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 6, 0, 0, 0), null, null },
                    { 4, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5933), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 7, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 6, 30, 0, 0), null, null },
                    { 5, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6125), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 7, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 7, 0, 0, 0), null, null },
                    { 6, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6299), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 8, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 7, 30, 0, 0), null, null },
                    { 7, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6323), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 8, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 8, 0, 0, 0), null, null },
                    { 8, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6344), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 9, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 8, 30, 0, 0), null, null },
                    { 9, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6363), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 9, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 9, 0, 0, 0), null, null },
                    { 10, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6383), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 10, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 9, 30, 0, 0), null, null },
                    { 11, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6401), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 10, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 10, 0, 0, 0), null, null },
                    { 12, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6419), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 11, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 10, 30, 0, 0), null, null },
                    { 13, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6439), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 11, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 11, 0, 0, 0), null, null },
                    { 14, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6457), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 12, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 11, 30, 0, 0), null, null },
                    { 15, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6484), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 12, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 12, 0, 0, 0), null, null },
                    { 16, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6501), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 13, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 12, 30, 0, 0), null, null },
                    { 17, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6519), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 13, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 13, 0, 0, 0), null, null },
                    { 18, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6544), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 14, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 13, 30, 0, 0), null, null },
                    { 19, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6561), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 14, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 14, 0, 0, 0), null, null },
                    { 20, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6580), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 15, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 14, 30, 0, 0), null, null },
                    { 21, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6598), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 15, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 15, 0, 0, 0), null, null },
                    { 22, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6616), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 16, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 15, 30, 0, 0), null, null },
                    { 23, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6633), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 16, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 16, 0, 0, 0), null, null },
                    { 24, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6734), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 17, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 16, 30, 0, 0), null, null },
                    { 25, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6756), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 17, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 17, 0, 0, 0), null, null },
                    { 26, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6778), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 18, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 17, 30, 0, 0), null, null },
                    { 27, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6796), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 18, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 18, 0, 0, 0), null, null },
                    { 28, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6813), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 19, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 18, 30, 0, 0), null, null },
                    { 29, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6831), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 19, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 19, 0, 0, 0), null, null },
                    { 30, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6849), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 20, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 19, 30, 0, 0), null, null },
                    { 31, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6877), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 20, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 20, 0, 0, 0), null, null },
                    { 32, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6895), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 21, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 20, 30, 0, 0), null, null },
                    { 33, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6913), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 21, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 21, 0, 0, 0), null, null },
                    { 34, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6933), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 22, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 21, 30, 0, 0), null, null },
                    { 35, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6951), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 22, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 22, 0, 0, 0), null, null },
                    { 36, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6969), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 23, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 22, 30, 0, 0), null, null },
                    { 37, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(6986), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(0, 23, 30, 0, 0), true, false, 50000.0, new TimeSpan(0, 23, 0, 0, 0), null, null },
                    { 38, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(7005), new TimeSpan(0, 0, 0, 0, 0)), "system", 0.5, new TimeSpan(1, 0, 0, 0, 0), true, false, 50000.0, new TimeSpan(0, 23, 30, 0, 0), null, null }
                });

            migrationBuilder.InsertData(
                table: "VoucherTemplates",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "DiscountType", "Duration", "MaximumValue", "Name", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5630), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 1, 30, 0.0, "Phiếu giảm giá 10%", null, null, 10.0 },
                    { 2, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5628), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 1, 30, 0.0, "Phiếu giảm giá 15%", null, null, 15.0 },
                    { 3, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5631), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 1, 15, 0.0, "Phiếu giảm giá 20%", null, null, 20.0 },
                    { 4, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5633), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 0, 15, 0.0, "Phiếu giảm giá 20.000đ", null, null, 20000.0 },
                    { 5, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(5634), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 0, 15, 0.0, "Phiếu giảm giá 30.000đ", null, null, 30000.0 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "225a8b8f-6895-4429-8e64-76355415fa94", "0cb31850-6e26-43b0-bd66-ae58e99cad7d" },
                    { "1ce72cac-5134-437d-bd03-0f76c5180afe", "845d2699-2419-4cd6-96ac-1d592f143e41" },
                    { "bdd06cc1-4b82-48ce-9aa2-2f574bd1896c", "8c18473e-f0be-4202-bc37-38ced67318cb" }
                });

            migrationBuilder.InsertData(
                table: "Courts",
                columns: new[] { "Id", "Coofficient", "CreatedAt", "CreatedBy", "Description", "FacilityId", "Images", "Name", "Status", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 1.2, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(7060), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 1, null, "Sân 1 (VIP)", 1, 1, null, null },
                    { 2, 1.2, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(7061), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 1, null, "Sân 2 (VIP)", 1, 1, null, null },
                    { 3, 1.0, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(7063), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 1, null, "Sân 3", 1, 0, null, null },
                    { 4, 1.0, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(7063), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 1, null, "Sân 4", 1, 0, null, null },
                    { 5, 1.0, new DateTimeOffset(new DateTime(2025, 4, 3, 5, 56, 20, 283, DateTimeKind.Unspecified).AddTicks(7065), new TimeSpan(0, 0, 0, 0, 0)), "system", null, 1, null, "Sân 5", 1, 0, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_BookingId",
                table: "BookingDetails",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_CourtId",
                table: "BookingDetails",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_TimeSlotId",
                table: "BookingDetails",
                column: "TimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingHolds_CourtId",
                table: "BookingHolds",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingHolds_TimeSlotId",
                table: "BookingHolds",
                column: "TimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MemberId",
                table: "Bookings",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PromotionId",
                table: "Bookings",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TeamId",
                table: "Bookings",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VoucherId",
                table: "Bookings",
                column: "VoucherId",
                unique: true,
                filter: "[VoucherId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionHistories_MemberId",
                table: "ContributionHistories",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ContributionHistories_TeamId",
                table: "ContributionHistories",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Courts_FacilityId",
                table: "Courts",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LeftHistories_MemberId",
                table: "LeftHistories",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_LeftHistories_TeamId",
                table: "LeftHistories",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_AccountId",
                table: "Members",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Members_CurrentTeamId",
                table: "Members",
                column: "CurrentTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_TeamId",
                table: "Members",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeaderId",
                table: "Teams",
                column: "LeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_AccountId",
                table: "Vouchers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_VoucherTemplateId",
                table: "Vouchers",
                column: "VoucherTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Bookings_BookingId",
                table: "BookingDetails",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Members_MemberId",
                table: "Bookings",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Teams_TeamId",
                table: "Bookings",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionHistories_Members_MemberId",
                table: "ContributionHistories",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContributionHistories_Teams_TeamId",
                table: "ContributionHistories",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeftHistories_Members_MemberId",
                table: "LeftHistories",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeftHistories_Teams_TeamId",
                table: "LeftHistories",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Teams_CurrentTeamId",
                table: "Members",
                column: "CurrentTeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Teams_TeamId",
                table: "Members",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_AccountId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Members_LeaderId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BookingDetails");

            migrationBuilder.DropTable(
                name: "BookingHolds");

            migrationBuilder.DropTable(
                name: "BusinessRules");

            migrationBuilder.DropTable(
                name: "ContributionHistories");

            migrationBuilder.DropTable(
                name: "LeftHistories");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "TeamTiers");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Courts");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "VoucherTemplates");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
