using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorFuncionarios.Migrations
{
    /// <inheritdoc />
    public partial class MakeDepartamentoNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_departamentos_DepartamentoId",
                table: "Usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_departamentos_DepartamentoId",
                table: "Usuario",
                column: "DepartamentoId",
                principalTable: "departamentos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_departamentos_DepartamentoId",
                table: "Usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_departamentos_DepartamentoId",
                table: "Usuario",
                column: "DepartamentoId",
                principalTable: "departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
