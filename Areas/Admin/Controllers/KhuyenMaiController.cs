using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using PagedList;
using Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Models;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Controllers
{
    public class KhuyenMaiController : Controller
    {
        // GET: Admin/KhuyenMai
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();
        public ActionResult QuanTriKhuyenMai(int page = 1, int pageSize = 5)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var dsKM = db.KHUYENMAIs.OrderByDescending(x => x.TiLe).ToPagedList(page, pageSize);
                    var model = new QuanLyThongTin
                    {
                        PLKhuyenMai = dsKM
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        [HttpPost]
        public ActionResult SuaKhuyenMai(int maKM, FormCollection f)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    KHUYENMAI p = db.KHUYENMAIs.Single(ma => ma.MaKhuyenMai == maKM);
                    var tenKM = f["tenKM"].Trim();
                    var tgBD = f["tgBD"];
                    var tgKT = f["tgKT"];
                    var tile = f["tile"];
                    if (!String.IsNullOrEmpty(tenKM))
                    {
                        p.TenChuongTrinhKM = tenKM;
                        p.TGBatDau = DateTime.Parse(tgBD);
                        p.TGKetThuc = DateTime.Parse(tgKT);
                        p.TiLe = decimal.Parse(tile);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Sửa khuyến mãi thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriKhuyenMai", "KhuyenMai");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        [HttpPost]
        public ActionResult ThemKhuyenMai(FormCollection f)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    KHUYENMAI p = new KHUYENMAI();
                    var tenKM = f["tenKM"].Trim();
                    var tgBD = f["tgBD"];
                    var tgKT = f["tgKT"];
                    var tile = f["tile"];
                    if (!String.IsNullOrEmpty(tenKM))
                    {
                        p.TenChuongTrinhKM = tenKM;
                        p.TGBatDau = DateTime.Parse(tgBD);
                        p.TGKetThuc = DateTime.Parse(tgKT);
                        p.TiLe = decimal.Parse(tile);
                        db.KHUYENMAIs.InsertOnSubmit(p);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Thêm khuyến mãi mới thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriKhuyenMai", "KhuyenMai");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult SearchKhuyenMai(string searchKM, int page = 1, int pageSize = 5)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    if (string.IsNullOrEmpty(searchKM))
                    {
                        return RedirectToAction("QuanTriKhuyenMai", "KhuyenMai");
                    }
                    var dsKMS = db.KHUYENMAIs.Where(x => x.TenChuongTrinhKM.Contains(searchKM)).OrderByDescending(x => x.TiLe).ToPagedList(page, pageSize);
                    var models = new QuanLyThongTin
                    {
                        PLKhuyenMai = dsKMS
                    };
                    ViewBag.Search = searchKM;
                    return View(models);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult ThietLapKhuyenMai(int maLoai, int page = 1, int pageSize = 5)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    List<KHUYENMAI> dsKM = db.KHUYENMAIs.OrderByDescending(x => x.TiLe).ToList();
                    List<LOAISANPHAM> dsLSP = db.LOAISANPHAMs.OrderBy(x => x.MaDanhMuc).ToList();
                    IPagedList<SANPHAM> dsSP = null;
                    if (maLoai == 0)
                    {
                        dsSP = db.SANPHAMs.OrderByDescending(y => y.SoLuongTon).ToPagedList(page, pageSize);
                    }
                    else
                    {
                        dsSP = db.SANPHAMs.Where(x => x.MaLoaiSP == maLoai).OrderByDescending(y => y.SoLuongTon).ToPagedList(page, pageSize);
                    }
                    var model = new QuanLyThongTin
                    {
                        PLSanPham = dsSP,
                        dsKhuyenMai = dsKM,
                        dsLoaiSanPham = dsLSP
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }
    }
}