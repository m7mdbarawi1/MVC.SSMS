using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSMS.Migrations
{
    /// <inheritdoc />
    public partial class FinalSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ__Teachers__1788CCADD8C65CBA",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "UQ__Students__1788CCAD10129B91",
                table: "Students");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Teachers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Teachers__1788CCADD8C65CBA",
                table: "Teachers",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Students__1788CCAD10129B91",
                table: "Students",
                column: "UserID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ__Teachers__1788CCADD8C65CBA",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "UQ__Students__1788CCAD10129B91",
                table: "Students");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Teachers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Students",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "UQ__Teachers__1788CCADD8C65CBA",
                table: "Teachers",
                column: "UserID",
                unique: true,
                filter: "[UserID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Students__1788CCAD10129B91",
                table: "Students",
                column: "UserID",
                unique: true,
                filter: "[UserID] IS NOT NULL");
        }
    }
}
