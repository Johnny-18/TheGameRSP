using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RSPGame.Models.JsonConverter;
using RSPGame.Models.OptionsModel;
using RSPGame.Services;
using RSPGame.Services.Authentication;
using RSPGame.Services.FileWorker;
using RSPGame.Services.Room;
using RSPGame.Services.Rsp;
using RSPGame.Storage;
using Serilog;

namespace RSPGame
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new TimespanConverter());
            });

            services.AddSwaggerGen();

            services.Configure<FilesOptions>(Configuration.GetSection(FilesOptions.Files));
            services.Configure<AuthKeyOptions>(Configuration.GetSection(AuthKeyOptions.AuthKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration
                            .GetSection(AuthKeyOptions.AuthKey).Get<AuthKeyOptions>().SecretKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IFileWorker, FileWorker>();
            services.AddSingleton<IRspStorage, RspStorage>();

            services.AddTransient<IRspService, RspService>();
            services.AddTransient<PasswordHashGenerator>();
            services.AddTransient<SinglePlayerService>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddSingleton<RoomStorage>();
            services.AddSingleton<GameStorage>();
            services.AddSingleton<RoundStorage>();
            services.AddSingleton<IRoomService, RoomService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}