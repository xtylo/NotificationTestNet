using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class NotificationLogsController : Controller
    {
        private readonly INotificationLogService _service;
        public NotificationLogsController(INotificationLogService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _service.GetAllAsync();

            return Ok(logs);
        }
    }
}
