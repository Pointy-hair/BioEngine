﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BioEngine.Common.Migrations
{
    public partial class pid_can_be_null_in_FileCat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "pid",
                table: "be_files_cats",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.Sql("UPDATE `be_files_cats` SET `pid`=null WHERE `pid` = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_be_files_cats_be_files_cats_pid",
                table: "be_files_cats",
                column: "pid",
                principalTable: "be_files_cats",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_be_files_cats_be_files_cats_pid",
                table: "be_files_cats");

            migrationBuilder.AlterColumn<int>(
                name: "pid",
                table: "be_files_cats",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_be_files_cats_be_files_cats_pid",
                table: "be_files_cats",
                column: "pid",
                principalTable: "be_files_cats",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}