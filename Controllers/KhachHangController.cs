using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using PagedList;

namespace Website_QuanLyNhaSachNguyenVanCu.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: KhachHang
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
                KHACHHANG k = db.KHACHHANGs.SingleOrDefault(x => x.Email.Equals(email));
                if (k != null)
                {
                    if (k.TrangThai.Equals("OPEN"))
                    {
                        string hash = pub.MaHoaHashPassword(matKhau, k.Salt);
                        if (hash == k.MatKhau)
                        {
                            Session["TaiKhoan"] = k;
                            return Json(new { success = true });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Tài khoản của bạn đã bị khoá!!" });
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
            return RedirectToAction("DangNhap", "KhachHang");
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(string hoTen, string email, string sdt, string diaChi, string matKhau)
        {
            if (!string.IsNullOrEmpty(hoTen) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(sdt) && !string.IsNullOrEmpty(diaChi) && !string.IsNullOrEmpty(matKhau))
            {
                bool kh = db.KHACHHANGs.Any(x => x.Email.Equals(email));
                if (kh)
                {
                    return Json(new { success = false, message = "Đã có tài khoản của Email này!!" });
                }
                else
                {
                    KHACHHANG k = new KHACHHANG();
                    k.TenKhachHang = hoTen;
                    k.Email = email;
                    k.SDT = sdt;
                    k.DiaChi = diaChi;
                    string salt = pub.KhoiTaoSaltChoUser();
                    k.Salt = salt;
                    string hash = pub.MaHoaHashPassword(matKhau, salt);
                    k.MatKhau = hash;
                    k.TrangThai = "OPEN";
                    db.KHACHHANGs.InsertOnSubmit(k);
                    db.SubmitChanges();
                    return Json(new { success = true, message = "Đăng ký tài khoản thành công" });
                }
            }
            return Json(new { success = false, message = "Thông tin không hợp lệ. Hãy thử lại!!" });
        }

        [HttpGet]
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GuiMaOTP(string email)
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            bool kh = db.KHACHHANGs.Any(x => x.Email.Equals(email));
            if (kh)
            {
                pub.GuiEmail(email, "Quên mật khẩu", "Mã OTP xác nhận: " + otp);
                Session["maOTP"] = otp;
                return Json(new { success = true, message = "Mã OTP đã được gửi thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Email không tồn tại. Vui lòng kiểm tra lại!!" });
            }
        }


        [HttpPost]
        public JsonResult QuenMatKhau(string email, string otp, string matKhau)
        {
            var maOTP = Session["maOTP"];
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(otp) && !string.IsNullOrEmpty(matKhau))
            {
                KHACHHANG k = db.KHACHHANGs.SingleOrDefault(x => x.Email.Equals(email));
                int otpGui = int.Parse(maOTP.ToString());
                int otpNhan = int.Parse(otp);
                if (otpNhan == otpGui)
                {
                    string hash = pub.MaHoaHashPassword(matKhau, k.Salt);
                    k.MatKhau = hash;
                    db.SubmitChanges();
                    return Json(new { success = true, message = "Đổi mật khẩu thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Mã OTP không chính xác!!" });
                }
            }
            return Json(new { success = false, message = "Thông tin không hợp lệ. Hãy thử lại!!" });
        }

        public ActionResult DanhSachDonHang()
        {
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                var dsDH = db.CHITIETDONHANGs.Where(x => x.DONHANG.MaKhachHang == k.MaKhachHang && x.DONHANG.TinhTrang.Equals("Đặt hàng thành công")).OrderByDescending(y => y.DONHANG.NgayDat).ToList();
                var dsDHCTT = db.CHITIETDONHANGs.Where(x => x.DONHANG.MaKhachHang == k.MaKhachHang && x.DONHANG.ThanhToan.Equals("Chưa thanh toán") && x.DONHANG.HinhThucTT.Equals("Chuyển khoản")).OrderByDescending(y => y.DONHANG.NgayDat).ToList();
                var dsDHCGH = db.CHITIETDONHANGs.Where(x => x.DONHANG.MaKhachHang == k.MaKhachHang && x.DONHANG.TinhTrang.Equals("Đang giao hàng")).OrderByDescending(y => y.DONHANG.NgayDat).ToList();
                var dsDHHT = db.CHITIETDONHANGs.Where(x => x.DONHANG.MaKhachHang == k.MaKhachHang && x.DONHANG.TinhTrang.Equals("Đơn hàng đã được giao thành công")).OrderByDescending(y => y.DONHANG.NgayDat).ToList();
                var dsDHHuy = db.CHITIETDONHANGs.Where(x => x.DONHANG.MaKhachHang == k.MaKhachHang && x.DONHANG.TinhTrang.Equals("Đã huỷ")).OrderByDescending(y => y.DONHANG.NgayDat).ToList();
                var model = new DanhSachDonHang
                {
                    dsDonHang = dsDH,
                    dsDHChoThanhToan = dsDHCTT,
                    dsDHChoGiaoHang = dsDHCGH,
                    dsDHHoanThanh = dsDHHT,
                    dsDHDaHuy = dsDHHuy
                };
                return View(model);
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

        public ActionResult HuyDonHang(int maDonHang)
        {
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                DONHANG dh = db.DONHANGs.Single(x => x.MaDonHang == maDonHang && x.MaKhachHang == k.MaKhachHang);
                dh.TinhTrang = "Đã huỷ";
                SANPHAM sp = null;
                foreach (var item in dh.CHITIETDONHANGs)
                {
                    sp = db.SANPHAMs.SingleOrDefault(x => x.MaSanPham == item.MaSanPham);
                    if (item.MaSanPham == sp.MaSanPham)
                        sp.SoLuongTon += item.SoLuong;
                }
                db.SubmitChanges();
                return RedirectToAction("DanhSachDonHang", "KhachHang");
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

        public ActionResult NhanDonHang(int maDonHang)
        {
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                DONHANG dh = db.DONHANGs.Single(x => x.MaDonHang == maDonHang && x.MaKhachHang == k.MaKhachHang);
                dh.TinhTrang = "Đơn hàng đã được giao thành công";
                dh.NgayGiao = DateTime.Now;
                db.SubmitChanges();
                return RedirectToAction("DanhSachDonHang", "KhachHang");
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

    }
}