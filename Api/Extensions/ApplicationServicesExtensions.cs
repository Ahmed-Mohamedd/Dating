using Api.Data;
using Api.Helpers;
using Api.Interfaces;
using Api.Services;
using System.Runtime.CompilerServices;

namespace Api.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(typeof(MappingProfile));
            services.Configure<JWT>(Configuration.GetSection("JWT")); //Add configuration For JWTSetting Class
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));


            // Allowing Cors
            services.AddCors(options =>
            {
                options.AddPolicy("MyCors", AppCors =>
                {
                    AppCors.AllowAnyHeader();
                    AppCors.AllowAnyMethod();
                    AppCors.WithOrigins("https://localhost:4200");
                });
            });

            return services;
        }
    }
}
