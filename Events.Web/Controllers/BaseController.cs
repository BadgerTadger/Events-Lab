namespace Events.Web.Controllers
{
    using Events.Data;
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();
    }
}