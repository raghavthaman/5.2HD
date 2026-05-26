using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AboriginalArtGallery.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAndOrderSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForPurchase",
                table: "Artifacts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Artifacts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    BillingFirstName = table.Column<string>(type: "text", nullable: false),
                    BillingLastName = table.Column<string>(type: "text", nullable: false),
                    BillingAddressLine1 = table.Column<string>(type: "text", nullable: false),
                    BillingAddressLine2 = table.Column<string>(type: "text", nullable: true),
                    BillingCity = table.Column<string>(type: "text", nullable: false),
                    BillingState = table.Column<string>(type: "text", nullable: false),
                    BillingPostCode = table.Column<string>(type: "text", nullable: false),
                    BillingCountry = table.Column<string>(type: "text", nullable: false),
                    BillingPhone = table.Column<string>(type: "text", nullable: false),
                    SameAsDelivery = table.Column<bool>(type: "boolean", nullable: false),
                    DeliveryFirstName = table.Column<string>(type: "text", nullable: true),
                    DeliveryLastName = table.Column<string>(type: "text", nullable: true),
                    DeliveryAddressLine1 = table.Column<string>(type: "text", nullable: true),
                    DeliveryAddressLine2 = table.Column<string>(type: "text", nullable: true),
                    DeliveryCity = table.Column<string>(type: "text", nullable: true),
                    DeliveryState = table.Column<string>(type: "text", nullable: true),
                    DeliveryPostCode = table.Column<string>(type: "text", nullable: true),
                    DeliveryCountry = table.Column<string>(type: "text", nullable: true),
                    PreferredDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeliveryTimeSlot = table.Column<string>(type: "text", nullable: false),
                    IsGift = table.Column<bool>(type: "boolean", nullable: false),
                    GiftMessage = table.Column<string>(type: "text", nullable: true),
                    GiftWrapStyle = table.Column<string>(type: "text", nullable: true),
                    CouponCode = table.Column<string>(type: "text", nullable: true),
                    RedeemCode = table.Column<string>(type: "text", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountType = table.Column<string>(type: "text", nullable: true),
                    PaymentMethod = table.Column<string>(type: "text", nullable: false),
                    CardLastFour = table.Column<string>(type: "text", nullable: true),
                    CardBrand = table.Column<string>(type: "text", nullable: true),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    ShippingCost = table.Column<decimal>(type: "numeric", nullable: false),
                    GiftWrapFee = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxUses = table.Column<int>(type: "integer", nullable: false),
                    UsedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ArtifactId = table.Column<int>(type: "integer", nullable: false),
                    ArtifactTitle = table.Column<string>(type: "text", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Artifacts_ArtifactId",
                        column: x => x.ArtifactId,
                        principalTable: "Artifacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PromoCodes",
                columns: new[] { "Id", "Code", "ExpiresAt", "IsActive", "MaxUses", "Type", "UsedCount", "Value" },
                values: new object[,]
                {
                    { 1, "WELCOME20", null, true, 100, "percentage", 0, 20m },
                    { 2, "MINUS50", null, true, 100, "fixed", 0, 50m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ArtifactId",
                table: "OrderItems",
                column: "ArtifactId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropColumn(
                name: "IsAvailableForPurchase",
                table: "Artifacts");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Artifacts");
        }
    }
}
