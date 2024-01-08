using System.Text.Json.Serialization;
using AIDB.Core.IRepositories;
using AIDB.Core.Services;
using AIDB.Infrastructure;
using AIDB.Infrastructure.Repositories;
using AIDB.Infrastructure.Services;
using Microsoft.OpenApi.Models;
using OpenAI;
using Serilog;

namespace AIDB.App.Configuration
{
    public static class DIConfigurator
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            // Core services
            services.AddControllers()
                    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpContextAccessor();
            services.AddHttpLogging(o => o = new Microsoft.AspNetCore.HttpLogging.HttpLoggingOptions());

            services.AddCors(cors =>
            {
                cors.AddDefaultPolicy(options =>
                {
                    options.AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                });
            });

            return services;
        }

        public static IServiceCollection AddBaseServices(this IServiceCollection services, IConfiguration config)
        {
            // Services
            services.AddScoped<IStaffManagerService, StaffManagerService>();
            services.AddScoped<IPersonManagerService, PersonManagerService>();
            services.AddScoped<IDynamicAiQueryExecutorService, DynamicAiQueryExecutorService>();

            // Infra services
            services.AddScoped<IRawSqlExecutor, RawSqlExecutor>();

            // Repositories
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IGradeRepository, GradeRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ITitleRepository, TitleRepository>();
            services.AddScoped<IAiDbCommandRepository, AiDbCommandsRepository>();

            services.AddScoped<OpenAIClient>((provider) =>
            {
                var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient("GPTClient");

                string? openAIKey = config["AiToken"];
                if (string.IsNullOrEmpty(openAIKey))
                {
                    throw new ArgumentNullException(nameof(openAIKey), "OpenAIKey is null or empty");
                }

                return new OpenAIClient(openAIKey, client: httpClient);
            });

            return services;
        }

        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            // Used for OpenAI API
            services.AddHttpClient("GPTClient", c => { });

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            string? connString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connString))
            {
                connString = Path.Combine(AppContext.BaseDirectory, "AIDB.db");
                Log.Warning("Connection string is null or empty, using default connection string");
            }

            services.AddAIDBContext(connString);

            return services;
        }
    }
}