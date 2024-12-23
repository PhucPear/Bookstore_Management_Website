using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using System.Net;
using System.Net.Mail;
using System.IO;
using Website_QuanLyNhaSachNguyenVanCu.Controllers;
using Website_QuanLyNhaSachNguyenVanCu.Models.Payment;
using System.Configuration;

namespace Website_QuanLyNhaSachNguyenVanCu.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();
        BaseController pub = new BaseController();

        public int tinhTongSLSPTrongGio(int maKH)
        {
            int sum = 0;
            var cart = db.GIOHANGs.Where(x => x.MaKhachHang == maKH).OrderByDescending(y => y.NgayThem).ToList();
            sum = cart.Count;
            return sum;
        }

        public ActionResult KiemTraSLTrongGioHang()
        {
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                int sl = tinhTongSLSPTrongGio(k.MaKhachHang);
                if (sl == 0)
                    return RedirectToAction("GioHangRong", "GioHang");
                return RedirectToAction("GioHang", "GioHang");
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

        private decimal tinhTongTTGio(int maKH)
        {
            decimal sum = 0;
            KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
            var cart = db.GIOHANGs.Where(x => x.MaKhachHang == maKH).OrderByDescending(y => y.NgayThem).ToList();
            sum = (decimal)cart.Sum(item => (item.SANPHAM.DonGia - item.SANPHAM.DonGia * item.SANPHAM.KHUYENMAI.TiLe) * item.SoLuong);
            return sum;
        }

        public ActionResult GioHang()
        {
            List<GIOHANG> cart = null;
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                cart = db.GIOHANGs.Where(x => x.MaKhachHang == k.MaKhachHang).OrderByDescending(y => y.NgayThem).ToList();
                if (cart != null)
                {
                    ViewBag.TongTT = tinhTongTTGio(k.MaKhachHang);
                    ViewBag.SoLuongSP = tinhTongSLSPTrongGio(k.MaKhachHang);
                    return View(cart);
                }
                else
                {
                    return RedirectToAction("GioHangRong", "GioHang");
                }

            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

        public ActionResult GioHangRong()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult CapNhatSLTrongGio(int maKH, int maSP, int sl)
        {
            var cartItem = db.GIOHANGs.Single(x => x.MaKhachHang == maKH && x.MaSanPham == maSP);
            SANPHAM sp = db.SANPHAMs.SingleOrDefault(x => x.MaSanPham == maSP);
            if (cartItem != null)
            {
                int slc = (int)cartItem.SoLuong;
                var dg = cartItem.SANPHAM.DonGia - cartItem.SANPHAM.DonGia * cartItem.SANPHAM.KHUYENMAI.TiLe;
                if (sl > slc)
                {
                    int slTonTru = sl - slc;
                    sp.SoLuongTon -= slTonTru;
                }
                else
                {
                    int slTonCong = slc - sl;
                    sp.SoLuongTon += slTonCong;
                }
                cartItem.SoLuong = sl;
                db.SubmitChanges();
                var tongTT = tinhTongTTGio(maKH);
                return Json(new { success = true, sl = sl, slc = slc, dg = dg, tongTT = tongTT });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult XoaItemGioHang(int maSP)
        {
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                GIOHANG item = db.GIOHANGs.Single(x => x.MaKhachHang == k.MaKhachHang && x.MaSanPham == maSP);
                SANPHAM sp = db.SANPHAMs.SingleOrDefault(x => x.MaSanPham == maSP);
                db.GIOHANGs.DeleteOnSubmit(item);
                sp.SoLuongTon += item.SoLuong;
                db.SubmitChanges();
                int slSP = tinhTongSLSPTrongGio(k.MaKhachHang);
                if (slSP > 0)
                    return RedirectToAction("GioHang", "GioHang");
                else
                    return RedirectToAction("GioHangRong", "GioHang");
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

        [HttpPost]
        public ActionResult ThemGioHang(int maSP)
        {
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                GIOHANG cart = db.GIOHANGs.SingleOrDefault(x => x.MaKhachHang == k.MaKhachHang && x.MaSanPham == maSP);
                SANPHAM sp = db.SANPHAMs.SingleOrDefault(x => x.MaSanPham == maSP);
                if (cart != null)
                {
                    cart.SoLuong += 1;
                    cart.NgayThem = DateTime.Now;
                    sp.SoLuongTon -= 1;
                }
                else
                {
                    GIOHANG newCart = new GIOHANG
                    {
                        MaKhachHang = k.MaKhachHang,
                        MaSanPham = maSP,
                        SoLuong = 1,
                        NgayThem = DateTime.Now
                    };
                    sp.SoLuongTon -= 1;
                    db.GIOHANGs.InsertOnSubmit(newCart);
                }
                db.SubmitChanges();
                int slItem = tinhTongSLSPTrongGio(k.MaKhachHang);
                return Json(new { success = true, sl = slItem });
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] != null)
            {
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                var cart = db.GIOHANGs.Where(x => x.MaKhachHang == k.MaKhachHang).OrderByDescending(y => y.NgayThem).ToList();
                if (cart == null)
                    return RedirectToAction("TrangChu", "Home");
                else
                {
                    ViewBag.TongSL = tinhTongSLSPTrongGio(k.MaKhachHang);
                    ViewBag.TongTT = tinhTongTTGio(k.MaKhachHang);
                    return View(cart);
                }
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            var sdt = f["soDT"].Trim();           
            var diaChi = f["dcGiaoHang"].Trim();            
            var type = f["typePayment"];
            int typePay = int.Parse(type);
            DONHANG dh = new DONHANG();
            if (Session["TaiKhoan"] != null)
            {
                //Thêm đơn hàng
                KHACHHANG k = (KHACHHANG)Session["TaiKhoan"];
                dh.MaKhachHang = k.MaKhachHang;
                dh.NgayDat = DateTime.Now;
                dh.SDT = sdt;
                dh.DiaChi = diaChi;
                if (typePay == 0)
                    dh.HinhThucTT = "COD";
                else
                    dh.HinhThucTT = "Chuyển khoản";
                db.DONHANGs.InsertOnSubmit(dh);
                db.SubmitChanges();
                var cart = db.GIOHANGs.Where(x => x.MaKhachHang == k.MaKhachHang).OrderByDescending(y => y.NgayThem).ToList();
                foreach (var item in cart)
                {
                    //Thêm chi tiết đơn hàng
                    CHITIETDONHANG ctDH = new CHITIETDONHANG();
                    ctDH.MaDonHang = dh.MaDonHang;
                    ctDH.MaSanPham = item.MaSanPham;
                    ctDH.SoLuong = item.SoLuong;
                    var ttKM = item.SANPHAM.DonGia - item.SANPHAM.DonGia * item.SANPHAM.KHUYENMAI.TiLe;
                    ctDH.ThanhTien = ttKM * item.SoLuong;
                    db.CHITIETDONHANGs.InsertOnSubmit(ctDH);
                }
                dh.TongThanhTien = tinhTongTTGio(k.MaKhachHang);
                dh.TinhTrang = "Đặt hàng thành công";
                dh.ThanhToan = "Chưa thanh toán";
                //Xoá giỏ hàng              
                db.GIOHANGs.DeleteAllOnSubmit(cart);
                db.SubmitChanges();
                //Phương thức thanh toán
                if (typePay == 0)
                {
                    pub.GuiEmail(k.Email, "Xác nhận đặt hàng", "Đặt hàng thành công");
                    return RedirectToAction("XacNhanDonHang", "GioHang");
                }
                else
                {
                    var url = pub.UrlPayment(typePay, dh.MaDonHang);
                    return Redirect(url);
                }
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }

        public ActionResult XacNhanThanhToan()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();
                foreach (string s in vnpayData)
                {
                    //lấy dữ liệu chuỗi Request
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                int maDH = int.Parse(orderCode); ;
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var dh = db.DONHANGs.FirstOrDefault(x => x.MaDonHang == maDH);
                        if (dh != null)
                        {
                            dh.ThanhToan = "Đã thanh toán";
                            db.SubmitChanges();
                        }
                        ViewBag.TrangThai = "Thanh toán thành công";
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.TrangThai = "Thanh toán thất bại: " + vnp_ResponseCode;
                    }
                    ViewBag.NoiDung = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                }
            }
            return View();
        }
    }
}