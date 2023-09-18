using Microsoft.AspNetCore.Mvc;

namespace NoteKeeper.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IActionResult Index()
        {
            return Content("api started successfully");
        }
    }
}
