using events.Models;
using events.Services;
using Microsoft.AspNetCore.Mvc;

namespace events.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventProducer _eventProducer;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventProducer eventProducer, ILogger<EventsController> logger)
        {
            _eventProducer = eventProducer;
            _logger = logger;
        }

        [HttpPost("movie")]
        public async Task<IActionResult> CreateMovieEvent([FromBody] MovieEventDto movieEventDto)
        {
            try
            {
                var eventData = new Event
                {
                    Id = $"movie-{movieEventDto.MovieId}-{movieEventDto.Action}",
                    Type = "movie",
                    Timestamp = DateTime.UtcNow,
                    Payload = movieEventDto
                };

                var result = await _eventProducer.ProduceAsync("movie-events", eventData);

                return CreatedAtAction(nameof(CreateMovieEvent), new EventResponse
                {
                    Status = "success",
                    Partition = result.Partition,
                    Offset = result.Offset.Value,
                    Event = eventData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie event");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpPost("user")]
        public async Task<IActionResult> CreateUserEvent([FromBody] UserEventDto userEventDto)
        {
            try
            {
                var eventData = new Event
                {
                    Id = $"user-{userEventDto.UserId}-{userEventDto.Action}",
                    Type = "user",
                    Timestamp = userEventDto.Timestamp,
                    Payload = userEventDto
                };

                var result = await _eventProducer.ProduceAsync("user-events", eventData);

                return CreatedAtAction(nameof(CreateUserEvent), new EventResponse
                {
                    Status = "success",
                    Partition = result.Partition,
                    Offset = result.Offset.Value,
                    Event = eventData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user event");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpPost("payment")]
        public async Task<IActionResult> CreatePaymentEvent([FromBody] PaymentEventDto paymentEventDto)
        {
            try
            {
                var eventData = new Event
                {
                    Id = $"payment-{paymentEventDto.PaymentId}",
                    Type = "payment",
                    Timestamp = paymentEventDto.Timestamp,
                    Payload = paymentEventDto
                };

                var result = await _eventProducer.ProduceAsync("payment-events", eventData);

                return CreatedAtAction(nameof(CreatePaymentEvent), new EventResponse
                {
                    Status = "success",
                    Partition = result.Partition,
                    Offset = result.Offset.Value,
                    Event = eventData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment event");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }
    }
}