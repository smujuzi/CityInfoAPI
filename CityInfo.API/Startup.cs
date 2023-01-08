using Amazon.DynamoDBv2;
using CityInfo.API.Repositories;
using CityInfo.API.Services;
using CityInfo.API.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace CityInfo.API
{
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) 
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSingleton<FileExtensionContentTypeProvider>();

            #if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
            #else
            services.AddTransient<IMailService,CloudMailService>();
            #endif

            services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.KeyName));
            services.AddSingleton<CitiesDataStore>();
            services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1));
            services.AddSingleton<ICityRepository, CityRepository>();

            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Authentication:Issuer"],
                        ValidAudience = Configuration["Authentication:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(Configuration["Authentication:SecretForKey"]))
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MustBeFromAntwerp", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("city", "Antwerp");
                });
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            //Write code here to configure the request processing pipeline
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                }
            );

        }
    }
}

