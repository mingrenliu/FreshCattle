using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using NSwag.Generation.AspNetCore;

namespace NSwagApi;

public static class NSwagExtensions
{
    public static IServiceCollection AddNSwag(this IServiceCollection services, Action<AspNetCoreOpenApiDocumentGeneratorSettings> configure)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(configure);
        return services;
    }

    public static IServiceCollection AddNSwag(this IServiceCollection services, Action<AspNetCoreOpenApiDocumentGeneratorSettings, IServiceProvider> configure = null)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(configure);
        return services;
    }

    public static IApplicationBuilder UseNSwag(this IApplicationBuilder app, Action<OpenApiDocumentMiddlewareSettings> documentConfigure = null, Action<SwaggerUi3Settings> swaggerUi3Configure = null, string typeScriptDocumentName = null)
    {
        app.UseOpenApi(documentConfigure);
        app.UseSwaggerUi3(swaggerUi3Configure);
        Func<string> f = () => typeScriptDocumentName;
        app.UseMiddleware<TypeScriptCodeGeneratorMiddleware>(new object[] { f });
        return app;
    }
}