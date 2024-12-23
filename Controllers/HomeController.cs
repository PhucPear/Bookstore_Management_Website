using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Models;

namespace Website_QuanLyNhaSachNguyenVanCu.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();

        private List<SANPHAM> LaySPTheoDanhMuc(int maDanhMuc)
        {
            return db.SANPHAMs.Where(s => s.LOAISANPHAM.MaDanhMuc == maDanhMuc).OrderByDescending(x => x.MaSanPham).Take(12).ToList();
        }

        public ActionResult TrangChu()
        {
            var model = new DanhSachTheoSanPham
            {
                dsSach = LaySPTheoDanhMuc(1),
                dsVanPhongPham = LaySPTheoDanhMuc(2),
                dsQuaLuuNiem = LaySPTheoDanhMuc(3)
            };
            return View(model);
        }

        public ActionResult DanhMucPartial()
        {
            var dsDM = db.DANHMUCs.OrderBy(x => x.MaDanhMuc).ToList();
            var dsLSP = db.LOAISANPHAMs.OrderBy(y => y.MaDanhMuc).ToList();

            var model = new DanhSachTheoSanPham
            {
                dsDanhMuc = dsDM,
                dsLoaiSanPham = dsLSP
            };
            return PartialView(model);
        }

        public ActionResult KhachHangPartial()
        {
            return PartialView();
        }

        public ActionResult GioHangPartial()
        {
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                int cart = db.GIOHANGs.Where(x => x.MaKhachHang == k.MaKhachHang).ToList().Count;
                ViewBag.SoLuongSP = cart;
            }
            return PartialView();
        }



    }
}