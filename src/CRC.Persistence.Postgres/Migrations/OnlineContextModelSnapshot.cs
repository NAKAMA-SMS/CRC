using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CRC.Persistence.Postgres.Migrations;

[DbContext(typeof(OnlineContext))]
public sealed class OnlineContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder) => modelBuilder.HasAnnotation("ProductVersion", "10.0.12");
}
