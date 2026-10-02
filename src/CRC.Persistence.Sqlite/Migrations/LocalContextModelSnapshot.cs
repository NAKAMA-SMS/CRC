using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CRC.Persistence.Sqlite.Migrations;

[DbContext(typeof(LocalContext))]
public sealed class LocalContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder) => modelBuilder.HasAnnotation("ProductVersion", "10.0.12");
}
