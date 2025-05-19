using events.Consumers;
using events.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8082);
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var kafkaBrokers = Environment.GetEnvironmentVariable("KAFKA_BROKERS");
builder.Services.AddSingleton<IEventProducer>(provider =>
    new KafkaEventProducer(kafkaBrokers, provider.GetRequiredService<ILogger<KafkaEventProducer>>()));

var app = builder.Build();

app.MapControllers();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

var movieConsumer = new MovieEventConsumer(kafkaBrokers, "movie-events", loggerFactory);
var userConsumer = new UserEventConsumer(kafkaBrokers, "user-events", loggerFactory);
var paymentConsumer = new PaymentEventConsumer(kafkaBrokers, "payment-events", loggerFactory);

var hostApplicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

hostApplicationLifetime.ApplicationStarted.Register(() =>
{
    movieConsumer.StartConsuming();
    userConsumer.StartConsuming();
    paymentConsumer.StartConsuming();
});

hostApplicationLifetime.ApplicationStopping.Register(() =>
{
    movieConsumer.StopConsuming();
    userConsumer.StopConsuming();
    paymentConsumer.StopConsuming();
});

app.Run();