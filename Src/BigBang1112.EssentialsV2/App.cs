using BigBang1112.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BigBang1112;

public static class App
{
    public static void Services(IServiceCollection services, AppOptions options)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{options.Title} API", Version = "v1" });

            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{services.GetType().Assembly.GetName().Name}.xml"));

            c.CustomSchemaIds(type => type.GetCustomAttribute<SwaggerModelNameAttribute>()?.Name ?? type.Name);
        });
    }

    public static void Middleware(IApplicationBuilder app, AppOptions options)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "swagger/{documentname}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{options.Title} API v1");
            c.InjectStylesheet("../_content/BigBang1112.EssentialsV2.Razor/css/SwaggerDark.css");
        });

        app.UseReDoc(c =>
        {

        });
    }
}
