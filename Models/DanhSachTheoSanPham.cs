using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_QuanLyNhaSachNguyenVanCu.Models
{
    public class DanhSachTheoSanPham
    {
        public List<SANPHAM> dsSach { get; set; }
        public List<SANPHAM> dsVanPhongPham { get; set; }
        public List<SANPHAM> dsQuaLuuNiem { get; set; }
        public List<DANHMUC> dsDanhMuc { get; set; }
        public List<LOAISANPHAM> dsLoaiSanPham { get; set; }
    }
}