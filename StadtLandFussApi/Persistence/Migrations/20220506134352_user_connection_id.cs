using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StadtLandFussApi.Persistence.Migrations
{
    public partial class user_connection_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "connection_id",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "lobbies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "connection_id",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "lobbies",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
