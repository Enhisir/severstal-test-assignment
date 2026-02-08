using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TestAssignment.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rolls",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    length = table.Column<double>(type: "double precision", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: false),
                    date_added = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "TIMEZONE('utc', now())"),
                    date_deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rolls", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "rolls",
                columns: new[] { "id", "date_added", "date_deleted", "length", "weight" },
                values: new object[,]
                {
                    { new Guid("40bed416-0384-403c-b16d-c5c77e3aa12c"), new DateTime(2026, 1, 2, 21, 0, 0, 0, DateTimeKind.Utc), null, 200.0, 200.0 },
                    { new Guid("689f7f26-8cfc-4d58-81ed-6200a43b0813"), new DateTime(2025, 12, 31, 21, 0, 0, 0, DateTimeKind.Utc), null, 1.0, 1.0 },
                    { new Guid("b248061c-1001-482d-8270-af253ee3354d"), new DateTime(2026, 1, 1, 21, 0, 0, 0, DateTimeKind.Utc), null, 100.0, 50.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rolls");
        }
    }
}
