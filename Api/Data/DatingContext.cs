using Api.Data.Entities;
using Api.Data.TypesConventions;
using Microsoft.EntityFrameworkCore;


namespace Api.Data
{
    public class DatingContext:DbContext
    {
        public DatingContext(DbContextOptions<DatingContext> options):base(options)
        {
            
        }
        public DbSet<AppUser> Users { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            base.ConfigureConventions(builder);
            builder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>();
        }
    }
}
