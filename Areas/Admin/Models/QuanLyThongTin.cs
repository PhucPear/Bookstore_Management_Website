using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using PagedList;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Models
{
    public class QuanLyThongTin
    {
        public IPagedList<DANHMUC> PLDanhMuc { get; set; }
        public IPagedList<SANPHAM> PLSanPham { get; set; }
        public IPagedList<LOAISANPHAM> PLLoaiSanPham { get; set; }
        public IPagedList<KHUYENMAI> PLKhuyenMai { get; set; }
        public IPagedList<KHACHHANG> PLKhachHang { get; set; }
        public IPagedList<NHANVIEN> PLNhanVien { get; set; }
        public IPagedList<DONHANG> PLDonHang { get; set; }
        public List<DANHMUC> dsDanhMuc { get; set; }
        public List<LOAISANPHAM> dsLoaiSanPham { get; set; }
        public List<SANPHAM> dsSanPham { get; set; }
        public List<TACGIA> dsTacGia { get; set; }
        public List<NHAXUATBAN> dsNXB { get; set; }
        public List<NHACUNGCAP> dsNCC { get; set; }
        public List<KHUYENMAI> dsKhuyenMai { get; set; }
        public List<DONHANG> dsDonHang { get; set; }
        public List<CHITIETDONHANG> dsCTDonHang { get; set; }
        public string tongDoanhThu { get; set; }
        public DONHANG donHang { get; set; }

    }
}