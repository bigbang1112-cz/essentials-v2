using BigBang1112.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using System.Net;
using System.Reflection;

namespace BigBang1112;

public static class App
{
    public static void Services(IServiceCollection services, AppOptions options, IConfiguration configuration)
    {
        if (options.Assembly is null)
        {
            throw new Exception("Assembly is null");
        }

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var v = options.ApiVersion ?? "v1";

            c.SwaggerDoc(v, new OpenApiInfo { Title = $"{options.Title} API", Version = v });
            
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{options.Assembly.GetName().Name}.xml"));

            c.CustomSchemaIds(type => type.GetCustomAttribute<SwaggerModelNameAttribute>()?.Name ?? type.Name);
        });

        // Figures out HTTPS behind proxies
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            foreach (var knownProxy in configuration.GetSection("KnownProxies").Get<string[]>() ?? [])
            {
                if (IPAddress.TryParse(knownProxy, out var ipAddress))
                {
                    options.KnownProxies.Add(ipAddress);
                    continue;
                }

                foreach (var hostIpAddress in Dns.GetHostAddresses(knownProxy))
                {
                    options.KnownProxies.Add(hostIpAddress);
                }
            }
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
