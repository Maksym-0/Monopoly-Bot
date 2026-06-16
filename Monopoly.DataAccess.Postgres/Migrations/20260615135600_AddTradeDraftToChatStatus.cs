using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonopolyBot.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddTradeDraftToChatStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "TradeGiveCells",
                table: "ChatStatuses",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TradeGiveMoney",
                table: "ChatStatuses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TradeOffereeId",
                table: "ChatStatuses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradeOffereeName",
                table: "ChatStatuses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<int>>(
                name: "TradeWantedCells",
                table: "ChatStatuses",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TradeWantedMoney",
                table: "ChatStatuses",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradeGiveCells",
                table: "ChatStatuses");

            migrationBuilder.DropColumn(
                name: "TradeGiveMoney",
                table: "ChatStatuses");

            migrationBuilder.DropColumn(
                name: "TradeOffereeId",
                table: "ChatStatuses");

            migrationBuilder.DropColumn(
                name: "TradeOffereeName",
                table: "ChatStatuses");

            migrationBuilder.DropColumn(
                name: "TradeWantedCells",
                table: "ChatStatuses");

            migrationBuilder.DropColumn(
                name: "TradeWantedMoney",
                table: "ChatStatuses");
        }
    }
}
