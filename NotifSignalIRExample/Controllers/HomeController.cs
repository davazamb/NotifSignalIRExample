using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotifSignalIRExample.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetNotificationContacts()
        {
            var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
            NotificatiorComponents NC = new NotificatiorComponents();
            var list = NC.GetContacts(notificationRegisterTime);
            //Realizamos un update de la session para agregar la notificaion
            Session["LastUpdated"] = DateTime.Now;
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}