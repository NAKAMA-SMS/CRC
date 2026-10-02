using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CRC.Foundation.Tests;

// Disposable migration classes belong only to the test assembly, never a publish artifact.
internal sealed class UpgradeFixtureContext(DbContextOptions<UpgradeFixtureContext> options) : DbContext(options);

[DbContext(typeof(UpgradeFixtureContext))]
[Migration("20261002000000_Baseline")]
public sealed class FixtureBaseline : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) { }
    protected override void Down(MigrationBuilder migrationBuilder) { }
}

[DbContext(typeof(UpgradeFixtureContext))]
[Migration("20261002000001_FailedUpgrade")]
public sealed class FixtureFailedUpgrade : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE TABLE fixture_upgrade (id INTEGER PRIMARY KEY)");
        migrationBuilder.Sql("INSERT INTO fixture_does_not_exist VALUES (1)");
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("DROP TABLE fixture_upgrade");
}
