using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CRC.Persistence.Sqlite.Migrations;

[DbContext(typeof(LocalContext))]
[Migration("20261002000000_Baseline")]
public sealed class Baseline : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) { }
    protected override void Down(MigrationBuilder migrationBuilder) { }
}
