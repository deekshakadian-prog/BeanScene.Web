using Microsoft.AspNetCore.Mvc;

namespace BeanScene.Web.Controllers
{
    public class ChatController : Controller
    {
        // GET: /Chat
        public IActionResult Index()
        {
            return View();
        }
    }
}
