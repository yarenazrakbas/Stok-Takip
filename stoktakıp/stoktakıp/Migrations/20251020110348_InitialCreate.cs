using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace stoktakıp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cihazlar",
                columns: table => new
                {
                    CihazId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CihazAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Marka = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SeriNumarasi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToplamAdet = table.Column<int>(type: "int", nullable: false),
                    MevcutStok = table.Column<int>(type: "int", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cihazlar", x => x.CihazId);
                });

            migrationBuilder.CreateTable(
                name: "TeslimIslemleri",
                columns: table => new
                {
                    IslemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CihazId = table.Column<int>(type: "int", nullable: false),
                    TeslimEden = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TeslimAlan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TeslimTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IadeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IslemTipi = table.Column<int>(type: "int", nullable: false),
                    Adet = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeslimIslemleri", x => x.IslemId);
                    table.ForeignKey(
                        name: "FK_TeslimIslemleri_Cihazlar_CihazId",
                        column: x => x.CihazId,
                        principalTable: "Cihazlar",
                        principalColumn: "CihazId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cihazlar",
                columns: new[] { "CihazId", "CihazAdi", "KayitTarihi", "Marka", "MevcutStok", "Model", "SeriNumarasi", "ToplamAdet" },
                values: new object[,]
                {
                    { 1, "Laptop", new DateTime(2025, 10, 20, 14, 3, 48, 112, DateTimeKind.Local).AddTicks(2720), "Dell", 10, "Latitude 5420", "DELL-LAT-001", 10 },
                    { 2, "Mouse", new DateTime(2025, 10, 20, 14, 3, 48, 112, DateTimeKind.Local).AddTicks(2723), "Logitech", 20, "M185", "LOG-M185-001", 20 },
                    { 3, "Klavye", new DateTime(2025, 10, 20, 14, 3, 48, 112, DateTimeKind.Local).AddTicks(2725), "Logitech", 15, "K120", "LOG-K120-001", 15 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cihazlar_SeriNumarasi",
                table: "Cihazlar",
                column: "SeriNumarasi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeslimIslemleri_CihazId",
                table: "TeslimIslemleri",
                column: "CihazId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeslimIslemleri");

            migrationBuilder.DropTable(
                name: "Cihazlar");
        }
    }
}
