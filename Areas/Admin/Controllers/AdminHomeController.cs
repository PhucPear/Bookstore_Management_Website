using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Models;
using Website_QuanLyNhaSachNguyenVanCu.Models;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        // GET: Admin/Home
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();

        public List<ThongKe> layDoanhThuTheoThang(int nam)
        {
            List<ThongKe> ds = new List<ThongKe>();
            string tt = "Đã thanh toán";
            var dsDH = db.DONHANGs.Where(x => x.ThanhToan.Equals(tt) && x.NgayDat.Value.Year == nam).GroupBy(y => y.NgayDat.Value.Month).OrderBy(z => z.Key).ToList();
            foreach (var dh in dsDH)
            {
                ThongKe s = new ThongKe();
                s.thang = "Tháng  " + dh.Key;
                s.doanhThu = (decimal)dh.Sum(x => x.TongThanhTien);
                ds.Add(s);
            }
            return ds;
        }

        public ActionResult QuanTri()
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN u = (NHANVIEN)Session["TaiKhoan"];
                if (u.Quyen.Equals("ADMIN"))
                {
                    DateTime time = DateTime.Now;
                    int sp = db.SANPHAMs.ToList().Count;
                    int dm = db.DANHMUCs.ToList().Count;
                    int lsp = db.LOAISANPHAMs.ToList().Count;
                    int km = db.KHUYENMAIs.ToList().Count;
                    int dh = db.DONHANGs.ToList().Count;
                    int kh = db.KHACHHANGs.ToList().Count;
                    int nv = db.NHANVIENs.ToList().Count;
                    int nxb = db.NHAXUATBANs.ToList().Count;
                    List<ThongKe> dsDT = layDoanhThuTheoThang(time.Year);
                    var model = new ThongKe
                    {
                        slSanPham = sp,
                        slDanhMuc = dm,
                        slLoaiSanPham = lsp,
                        slKhuyenMai = km,
                        slDonHang = dh,
                        slKhachHang = kh,
                        slNhanVien = nv,
                        dsDoanhThu = dsDT,
                        slNhaXuatBan = nxb
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult TaiKhoanPartial()
        {
            return PartialView();
        }

        public ActionResult NhanVien()
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN u = (NHANVIEN)Session["TaiKhoan"];
                if (u.Quyen.Equals("NV"))
                {
                    DateTime time = DateTime.Now;
                    int sp = db.SANPHAMs.ToList().Count;
                    int dm = db.DANHMUCs.ToList().Count;
                    int lsp = db.LOAISANPHAMs.ToList().Count;
                    int km = db.KHUYENMAIs.ToList().Count;
                    int dh = db.DONHANGs.ToList().Count;
                    int kh = db.KHACHHANGs.ToList().Count;
                    int nv = db.NHANVIENs.ToList().Count;
                    int nxb = db.NHAXUATBANs.ToList().Count;
                    List<ThongKe> dsDT = layDoanhThuTheoThang(time.Year);
                    var model = new ThongKe
                    {
                        slSanPham = sp,
                        slDanhMuc = dm,
                        slLoaiSanPham = lsp,
                        slKhuyenMai = km,
                        slDonHang = dh,
                        slKhachHang = kh,
                        slNhanVien = nv,
                        dsDoanhThu = dsDT,
                        slNhaXuatBan = nxb
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }
       
    }
}