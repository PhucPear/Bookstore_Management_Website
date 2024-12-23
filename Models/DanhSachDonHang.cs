using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_QuanLyNhaSachNguyenVanCu.Models
{
    public class DanhSachDonHang
    {
        public List<CHITIETDONHANG> dsDonHang { get; set; }
        public List<CHITIETDONHANG> dsDHChoThanhToan { get; set; }
        public List<CHITIETDONHANG> dsDHChoGiaoHang { get; set; }
        public List<CHITIETDONHANG> dsDHHoanThanh { get; set; }
        public List<CHITIETDONHANG> dsDHDaHuy { get; set; }

    }
}