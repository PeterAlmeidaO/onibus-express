using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnibusExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IndiceUnicoAssentoConfirmado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservas_ViagemId",
                table: "Reservas");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ViagemId_NumeroAssento_Confirmada",
                table: "Reservas",
                columns: new[] { "ViagemId", "NumeroAssento" },
                unique: true,
                filter: "\"Status\" = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservas_ViagemId_NumeroAssento_Confirmada",
                table: "Reservas");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ViagemId",
                table: "Reservas",
                column: "ViagemId");
        }
    }
}
