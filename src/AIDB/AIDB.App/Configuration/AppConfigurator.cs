using AIDB.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

namespace AIDB.App.Configuration
{
    public static class AppConfigurator
    {
        public static WebApplication ConfigureAppBase(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
                app.Map("/", () => Results.Redirect("/swagger"));
            }

            app.UseHttpLogging();
            app.MapControllers();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.MapFallbackToFile("index.html");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            return app;
        }

        public static WebApplication UseAIDBSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AIDB API V1");
            });

            return app;
        }

        public static WebApplication Migrate(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AIDBMainContext>();

            context.Database.Migrate();

            return app;
        }
    }
}