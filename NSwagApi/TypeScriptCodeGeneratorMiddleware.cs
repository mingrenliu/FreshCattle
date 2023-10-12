using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag.AspNetCore;
using NSwag.Generation.AspNetCore;
using System.Net.Mime;

namespace NSwagApi;

internal class TypeScriptCodeGeneratorMiddleware
{
    private readonly RequestDelegate _nextDelegate;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _documentName;

    public TypeScriptCodeGeneratorMiddleware(RequestDelegate nextDelegate, IServiceProvider serviceProvider, Func<string> documentName)
    {
        _nextDelegate = nextDelegate;
        _serviceProvider = serviceProvider;
        _documentName = documentName();
    }

    public async Task Invoke(HttpContext context)
    {
        var name = _documentName ?? _serviceProvider.GetRequiredService<IHostEnvironment>().ApplicationName.Split(new[] { '.', '-', '_' }).Last();
        if (!context.Request.Path.HasValue || !string.Equals(context.Request.Path.Value!.Trim('/'), $"{name}/tsCode", StringComparison.OrdinalIgnoreCase))
        {
            await _nextDelegate(context);
            return;
        }

        var setting = _serviceProvider.GetService<OpenApiDocumentRegistration>()?.Settings;
        context.Response.StatusCode = 200;
        var docGenerator = new AspNetCoreOpenApiDocumentGenerator(setting ?? new AspNetCoreOpenApiDocumentGeneratorSettings());
        var doc = await docGenerator.GenerateAsync(_serviceProvider);
        string text = TypeScriptCodeGenerator.GenerateFile(doc);
        context.Response.Headers["Content-Type"] = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(text);
    }
}