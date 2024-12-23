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
    public class NhanVienController : Controller
    {
        // GET: Admin/NhanVien
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();
        public ActionResult NhanVien()
        {
            return View();
        }

        public ActionResult QuanLyDonHang(int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("NV"))
                {
                    var dsDH = db.DONHANGs.Where(y => y.TinhTrang.Equals("Đặt hàng thành công")).OrderByDescending(x => x.NgayDat).ToPagedList(page, pageSize);
                    var model = new QuanLyThongTin
                    {
                        PLDonHang = dsDH
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult ChiTietDonHang(int maDH)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("NV"))
                {
                    var dh = db.DONHANGs.Single(x => x.MaDonHang == maDH);
                    var ctdh = db.CHITIETDONHANGs.Where(x => x.MaDonHang == dh.MaDonHang).ToList();
                    var model = new QuanLyThongTin
                    {
                        donHang = dh,
                        dsCTDonHang = ctdh
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult SearchDonHang(string searchDH, int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("NV"))
                {
                    if (string.IsNullOrEmpty(searchDH))
                    {
                        return RedirectToAction("QuanLyDonHang", "NhanVien");
                    }
                    int maDH = 0;
                    bool laSoNguyen = int.TryParse(searchDH, out maDH);
                    if (laSoNguyen)
                        maDH = int.Parse(searchDH);
                    var dsDH = db.DONHANGs.Where(y => y.MaDonHang == maDH || y.KHACHHANG.TenKhachHang.Contains(searchDH) && y.TinhTrang.Equals("Đặt hàng thành công")).OrderByDescending(x => x.NgayDat).ToPagedList(page, pageSize);
                    var modelS = new QuanLyThongTin
                    {
                        PLDonHang = dsDH
                    };
                    ViewBag.Search = searchDH;
                    return View(modelS);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult DuyetDonHang(int maDonHang)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("NV"))
                {
                    DONHANG dh = db.DONHANGs.Single(ma => ma.MaDonHang == maDonHang);
                    dh.MaNhanVien = nv.MaNhanVien;
                    dh.TinhTrang = "Đang giao hàng";
                    db.SubmitChanges();
                    TempData["ThongBao"] = "Duyệt đơn hàng thành công";
                    TempData["LoaiTB"] = "alert-success";
                    TempData["ht"] = "block";
                }
                return RedirectToAction("QuanLyDonHang", "NhanVien");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult HuyDonHang(int maDonHang)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("NV"))
                {
                    DONHANG dh = db.DONHANGs.Single(ma => ma.MaDonHang == maDonHang);
                    dh.MaNhanVien = nv.MaNhanVien;
                    dh.TinhTrang = "Đã huỷ";
                    SANPHAM sp = null;
                    foreach (var item in dh.CHITIETDONHANGs)
                    {
                        sp = db.SANPHAMs.SingleOrDefault(x => x.MaSanPham == item.MaSanPham);
                        if (item.MaSanPham == sp.MaSanPham)
                            sp.SoLuongTon += item.SoLuong;
                    }
                    db.SubmitChanges();
                    TempData["ThongBao"] = "Huỷ đơn hàng thành công";
                    TempData["LoaiTB"] = "alert-success";
                    TempData["ht"] = "block";
                }
                return RedirectToAction("QuanLyDonHang", "NhanVien");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }
    }
}