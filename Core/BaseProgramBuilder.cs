using AutoMapper;
using DotNetEnv;
using EliseuBatista99.Core.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace EliseuBatista99.Core
{
    public class BaseProgramBuilder
    {
        protected virtual bool UseAuthorization { get; } = false;
        protected virtual bool UseAuthentication { get; } = false;

        protected virtual string[] corsOrigins { get; } = new[]
        {
            "http://localhost:3000",
            "https://localhost:3000",
            "http://127.0.0.1:3000",
            "http://client:3000",
            "https://client:3000"
        };

        protected WebApplicationBuilder? Builder;

        protected WebApplication? App;

        protected virtual void InjectDependencies()
        {
            if (Builder != null)
            {
                Builder.Services.AddScoped<IExecutionContext, ExecutionContext>();
            }

        }

        protected virtual Profile[] GetMapperProfiles()
        {
            return [];
        }

        protected virtual void ConfigureDatabase()
        {

        }

        protected virtual void SeedDatabase()
        {

        }

        protected virtual void ConfigureControllers()
        {
            if (Builder != null)
            {
                Builder!.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Enums as string instead of int
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            }
        }

        protected virtual void ConfigureSwagger()
        {
            if (Builder != null)
            {
                Builder!.Services.AddOpenApi();
                Builder!.Services.AddEndpointsApiExplorer();
                Builder!.Services.AddSwaggerGen(c =>
                {
                    c.SchemaFilter<EnumSchemaFilter>();
                    c.DocumentFilter<ForceAllDtosDocumentFilter>();
                    c.EnableAnnotations();
                });
            }
        }

        public (WebApplicationBuilder builder, WebApplication app) Build(string[] args)
        {

            Builder = WebApplication.CreateBuilder(args);

            Env.Load();
            Builder.Configuration.AddEnvironmentVariables();

            // Kestrel uses env vars by default
            Builder.WebHost.ConfigureKestrel(options =>
            {
            });

            if (UseAuthentication)
            {
                var jwt = Builder.Configuration
                    .GetSection("Jwt")
                    .Get<JwtSettings>();

                if (jwt == null)
                {
                    throw new InvalidConfigurationException("Jwt missing in appsettings");
                }

                Builder.Services
                    .AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwt.Issuer,
                            ValidAudience = jwt.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwt.Key))
                        };
                    });
            }

            // Configuração do CORS
            Builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .WithOrigins(corsOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            Builder.Services.AddAutoMapper(cfg =>
            {
                var mapperProfiles = GetMapperProfiles();

                foreach (var profile in mapperProfiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            ConfigureDatabase();

            InjectDependencies();

            ConfigureControllers();

            ConfigureSwagger();

            App = Builder.Build();

            // Pipeline de desenvolvimento
            if (App.Environment.IsDevelopment())
            {
                App.MapOpenApi();
                App.UseSwagger();
                App.UseSwaggerUI();
                App.UseHttpsRedirection();
            }

            App.UseCors("CorsPolicy");

            if (UseAuthorization)
            {
                App.UseAuthorization();
            }

            App.MapControllers();

            return (Builder, App);
        }

        public void Run()
        {
            SeedDatabase();

            App!.Run();
        }

    }
}
