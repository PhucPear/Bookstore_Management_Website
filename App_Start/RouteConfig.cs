using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Website_QuanLyNhaSachNguyenVanCu
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "DangNhap",
                url: "dang-nhap",
                defaults: new { controller = "KhachHang", action = "DangNhap"}
            );

            routes.MapRoute(
                name: "DangKy",
                url: "dang-Ky",
                defaults: new { controller = "KhachHang", action = "DangKy" }
            );

            routes.MapRoute(
                name: "QuenMatKhau",
                url: "quen-mat-khau",
                defaults: new { controller = "KhachHang", action = "QuenMatKhau" }
            );

            routes.MapRoute(
                name: "TrangChu",
                url: "trang-chu",
                defaults: new { controller = "Home", action = "TrangChu"}
            );          

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "TrangChu", id = UrlParameter.Optional }
            );
        }
    }
}
