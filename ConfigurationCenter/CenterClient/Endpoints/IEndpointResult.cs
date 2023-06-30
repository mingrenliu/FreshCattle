namespace CenterClient.Endpoint.Endpoints;

public interface IEndpointResult
{
    /// <summary>
    /// Executes the result.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns></returns>
    Task ExecuteAsync(HttpContext context);
}