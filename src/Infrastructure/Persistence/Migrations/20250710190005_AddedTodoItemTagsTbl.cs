using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo_App.Infrastructure.Persistence.Migrations
{
    public partial class AddedTodoItemTagsTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItemTag_TodoItems_ItemId",
                table: "TodoItemTag");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItemTag_TodoTags_TagId",
                table: "TodoItemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoItemTag",
                table: "TodoItemTag");

            migrationBuilder.RenameTable(
                name: "TodoItemTag",
                newName: "TodoItemTags");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItemTag_TagId",
                table: "TodoItemTags",
                newName: "IX_TodoItemTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoItemTags",
                table: "TodoItemTags",
                columns: new[] { "ItemId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItemTags_TodoItems_ItemId",
                table: "TodoItemTags",
                column: "ItemId",
                principalTable: "TodoItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItemTags_TodoTags_TagId",
                table: "TodoItemTags",
                column: "TagId",
                principalTable: "TodoTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItemTags_TodoItems_ItemId",
                table: "TodoItemTags");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItemTags_TodoTags_TagId",
                table: "TodoItemTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoItemTags",
                table: "TodoItemTags");

            migrationBuilder.RenameTable(
                name: "TodoItemTags",
                newName: "TodoItemTag");

            migrationBuilder.RenameIndex(
                name: "IX_TodoItemTags_TagId",
                table: "TodoItemTag",
                newName: "IX_TodoItemTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoItemTag",
                table: "TodoItemTag",
                columns: new[] { "ItemId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItemTag_TodoItems_ItemId",
                table: "TodoItemTag",
                column: "ItemId",
                principalTable: "TodoItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItemTag_TodoTags_TagId",
                table: "TodoItemTag",
                column: "TagId",
                principalTable: "TodoTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
