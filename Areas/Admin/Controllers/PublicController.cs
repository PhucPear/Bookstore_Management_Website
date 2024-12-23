using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Models;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Controllers
{
    public class PublicController : Controller
    {
        // GET: Admin/Public
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();
        [HttpPost]
        public ActionResult GanMaChoSession(int ma)
        {
            if (Session["ma"] != null)
            {
                Session.Remove("ma");
            }
            Session["ma"] = ma;
            return Json(new { success = true, message = "Biến Session " + Session["ma"] + " đã được cập nhật là: " + ma });
        }      
    }
}