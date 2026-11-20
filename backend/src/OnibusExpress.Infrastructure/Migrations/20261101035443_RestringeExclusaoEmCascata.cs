using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnibusExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestringeExclusaoEmCascata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Passageiros_PassageiroId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Viagens_ViagemId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Viagens_Rotas_RotaId",
                table: "Viagens");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Passageiros_PassageiroId",
                table: "Reservas",
                column: "PassageiroId",
                principalTable: "Passageiros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Viagens_ViagemId",
                table: "Reservas",
                column: "ViagemId",
                principalTable: "Viagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Viagens_Rotas_RotaId",
                table: "Viagens",
                column: "RotaId",
                principalTable: "Rotas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Passageiros_PassageiroId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Viagens_ViagemId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Viagens_Rotas_RotaId",
                table: "Viagens");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Passageiros_PassageiroId",
                table: "Reservas",
                column: "PassageiroId",
                principalTable: "Passageiros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Viagens_ViagemId",
                table: "Reservas",
                column: "ViagemId",
                principalTable: "Viagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Viagens_Rotas_RotaId",
                table: "Viagens",
                column: "RotaId",
                principalTable: "Rotas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
