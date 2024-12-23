using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website_QuanLyNhaSachNguyenVanCu.Models;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Models
{
    public class ThongKe
    {
        //Xuất Report
        public List<DONHANG> dsDonHang { get; set; }
        public string theoThoiGian { get; set; }
        public string tongDoanhThu { get; set; }
        public string ngayLap { get; set; }
        public string nguoiLap { get; set; }

        //Thống Kê Số Lượng
        public int slSanPham { get; set; }
        public int slDanhMuc { get; set; }
        public int slLoaiSanPham { get; set; }
        public int slKhuyenMai { get; set; }
        public int slDonHang { get; set; }
        public int slKhachHang { get; set; }
        public int slNhanVien { get; set; }
        public int slNhaXuatBan { get; set; }
        //Thống Kê Trên Biểu Đồ
        public decimal doanhThu { get; set; }
        public string thang { get; set; }
        public List<ThongKe> dsDoanhThu { get; set; }

    }
}