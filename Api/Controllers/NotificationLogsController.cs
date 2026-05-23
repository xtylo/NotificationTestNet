using Application.Abstractions;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationLogsController : Controller
    {
        private readonly INotificationLogService _service;
        public NotificationLogsController(INotificationLogService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<List<NotificationLogDto>> GetAll()
        {
            return await _service.GetAllAsync();

        }
    }
}
