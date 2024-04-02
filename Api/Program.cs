
using Api.Data;
using Api.Data.Seeding;
using Api.Extensions;
using Api.Helpers;
using Api.Middlewares;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api
{
    public class Program
    {
        public  static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            // Add singletone for db connection 
            builder.Services.AddDbContext<DatingContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddAuthenticationServices(builder.Configuration);



            #endregion

            var app = builder.Build();

            #region Update Database

            //DatingContext dbContext = new DatingContext();
            //await dbContext.Database.MigrateAsync();


            using var scope = app.Services.CreateScope();     //Group of services where LifeTime is scoped
            var services = scope.ServiceProvider;            //services itself

            var LoggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                //Ask CLR To Create Object From DatingContext Explicitly
                var DbContext = services.GetRequiredService<DatingContext>();
                //var IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();
                //var UserManager = services.GetRequiredService<UserManager<User>>();

                await DbContext.Database.MigrateAsync();
                //await IdentityDbContext.Database.MigrateAsync();

                await UserSeeding.SeedAsync(DbContext, LoggerFactory);
                //await AppIdentityDbContextSeeding.SeedUserAsync(UserManager);

            }
            catch (Exception ex)
            {
                var logger = LoggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error Occurred While Updating Database");
            }

            #endregion

            #region  Configure the HTTP request pipeline.


            //Exception Middleware
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseRouting();
            app.UseStaticFiles();

            app.UseHttpsRedirection();    // convert any protocol to https protocol for more security
            
            app.UseCors("MyCors");
            
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            #endregion


            app.Run();
        }
    }
}
