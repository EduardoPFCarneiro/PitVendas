using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PitVendasApi.Migrations
{
    /// <inheritdoc />
    public partial class AlinhamentoComPrototipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "data_faturamento",
                table: "pedidos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "percentual_desconto",
                table: "pedidos",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "tabela_preco",
                table: "pedidos",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 6,
                column: "ordem",
                value: 7);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "codigo", "ordem" },
                values: new object[] { "XXG", 8 });

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 8,
                column: "ordem",
                value: 10);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 9,
                column: "ordem",
                value: 11);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 10,
                column: "ordem",
                value: 12);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 11,
                column: "ordem",
                value: 13);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 12,
                column: "ordem",
                value: 14);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 13,
                column: "ordem",
                value: 15);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 14,
                column: "ordem",
                value: 16);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 15,
                column: "ordem",
                value: 17);

            migrationBuilder.InsertData(
                table: "tamanhos",
                columns: new[] { "id", "codigo", "ordem" },
                values: new object[,]
                {
                    { 16, "EG", 6 },
                    { 17, "3XG", 9 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DropColumn(
                name: "data_faturamento",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "percentual_desconto",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "tabela_preco",
                table: "pedidos");

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 6,
                column: "ordem",
                value: 6);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "codigo", "ordem" },
                values: new object[] { "XGG", 7 });

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 8,
                column: "ordem",
                value: 8);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 9,
                column: "ordem",
                value: 9);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 10,
                column: "ordem",
                value: 10);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 11,
                column: "ordem",
                value: 11);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 12,
                column: "ordem",
                value: 12);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 13,
                column: "ordem",
                value: 13);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 14,
                column: "ordem",
                value: 14);

            migrationBuilder.UpdateData(
                table: "tamanhos",
                keyColumn: "id",
                keyValue: 15,
                column: "ordem",
                value: 15);
        }
    }
}
