using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using Website_QuanLyNhaSachNguyenVanCu.Models.Payment;

namespace Website_QuanLyNhaSachNguyenVanCu.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base   
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();
        public string KhoiTaoSaltChoUser()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public string MaHoaHashPassword(string password, string salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
            using (var sha256 = new SHA256Managed())
            {
                byte[] hashedPassword = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedPassword);
            }
        }

        public string ChuyenViewThanhChuoi(string viewName, object model, ControllerContext controllerContext)
        {
            ViewData.Model = model;
            
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }      

        public void GuiEmail(string nguoiNhan, string subject, string body)
        {
            var fromAddress = new MailAddress("asl.luffy1412@gmail.com", "Nhà Sách Nguyễn Văn Cừ");
            var toAddress = new MailAddress(nguoiNhan);
            const string fromPassword = "bhpp pvip yqda zmql";
            const string smtpServer = "smtp.gmail.com";
            const int smtpPort = 587;

            var smtp = new SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        public string UrlPayment(int typePayment, int maDonHang)
        {
            var urlPayment = "";
            var order = db.DONHANGs.FirstOrDefault(x => x.MaDonHang == maDonHang);
            //Get Config Info
            string vnp_Return_Url = ConfigurationManager.AppSettings["vnp_Return_Url"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TongThanhTien * 100000;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (typePayment == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (typePayment == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (typePayment == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.NgayDat.Value.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng #" + order.MaDonHang);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Return_Url);
            vnpay.AddRequestData("vnp_TxnRef", order.MaDonHang.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return urlPayment;
        }

        public bool checkSDT(string sdt)
        {
            string pattern = @"^0\d{9}$";
            return Regex.IsMatch(sdt, pattern);
        }
        
    }
}