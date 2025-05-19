using Confluent.Kafka;

namespace events.Consumers
{
    public abstract class BaseConsumer
    {
        protected readonly IConsumer<string, string> _consumer;
        protected readonly ILogger _logger;
        protected readonly string _topic;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _consumerTask;

        protected BaseConsumer(string bootstrapServers, string topic, ILoggerFactory loggerFactory)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = $"{topic}-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _topic = topic;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public void StartConsuming()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _consumerTask = Task.Run(() => Consume(_cancellationTokenSource.Token));
        }

        public void StopConsuming()
        {
            _cancellationTokenSource?.Cancel();
            _consumerTask?.Wait();
            _consumer.Close();
        }

        private async Task Consume(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(cancellationToken);
                        if (consumeResult != null)
                        {
                            await ProcessMessage(consumeResult.Message.Value);
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Error consuming from {_topic}: {e.Error.Reason}");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Unexpected error: {e.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer stopped");
            }
            finally
            {
                _consumer.Close();
            }
        }

        protected abstract Task ProcessMessage(string message);
    }
}
