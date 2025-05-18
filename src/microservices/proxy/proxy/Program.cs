using Microsoft.AspNetCore.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8000);
});

var app = builder.Build();

var gradualMigration = bool.Parse(Environment.GetEnvironmentVariable("GRADUAL_MIGRATION") ?? "false");
var monolithUrl = Environment.GetEnvironmentVariable("MONOLITH_URL") ?? throw new ArgumentNullException("MONOLITH_URL is required");
var moviesServiceUrl = Environment.GetEnvironmentVariable("MOVIES_SERVICE_URL") ?? throw new ArgumentNullException("MOVIES_SERVICE_URL is required");
var migrationPercent = int.Parse(Environment.GetEnvironmentVariable("MOVIES_MIGRATION_PERCENT") ?? "0");

if (migrationPercent is < 0 or > 100)
{
    throw new ArgumentException("MOVIES_MIGRATION_PERCENT must be between 0 and 100");
}

var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

app.Use(async (HttpContext context, Func<Task> next) =>
{
    var useNewService = true;

    if (gradualMigration)
    {
        var random = new Random();
        var randomPercent = random.Next(0, 100);

        useNewService = randomPercent < migrationPercent;
    }

    var targetUri = new Uri(useNewService ? moviesServiceUrl : monolithUrl);

    var request = context.Request;
    var uri = new UriBuilder(request.GetEncodedUrl())
    {
        Scheme = targetUri.Scheme,
        Host = targetUri.Host,
        Port = targetUri.Port
    }.Uri;

    var requestMessage = new HttpRequestMessage();
    requestMessage.RequestUri = uri;
    requestMessage.Method = new HttpMethod(request.Method);

    foreach (var header in request.Headers)
    {
        requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
    }

    if (!HttpMethods.IsGet(request.Method) &&
        !HttpMethods.IsHead(request.Method) &&
        !HttpMethods.IsDelete(request.Method) &&
        !HttpMethods.IsTrace(request.Method))
    {
        requestMessage.Content = new StreamContent(request.Body);

        foreach (var header in request.Headers)
        {
            requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    var responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);

    context.Response.StatusCode = (int)responseMessage.StatusCode;

    foreach (var header in responseMessage.Headers)
    {
        context.Response.Headers.TryAdd(header.Key, header.Value.ToArray());
    }

    foreach (var header in responseMessage.Content.Headers)
    {
        context.Response.Headers.TryAdd(header.Key, header.Value.ToArray());
    }

    await responseMessage.Content.CopyToAsync(context.Response.Body);
});

app.Run();