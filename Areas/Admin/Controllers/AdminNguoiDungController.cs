using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Models;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using PagedList;
using Website_QuanLyNhaSachNguyenVanCu.Controllers;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Controllers
{
    public class AdminNguoiDungController : Controller
    {
        // GET: Admin/NguoiDung
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();
        BaseController pub = new BaseController();

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(string email, string matKhau)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(matKhau))
            {
                NHANVIEN nv = db.NHANVIENs.SingleOrDefault(x => x.Email.Equals(email));
                if (nv != null)
                {
                    string hash = pub.MaHoaHashPassword(matKhau, nv.Salt);
                    if (nv.TrangThai.Equals("LOCK"))
                    {
                        return Json(new { success = false, message = "Tài khoản của bạn đã bị khoá!!" });
                    }
                    if (hash == nv.MatKhau && nv.Quyen.Equals("ADMIN"))
                    {
                        Session["TaiKhoan"] = nv;
                        return Json(new { success = true, message = "/Admin/AdminHome/QuanTri" });
                    }
                    else if (hash == nv.MatKhau && nv.Quyen.Equals("NV"))
                    {
                        Session["TaiKhoan"] = nv;
                        return Json(new { success = true, message = "/Admin/AdminHome/NhanVien" });
                    }
                }
                else
                    return Json(new { success = false, message = "Email hoặc mật khẩu không chính xác!!" });
            }
            return Json(new { success = false, message = "Email hoặc mật khẩu không chính xác!!" });
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult QuanTriKhachHang(int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var dsKH = db.KHACHHANGs.OrderByDescending(x => x.MaKhachHang).ToPagedList(page, pageSize);
                    var model = new QuanLyThongTin
                    {
                        PLKhachHang = dsKH
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }


        public ActionResult KhoaTKKhachHang(int maKhachHang)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    KHACHHANG k = db.KHACHHANGs.Single(ma => ma.MaKhachHang == maKhachHang);
                    k.TrangThai = "LOCK";
                    db.SubmitChanges();
                    TempData["ThongBao"] = "Khoá tài khoản của khách hàng thành công";
                    TempData["LoaiTB"] = "alert-success";
                    TempData["ht"] = "block";
                }
                return RedirectToAction("QuanTriKhachHang", "AdminNguoiDung");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult MoKhoaTKKhachHang(int maKhachHang)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    KHACHHANG k = db.KHACHHANGs.Single(ma => ma.MaKhachHang == maKhachHang);
                    k.TrangThai = "OPEN";
                    db.SubmitChanges();
                    TempData["ThongBao"] = "Mở khoá tài khoản cho khách hàng thành công";
                    TempData["LoaiTB"] = "alert-success";
                    TempData["ht"] = "block";
                }
                return RedirectToAction("QuanTriKhachHang", "AdminNguoiDung");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult SearchKhachHang(string searchKH, int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    if (string.IsNullOrEmpty(searchKH))
                    {
                        return RedirectToAction("QuanTriKhachHang", "AdminNguoiDung");
                    }
                    var dsKHS = db.KHACHHANGs.Where(x => x.TenKhachHang.Contains(searchKH)).OrderByDescending(x => x.MaKhachHang).ToPagedList(page, pageSize);
                    var models = new QuanLyThongTin
                    {
                        PLKhachHang = dsKHS
                    };
                    ViewBag.Search = searchKH;
                    return View(models);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }


        public ActionResult QuanTriNhanVien(int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var dsNV = db.NHANVIENs.Where(y => y.Quyen.Equals("NV")).OrderByDescending(x => x.MaNhanVien).ToPagedList(page, pageSize);
                    var model = new QuanLyThongTin
                    {
                        PLNhanVien = dsNV
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }


        [HttpPost]
        public ActionResult ThemNhanVien(FormCollection f)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var tenNV = f["tenNV"].Trim();
                    var ngaySinh = f["ngaySinh"];
                    var gioiTinh = f["gioiTinh"];
                    var soDT = f["soDT"];
                    var email = f["email"];
                    var diaChi = f["diaChi"];
                    var matKhau = f["matKhau"];
                    var quyen = f["quyen"];
                    if (!String.IsNullOrEmpty(tenNV) || !String.IsNullOrEmpty(diaChi) || !String.IsNullOrEmpty(matKhau))
                    {
                        NHANVIEN s = new NHANVIEN();
                        s.TenNhanVien = tenNV;
                        s.NgaySinh = DateTime.Parse(ngaySinh);
                        s.GioiTinh = gioiTinh;
                        s.SDT = soDT;
                        s.Email = email;
                        s.DiaChi = diaChi;
                        string salt = pub.KhoiTaoSaltChoUser();
                        s.Salt = salt;
                        string hash = pub.MaHoaHashPassword(matKhau, salt);
                        s.MatKhau = hash;
                        s.Quyen = quyen;
                        s.TrangThai = "OPEN";
                        db.NHANVIENs.InsertOnSubmit(s);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Thêm sản phẩm thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriNhanVien", "AdminNguoiDung");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }


        public ActionResult KhoaTKNhanVien(int maNhanVien)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    NHANVIEN s = db.NHANVIENs.Single(ma => ma.MaNhanVien == maNhanVien);
                    s.TrangThai = "LOCK";
                    db.SubmitChanges();
                    TempData["ThongBao"] = "Khoá tài khoản của nhân viên thành công";
                    TempData["LoaiTB"] = "alert-success";
                    TempData["ht"] = "block";
                }
                return RedirectToAction("QuanTriNhanVien", "AdminNguoiDung");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult MoKhoaTKNhanVien(int maNhanVien)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    NHANVIEN s = db.NHANVIENs.Single(ma => ma.MaNhanVien == maNhanVien);
                    s.TrangThai = "OPEN";
                    db.SubmitChanges();
                    TempData["ThongBao"] = "Khoá tài khoản của nhân viên thành công";
                    TempData["LoaiTB"] = "alert-success";
                    TempData["ht"] = "block";
                }
                return RedirectToAction("QuanTriNhanVien", "AdminNguoiDung");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult SearchNhanVien(string searchNV, int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    if (string.IsNullOrEmpty(searchNV))
                    {
                        return RedirectToAction("QuanTriNhanVien", "AdminNguoiDung");
                    }
                    var dsNVS = db.NHANVIENs.Where(x => x.TenNhanVien.Contains(searchNV)).OrderByDescending(x => x.MaNhanVien).ToPagedList(page, pageSize);
                    var models = new QuanLyThongTin
                    {
                        PLNhanVien = dsNVS
                    };
                    ViewBag.Search = searchNV;
                    return View(models);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

    }
}