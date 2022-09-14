using LearningWebSite.Core.Services.BotService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace LearningWebSite.Areas.Admin.Controllers
{
    [ApiController]
    public class TelgramBotController : ControllerBase
    {
        [HttpPost]
        [Route("bot")]
        public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService,
                                         [FromBody] Update update)
        {
            await handleUpdateService.EchoAsync(update);
            return Ok();
        }
    }
}
