using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Models;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using PagedList;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        // GET: Admin/DonHang
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();
        public ActionResult QuanLyDonHang(int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var dsDH = db.DONHANGs.OrderByDescending(x => x.NgayDat).ToPagedList(page, pageSize);               
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
                if (nv.Quyen.Equals("ADMIN"))
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
                if (nv.Quyen.Equals("ADMIN"))
                {
                    if (string.IsNullOrEmpty(searchDH))
                    {
                        return RedirectToAction("QuanLyDonHang", "DonHang");
                    }
                    int maDH = 0;
                    bool laSoNguyen = int.TryParse(searchDH, out maDH);
                    if (laSoNguyen)
                        maDH = int.Parse(searchDH);
                    var dsDH = db.DONHANGs.Where(y => y.MaDonHang == maDH || y.KHACHHANG.TenKhachHang.Contains(searchDH)).OrderByDescending(x => x.NgayDat).ToPagedList(page, pageSize);                  
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

        public ActionResult LocDonHang(FormCollection f, DateTime pt, string loc, int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var date = f["thoiGian"];
                    string locTheo = f["filter"];
                    if (date == null)
                        date = pt.ToString();
                    if (locTheo == null)
                        locTheo = loc;
                    if (date == "")
                    {
                        TempData["ThongBao"] = "Vui lòng chọn thời gian cần lọc!!";
                        TempData["LoaiTB"] = "alert-danger";
                        TempData["ht"] = "block";
                        return RedirectToAction("QuanLyDonHang", "DonHang");
                    }
                    else
                    {
                        DateTime time = DateTime.Parse(date);
                        IPagedList<DONHANG> dsDH = null;
                        if (locTheo.Equals("qq"))
                        {
                            int quy = (int)Math.Ceiling((double)time.Month / 3);
                            if (quy == 1)
                                dsDH = db.DONHANGs.Where(d => d.NgayDat.Value.Year == time.Year && d.NgayDat.Value.Month >= 1 && d.NgayDat.Value.Month <= 3).ToPagedList(page, pageSize);
                            else if (quy == 2)
                                dsDH = db.DONHANGs.Where(d => d.NgayDat.Value.Year == time.Year && d.NgayDat.Value.Month >= 4 && d.NgayDat.Value.Month <= 6).ToPagedList(page, pageSize);
                            else if (quy == 3)
                                dsDH = db.DONHANGs.Where(d => d.NgayDat.Value.Year == time.Year && d.NgayDat.Value.Month >= 7 && d.NgayDat.Value.Month <= 9).ToPagedList(page, pageSize);
                            else
                                dsDH = db.DONHANGs.Where(d => d.NgayDat.Value.Year == time.Year && d.NgayDat.Value.Month >= 10 && d.NgayDat.Value.Month <= 12).ToPagedList(page, pageSize);
                        }
                        else if (locTheo.Equals("mm"))
                        {
                            dsDH = db.DONHANGs.Where(x => x.NgayDat.Value.Year == time.Year && x.NgayDat.Value.Month == time.Month).OrderByDescending(y => y.NgayDat).ToPagedList(page, pageSize);
                        }
                        else if (locTheo.Equals("dd"))
                        {
                            dsDH = db.DONHANGs.Where(x => x.NgayDat.Value.Year == time.Year && x.NgayDat.Value.Month == time.Month && x.NgayDat.Value.Day == time.Day).OrderByDescending(y => y.NgayDat).ToPagedList(page, pageSize);
                        }
                        else
                        {
                            dsDH = db.DONHANGs.Where(x => x.NgayDat.Value.Year == time.Year).OrderByDescending(y => y.NgayDat).ToPagedList(page, pageSize);
                        }
                        var model = new QuanLyThongTin
                        {
                            PLDonHang = dsDH
                        };
                        ViewBag.Time = time;
                        ViewBag.Loc = locTheo;
                        return View(model);
                    }
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }
    }
}