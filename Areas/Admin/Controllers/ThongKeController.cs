using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using PagedList;
using Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Models;
using Excel = Microsoft.Office.Interop.Excel;
using Website_QuanLyNhaSachNguyenVanCu.Controllers;
using SelectPdf;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        // GET: Admin/ThongKe
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();
        BaseController pub = new BaseController();
        public ActionResult ListSanPham(int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    List<DANHMUC> dsDM = db.DANHMUCs.OrderBy(x => x.MaDanhMuc).ToList();
                    var dsSP = db.SANPHAMs.OrderBy(y => y.SoLuongTon).ToPagedList(page, pageSize);
                    var model = new QuanLyThongTin
                    {
                        PLSanPham = dsSP,
                        dsDanhMuc = dsDM
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult SearchSanPhamTon(string searchSP, int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    List<DANHMUC> dsDM = db.DANHMUCs.OrderBy(x => x.MaDanhMuc).ToList();
                    if (string.IsNullOrEmpty(searchSP))
                    {
                        return RedirectToAction("ListSanPham", "ThongKe");
                    }
                    int maSP = 0;
                    bool laSoNguyen = int.TryParse(searchSP, out maSP);
                    if (laSoNguyen)
                        maSP = int.Parse(searchSP);
                    var dsSPS = db.SANPHAMs.Where(x => x.MaSanPham == maSP || x.TenSanPham.Contains(searchSP)).OrderBy(y => y.SoLuongTon).ToPagedList(page, pageSize);
                    var modelS = new QuanLyThongTin
                    {
                        PLSanPham = dsSPS,
                        dsDanhMuc = dsDM
                    };
                    ViewBag.Search = searchSP;
                    return View(modelS);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }


        public ActionResult LocSanPham(FormCollection f, int ma, int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var danhMuc = f["danhMuc"];
                    List<DANHMUC> dsDM = db.DANHMUCs.OrderBy(x => x.MaDanhMuc).ToList();
                    int maDM = -1;
                    IPagedList<SANPHAM> dsSP = null;
                    if (maDM != ma && danhMuc != null)
                    {
                        maDM = int.Parse(danhMuc);
                        if (maDM == 0)
                        {
                            return RedirectToAction("ListSanPham", "ThongKe");
                        }
                        else
                        {
                            dsSP = db.SANPHAMs.Where(x => x.LOAISANPHAM.MaDanhMuc == maDM).OrderBy(y => y.SoLuongTon).ToPagedList(page, pageSize);
                        }
                    }
                    else
                    {
                        maDM = ma;
                        dsSP = db.SANPHAMs.Where(x => x.LOAISANPHAM.MaDanhMuc == maDM).OrderBy(y => y.SoLuongTon).ToPagedList(page, pageSize);
                    }
                    var model = new QuanLyThongTin
                    {
                        PLSanPham = dsSP,
                        dsDanhMuc = dsDM
                    };
                    ViewBag.ma = maDM;
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        //Thống kê số lượng tồn của sách ra file Excel
        public ActionResult ThongKeSoLuongTonSach(int maDM)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    List<SANPHAM> dsSach = null;
                    if (maDM != 0)
                    {
                        dsSach = db.SANPHAMs.Where(x => x.LOAISANPHAM.MaDanhMuc == maDM).OrderBy(y => y.SoLuongTon).ToList();
                    }
                    else
                    {
                        dsSach = db.SANPHAMs.OrderBy(y => y.SoLuongTon).ToList();
                    }
                    // Tạo một đối tượng Excel
                    var excelApp = new Excel.Application();
                    excelApp.Visible = false;

                    // Tạo một WorkBook mới
                    var workBook = excelApp.Workbooks.Add();
                    var workSheet = (Excel.Worksheet)workBook.Worksheets[1];

                    // Đặt tên các cột
                    workSheet.Cells[1, 2] = "THỐNG KÊ SỐ LƯỢNG TỒN CỦA SÁCH";
                    var row1 = workSheet.Rows[1] as Excel.Range;
                    row1.Font.Bold = true;
                    row1.Font.Size = 24;
                    row1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    workSheet.Cells[2, 1] = "Mã Sách";
                    workSheet.Cells[2, 2] = "Tên Sách";
                    workSheet.Cells[2, 3] = "Đơn Giá";
                    workSheet.Cells[2, 4] = "Số Lượng Tồn";

                    var row2 = workSheet.Rows[2] as Excel.Range;
                    row2.Font.Bold = true;
                    row2.Font.Size = 20;
                    row2.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    var col1 = workSheet.Columns[1] as Excel.Range;
                    col1.ColumnWidth = 17;
                    var col2 = workSheet.Columns[2] as Excel.Range;
                    col2.ColumnWidth = 150;
                    var col3 = workSheet.Columns[3] as Excel.Range;
                    col3.ColumnWidth = 30;
                    var col4 = workSheet.Columns[4] as Excel.Range;
                    col4.ColumnWidth = 30;

                    // Đổ dữ liệu vào Excel
                    int row = 3;
                    var rowk = workSheet.Rows[1] as Excel.Range;
                    foreach (var sach in dsSach)
                    {
                        workSheet.Cells[row, 1] = sach.MaSanPham;
                        workSheet.Cells[row, 2] = sach.TenSanPham;
                        workSheet.Cells[row, 3] = sach.DonGia;
                        workSheet.Cells[row, 4] = sach.SoLuongTon;
                        rowk = workSheet.Rows[row] as Excel.Range;
                        rowk.Font.Size = 18;
                        rowk.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        row++;
                    }
                    //Tổng số lượng tồn
                    int count = dsSach.Count + 4;
                    workSheet.Cells[count, 3] = "Tổng:";
                    var tongSL = dsSach.Sum(x => x.SoLuongTon);
                    workSheet.Cells[count, 4] = tongSL;
                    var tong = workSheet.Rows[count] as Excel.Range;
                    tong.Font.Bold = true;
                    tong.Font.Size = 18;
                    tong.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    //Thời gian và người lập
                    count += 2;
                    workSheet.Cells[count, 4] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    var date = workSheet.Rows[count] as Excel.Range;
                    date.Font.Size = 18;
                    date.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    count++;
                    workSheet.Cells[count, 4] = "Người lập";
                    var nl = workSheet.Rows[count] as Excel.Range;
                    nl.Font.Bold = true;
                    nl.Font.Size = 18;
                    nl.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    count++;
                    workSheet.Cells[count, 4] = nv.TenNhanVien;
                    var tl = workSheet.Rows[count] as Excel.Range;
                    tl.Font.Size = 18;
                    tl.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    DateTime time = DateTime.Now;
                    string str = "" + time.ToString("yyyyMMdd") + "_" + time.Hour + "h_" + time.Minute + "p";
                    string fileName = "SoLuongTonSanPham_" + str + ".xlsx";
                    // Lưu file Excel vào một đường dẫn tạm thời trên server
                    string tempFilePath = Server.MapPath("~/App_Data/ThongKeBaoCao/SoLuongTon/" + fileName + "");
                    workBook.SaveAs(tempFilePath);
                    workBook.Close();
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workBook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    // Trả về file Excel cho người dùng
                    return File(tempFilePath, "application/vnd.ms-excel", fileName);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult QuanLyBanHang(int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    string tt = "Đã thanh toán";
                    var dsDH = db.DONHANGs.Where(y => y.ThanhToan.Equals(tt)).OrderByDescending(x => x.NgayDat).ToPagedList(page, pageSize);
                    var doanhThu = db.DONHANGs.Where(y => y.ThanhToan.Equals(tt)).OrderByDescending(x => x.NgayDat).ToList();
                    decimal tongDoanhThu = (decimal)doanhThu.Sum(x => x.TongThanhTien);
                    string str = "Tổng doanh thu của nhà sách là:   " + tongDoanhThu;
                    var model = new QuanLyThongTin
                    {
                        PLDonHang = dsDH,
                        dsDonHang = doanhThu,
                        tongDoanhThu = str
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult SearchBanHang(string searchBH, int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    if (string.IsNullOrEmpty(searchBH))
                    {
                        return RedirectToAction("QuanLyBanHang", "ThongKe");
                    }
                    int maDH = 0;
                    bool laSoNguyen = int.TryParse(searchBH, out maDH);
                    if (laSoNguyen)
                        maDH = int.Parse(searchBH);
                    string tt = "Đã thanh toán";
                    var dsDH = db.DONHANGs.Where(y => y.MaDonHang == maDH || y.KHACHHANG.TenKhachHang.Contains(searchBH) && y.ThanhToan.Equals(tt)).OrderByDescending(x => x.NgayDat).ToPagedList(page, pageSize);
                    var doanhThu = db.DONHANGs.Where(y => y.MaDonHang == maDH || y.KHACHHANG.TenKhachHang.Contains(searchBH) && y.ThanhToan.Equals(tt)).OrderByDescending(x => x.NgayDat).ToList();
                    decimal tongDoanhThu = (decimal)doanhThu.Sum(x => x.TongThanhTien);
                    string str = "Tổng doanh thu của là:   " + tongDoanhThu;
                    var modelS = new QuanLyThongTin
                    {
                        PLDonHang = dsDH,
                        dsDonHang = doanhThu,
                        tongDoanhThu = str
                    };
                    ViewBag.Search = searchBH;
                    return View(modelS);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        private List<DONHANG> doanhThuTheoQuy(DateTime date)
        {
            string tt = "Đã thanh toán";
            int quy = (int)Math.Ceiling((double)date.Month / 3);
            List<DONHANG> dsDH = null;
            if (quy == 1)
                dsDH = db.DONHANGs.Where(d => d.ThanhToan.Equals(tt) && d.NgayDat.Value.Year == date.Year && d.NgayDat.Value.Month >= 1 && d.NgayDat.Value.Month <= 3).ToList();
            else if (quy == 2)
                dsDH = db.DONHANGs.Where(d => d.ThanhToan.Equals(tt) && d.NgayDat.Value.Year == date.Year && d.NgayDat.Value.Month >= 4 && d.NgayDat.Value.Month <= 6).ToList();
            else if (quy == 3)
                dsDH = db.DONHANGs.Where(d => d.ThanhToan.Equals(tt) && d.NgayDat.Value.Year == date.Year && d.NgayDat.Value.Month >= 7 && d.NgayDat.Value.Month <= 9).ToList();
            else
                dsDH = db.DONHANGs.Where(d => d.ThanhToan.Equals(tt) && d.NgayDat.Value.Year == date.Year && d.NgayDat.Value.Month >= 10 && d.NgayDat.Value.Month <= 12).ToList();
            return dsDH;
        }

        private ThongKe layDoanhThu(DateTime date, string locTheo)
        {
            ThongKe ql = new ThongKe();
            string tt = "Đã thanh toán";
            if (date.ToString() == "1000-01-01")
                locTheo = "all";
            decimal tongDoanhThu = 0;
            if (locTheo.Equals("yy"))
            {
                ql.dsDonHang = db.DONHANGs.Where(x => x.ThanhToan.Equals(tt) && x.NgayDat.Value.Year == date.Year).OrderByDescending(y => y.NgayDat).ToList();
                tongDoanhThu = (decimal)ql.dsDonHang.Sum(x => x.TongThanhTien);
                ql.tongDoanhThu = "Tổng doanh thu theo của năm " + date.Year + " là:   " + tongDoanhThu;
                ql.theoThoiGian = "Theo năm " + date.Year + "";
            }
            else if (locTheo.Equals("qq"))
            {
                ql.dsDonHang = doanhThuTheoQuy(date);
                tongDoanhThu = (decimal)ql.dsDonHang.Sum(x => x.TongThanhTien);
                int quy = (int)Math.Ceiling((double)date.Month / 3);
                ql.tongDoanhThu = "Tổng doanh thu quý " + quy + " của năm " + date.Year + " là:   " + tongDoanhThu;
                ql.theoThoiGian = "Theo quý " + quy + " của năm " + date.Year + "";
            }
            else if (locTheo.Equals("mm"))
            {
                ql.dsDonHang = db.DONHANGs.Where(x => x.ThanhToan.Equals(tt) && x.NgayDat.Value.Year == date.Year && x.NgayDat.Value.Month == date.Month).OrderByDescending(y => y.NgayDat).ToList();
                tongDoanhThu = (decimal)ql.dsDonHang.Sum(x => x.TongThanhTien);
                ql.tongDoanhThu = "Tổng doanh thu theo tháng " + date.Month + " của năm " + date.Year + " là:   " + tongDoanhThu;
                ql.theoThoiGian = "Theo tháng " + date.Month + " của năm " + date.Year + "";
            }
            else if (locTheo.Equals("dd"))
            {
                ql.dsDonHang = db.DONHANGs.Where(x => x.ThanhToan.Equals(tt) && x.NgayDat.Value.Year == date.Year && x.NgayDat.Value.Month == date.Month && x.NgayDat.Value.Day == date.Day).OrderByDescending(y => y.NgayDat).ToList();
                tongDoanhThu = (decimal)ql.dsDonHang.Sum(x => x.TongThanhTien);
                ql.tongDoanhThu = "Tổng doanh thu của ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year + " là:   " + tongDoanhThu;
                ql.theoThoiGian = "Theo ngày " + date.Day + " tháng " + date.Month + " năm " + date.Year + "";
            }
            else
            {
                ql.dsDonHang = db.DONHANGs.Where(y => y.ThanhToan.Equals(tt)).OrderByDescending(x => x.NgayDat).ToList();
                tongDoanhThu = (decimal)ql.dsDonHang.Sum(x => x.TongThanhTien);
                ql.tongDoanhThu = "Tổng doanh thu của nhà sách là:   " + tongDoanhThu;
            }
            return ql;
        }

        public ActionResult LocBanHang(FormCollection f, DateTime pt, string loc, int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    string tt = "Đã thanh toán";
                    var date = f["thoiGian"];
                    string locTheo = f["filter"];
                    if (date == null)
                        date = pt.ToString();
                    if (locTheo == null)
                        locTheo = loc;
                    if (date == "")
                    {
                        TempData["ThongBao"] = "Vui lòng chọn thời gian cần thống kê!!";
                        TempData["LoaiTB"] = "alert-danger";
                        TempData["ht"] = "block";
                        return RedirectToAction("QuanLyBanHang", "ThongKe");
                    }
                    else
                    {
                        DateTime time = DateTime.Parse(date);
                        ThongKe ql = layDoanhThu(time, locTheo);
                        IPagedList<DONHANG> dsDH = null;
                        if (locTheo.Equals("qq"))
                        {
                            int quy = (int)Math.Ceiling((double)time.Month / 3);
                            if (quy == 1)
                                dsDH = db.DONHANGs.Where(d => d.ThanhToan.Equals(tt) && d.NgayDat.Value.Year == time.Year && d.NgayDat.Value.Month >= 1 && d.NgayDat.Value.Month <= 3).ToPagedList(page, pageSize);
                            else if (quy == 2)
                                dsDH = db.DONHANGs.Where(d => d.ThanhToan.Equals(tt) && d.NgayDat.Value.Year == time.Year && d.NgayDat.Value.Month >= 4 && d.NgayDat.Value.Month <= 6).ToPagedList(page, pageSize);
                            else if (quy == 3)
                                dsDH = db.DONHANGs.Where(d => d.ThanhToan.Equals(tt) && d.NgayDat.Value.Year == time.Year && d.NgayDat.Value.Month >= 7 && d.NgayDat.Value.Month <= 9).ToPagedList(page, pageSize);
                            else
                                dsDH = db.DONHANGs.Where(d => d.ThanhToan.Equals(tt) && d.NgayDat.Value.Year == time.Year && d.NgayDat.Value.Month >= 10 && d.NgayDat.Value.Month <= 12).ToPagedList(page, pageSize);
                        }
                        else if (locTheo.Equals("mm"))
                        {
                            dsDH = db.DONHANGs.Where(x => x.ThanhToan.Equals(tt) && x.NgayDat.Value.Year == time.Year && x.NgayDat.Value.Month == time.Month).OrderByDescending(y => y.NgayDat).ToPagedList(page, pageSize);
                        }
                        else if (locTheo.Equals("dd"))
                        {
                            dsDH = db.DONHANGs.Where(x => x.ThanhToan.Equals(tt) && x.NgayDat.Value.Year == time.Year && x.NgayDat.Value.Month == time.Month && x.NgayDat.Value.Day == time.Day).OrderByDescending(y => y.NgayDat).ToPagedList(page, pageSize);
                        }
                        else
                        {
                            dsDH = db.DONHANGs.Where(x => x.ThanhToan.Equals(tt) && x.NgayDat.Value.Year == time.Year).OrderByDescending(y => y.NgayDat).ToPagedList(page, pageSize);
                        }
                        var model = new QuanLyThongTin
                        {
                            PLDonHang = dsDH,
                            dsDonHang = ql.dsDonHang,
                            tongDoanhThu = ql.tongDoanhThu
                        };
                        ViewBag.Time = time;
                        ViewBag.Loc = locTheo;
                        return View(model);
                    }
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }


        //Thống kê doanh thu bán hàng ra file PDF
        public ActionResult ThongKeDoanhThuBanHang(DateTime date, string locTheo)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    ThongKe ql = layDoanhThu(date, locTheo);
                    DateTime time = DateTime.Now;
                    var model = new ThongKe
                    {
                        dsDonHang = ql.dsDonHang,
                        tongDoanhThu = ql.tongDoanhThu,
                        ngayLap = time.ToString("dd/MM/yyyy"),
                        nguoiLap = nv.TenNhanVien,
                        theoThoiGian = ql.theoThoiGian
                    };
                    HtmlToPdf converter = new HtmlToPdf();
                    //Định dạng file PDF
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                    converter.Options.MarginLeft = 10;
                    converter.Options.MarginRight = 10;
                    converter.Options.MarginTop = 20;
                    converter.Options.MarginBottom = 20;

                    var htmlPDF = pub.ChuyenViewThanhChuoi("~/Areas/Admin/Views/ThongKe/ThongKeDoanhThuBanHang.cshtml", model, ControllerContext);
                    PdfDocument doccument = converter.ConvertHtmlString(htmlPDF);
                    string strTime = "" + time.ToString("yyyyMMdd") + "_" + time.Hour + "h_" + time.Minute + "p";
                    string fileName = "DoanhThuBanHang_" + strTime + ".pdf";
                    string tempFilePath = Server.MapPath("~/App_Data/ThongKeBaoCao/DoanhThuBanHang/" + fileName + "");
                    doccument.Save(tempFilePath);
                    doccument.Close();

                    return File(tempFilePath, "application/pdf", fileName);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }




    }
}