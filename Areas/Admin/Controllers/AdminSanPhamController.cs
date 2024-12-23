using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using PagedList;
using Excel = Microsoft.Office.Interop.Excel;
using Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Models;
using System.IO;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Controllers
{
    public class AdminSanPhamController : Controller
    {
        // GET: Admin/AdminSanPham
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();
        public ActionResult QuanTriDanhMuc(int page = 1, int pageSize = 5)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var dsDM = db.DANHMUCs.OrderByDescending(x => x.MaDanhMuc).ToPagedList(page, pageSize);
                    var list = db.DANHMUCs.OrderByDescending(x => x.MaDanhMuc).ToList();
                    var model = new QuanLyThongTin
                    {
                        PLDanhMuc = dsDM,
                        dsDanhMuc = list
                    };                    
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        [HttpPost]
        public ActionResult ThemDanhMuc(FormCollection f)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    DANHMUC c = new DANHMUC();
                    var tenDM = f["tenDM"].Trim();
                    if (!String.IsNullOrEmpty(tenDM))
                    {
                        c.TenDanhMuc = tenDM;
                        db.DANHMUCs.InsertOnSubmit(c);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Thêm danh mục thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriDanhMuc", "AdminSanPham");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult XoaDanhMuc(int maDanhMuc)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    DANHMUC c = db.DANHMUCs.Single(ma => ma.MaDanhMuc == maDanhMuc);
                    bool spTC = db.SANPHAMs.Any(y => y.LOAISANPHAM.MaDanhMuc == maDanhMuc);
                    if (spTC)
                    {
                        TempData["ThongBao"] = "Xoá không thành công đã có sản phẩm thuộc danh mục này!!";
                        TempData["LoaiTB"] = "alert-danger";
                        TempData["ht"] = "block";
                    }
                    else
                    {
                        db.DANHMUCs.DeleteOnSubmit(c);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Xoá danh mục thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriDanhMuc", "AdminSanPham");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        [HttpPost]
        public ActionResult SuaDanhMuc(int maDanhMuc, FormCollection f)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    DANHMUC c = db.DANHMUCs.Single(ma => ma.MaDanhMuc == maDanhMuc);
                    var tenDM = f["tenDM"].Trim();
                    if (!String.IsNullOrEmpty(tenDM))
                    {
                        c.TenDanhMuc = tenDM;
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Sửa danh mục thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriDanhMuc", "AdminSanPham");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult SearchDanhMuc(string searchDM, int page = 1, int pageSize = 5)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    if (string.IsNullOrEmpty(searchDM))
                    {
                        return RedirectToAction("QuanTriDanhMuc", "AdminSanPham");
                    }
                    var dsDMS = db.DANHMUCs.Where(x => x.TenDanhMuc.Contains(searchDM)).OrderByDescending(x => x.MaDanhMuc).ToPagedList(page, pageSize);
                    var models = new QuanLyThongTin
                    {
                        PLDanhMuc = dsDMS
                    };
                    ViewBag.Search = searchDM;
                    return View(models);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult QuanTriLoaiSanPham(int page = 1, int pageSize = 8)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var dsLSP = db.LOAISANPHAMs.OrderBy(x => x.MaDanhMuc).ToPagedList(page, pageSize);
                    List<DANHMUC> dsDanhMuc = db.DANHMUCs.OrderBy(x => x.MaDanhMuc).ToList();
                    var model = new QuanLyThongTin
                    {
                        PLLoaiSanPham = dsLSP,
                        dsDanhMuc = dsDanhMuc
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult XoaLoaiSanPham(int maLSP)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    LOAISANPHAM s = db.LOAISANPHAMs.Single(ma => ma.MaLoaiSP == maLSP);
                    bool spTC = db.SANPHAMs.Any(y => y.MaLoaiSP == maLSP);
                    if (spTC)
                    {
                        TempData["ThongBao"] = "Xoá không thành công đã có sản phẩm thuộc loại này!!";
                        TempData["LoaiTB"] = "alert-danger";
                        TempData["ht"] = "block";
                    }
                    else
                    {
                        db.LOAISANPHAMs.DeleteOnSubmit(s);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Xoá loại sản phẩm thành công thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriLoaiSanPham", "AdminSanPham");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        [HttpPost]
        public ActionResult SuaLoaiSanPham(int maLSP, FormCollection f)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    LOAISANPHAM c = db.LOAISANPHAMs.Single(ma => ma.MaLoaiSP == maLSP);
                    var tenLoai = f["tenLoai"].Trim();
                    var danhMuc = f["danhMuc"];
                    if (!String.IsNullOrEmpty(tenLoai))
                    {
                        c.TenLoaiSP = tenLoai;
                        c.MaDanhMuc = int.Parse(danhMuc);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Sửa loại sản phẩm thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriLoaiSanPham", "AdminSanPham");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        [HttpPost]
        public ActionResult ThemLoaiSanPham(FormCollection f)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    LOAISANPHAM s = new LOAISANPHAM();
                    var tenLoai = f["tenLoai"].Trim();
                    var danhMuc = f["danhMuc"];
                    if (!String.IsNullOrEmpty(tenLoai))
                    {
                        s.TenLoaiSP = tenLoai;
                        s.MaDanhMuc = int.Parse(danhMuc);
                        db.LOAISANPHAMs.InsertOnSubmit(s);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Thêm loại sản phẩm thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriLoaiSanPham", "AdminSanPham");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult SearchLoaiSanPham(string searchLoai, int page = 1, int pageSize = 5)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    List<DANHMUC> dsDanhMuc = db.DANHMUCs.OrderBy(x => x.MaDanhMuc).ToList();
                    if (string.IsNullOrEmpty(searchLoai))
                    {
                        return RedirectToAction("QuanTriLoaiSanPham", "AdminSanPham");
                    }
                    var dsLoaiS = db.LOAISANPHAMs.Where(x => x.TenLoaiSP.Contains(searchLoai)).OrderBy(x => x.MaDanhMuc).ToPagedList(page, pageSize);
                    var models = new QuanLyThongTin
                    {
                        PLLoaiSanPham = dsLoaiS,
                        dsDanhMuc = dsDanhMuc
                    };
                    ViewBag.Search = searchLoai;
                    return View(models);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }
        public ActionResult QuanTriSanPham(int page = 1, int pageSize = 6)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var dsSP = db.SANPHAMs.OrderBy(y => y.SoLuongTon).ToPagedList(page, pageSize);
                    List<LOAISANPHAM> dsLSP = db.LOAISANPHAMs.OrderBy(x => x.MaDanhMuc).ToList();
                    List<TACGIA> dsTG = db.TACGIAs.OrderBy(x => x.MaTacGia).ToList();
                    List<NHAXUATBAN> dsNXB = db.NHAXUATBANs.OrderBy(x => x.MaNXB).ToList();
                    List<KHUYENMAI> dsKM = db.KHUYENMAIs.OrderBy(x => x.TiLe).ToList();
                    List<DANHMUC> dsDM = db.DANHMUCs.OrderBy(x => x.MaDanhMuc).ToList();
                    var model = new QuanLyThongTin
                    {
                        PLSanPham = dsSP,
                        dsLoaiSanPham = dsLSP,
                        dsTacGia = dsTG,
                        dsNXB = dsNXB,
                        dsKhuyenMai = dsKM,
                        dsDanhMuc = dsDM
                    };
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult LocSanPham(FormCollection f, int ma, int page = 1, int pageSize = 6)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var danhMuc = f["danhMuc"];
                    List<DANHMUC> dsDM = db.DANHMUCs.OrderBy(x => x.MaDanhMuc).ToList();
                    List<LOAISANPHAM> dsLSP = db.LOAISANPHAMs.OrderBy(x => x.MaDanhMuc).ToList();
                    List<TACGIA> dsTG = db.TACGIAs.OrderBy(x => x.MaTacGia).ToList();
                    List<NHAXUATBAN> dsNXB = db.NHAXUATBANs.OrderBy(x => x.MaNXB).ToList();
                    List<KHUYENMAI> dsKM = db.KHUYENMAIs.OrderBy(x => x.TiLe).ToList();
                    int maDM = -1;
                    IPagedList<SANPHAM> dsSP = null;
                    if (maDM != ma && danhMuc != null)
                    {
                        maDM = int.Parse(danhMuc);
                        if (maDM == 0)
                        {
                            return RedirectToAction("QuanTriSanPham", "AdminSanPham");
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
                        dsDanhMuc = dsDM,
                        dsLoaiSanPham = dsLSP,
                        dsTacGia = dsTG,
                        dsNXB = dsNXB,
                        dsKhuyenMai = dsKM
                    };
                    ViewBag.ma = maDM;
                    return View(model);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult SearchSanPham(string searchSP, int page = 1, int pageSize = 6)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    List<LOAISANPHAM> dsLSP = db.LOAISANPHAMs.OrderBy(x => x.MaDanhMuc).ToList();
                    List<TACGIA> dsTG = db.TACGIAs.OrderBy(x => x.MaTacGia).ToList();
                    List<NHAXUATBAN> dsNXB = db.NHAXUATBANs.OrderBy(x => x.MaNXB).ToList();
                    List<KHUYENMAI> dsKM = db.KHUYENMAIs.OrderBy(x => x.TiLe).ToList();
                    if (string.IsNullOrEmpty(searchSP))
                    {
                        return RedirectToAction("QuanTriSanPham", "AdminSanPham");
                    }
                    var dsSPS = db.SANPHAMs.Where(x => x.TenSanPham.Contains(searchSP)).OrderByDescending(y => y.SoLuongTon).ToPagedList(page, pageSize);
                    var modelS = new QuanLyThongTin
                    {
                        PLSanPham = dsSPS,
                        dsLoaiSanPham = dsLSP,
                        dsTacGia = dsTG,
                        dsNXB = dsNXB,
                        dsKhuyenMai = dsKM
                    };
                    ViewBag.Search = searchSP;
                    return View(modelS);
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }
        public ActionResult XoaSanPham(int maSanPham)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    SANPHAM p = db.SANPHAMs.Single(x => x.MaSanPham == maSanPham);
                    bool ghTC = db.GIOHANGs.Any(y => y.MaSanPham == maSanPham);
                    bool dhTC = db.CHITIETDONHANGs.Any(y => y.MaSanPham == maSanPham);
                    if (ghTC && dhTC)
                    {
                        TempData["ThongBao"] = "Không thể xoá sản phẩm này!!";
                        TempData["LoaiTB"] = "alert-danger";
                        TempData["ht"] = "block";
                    }
                    else
                    {
                        db.SANPHAMs.DeleteOnSubmit(p);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Xoá sản phẩm thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriSanPham", "AdminSanPham");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        [HttpPost]
        public ActionResult SuaSanPham(int maSanPham, FormCollection f)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    SANPHAM p = db.SANPHAMs.Single(x => x.MaSanPham == maSanPham);
                    var loaiSP = f["loaiSP"];
                    var tacGia = f["tacGia"];
                    var nxb = f["NXB"];
                    var khuyenMai = f["khuyenMai"];
                    var tenSP = f["tenSP"].Trim();
                    var donGia = f["donGia"];
                    var slTon = f["slTon"];
                    var hinh = f["hinhAnh"].Trim();
                    var namXB = f["namXB"];
                    var moTa = f["moTa"].Trim();
                    if (!String.IsNullOrEmpty(loaiSP) || !String.IsNullOrEmpty(tacGia) || !String.IsNullOrEmpty(nxb) || !String.IsNullOrEmpty(khuyenMai)
                        || !String.IsNullOrEmpty(tenSP) || !String.IsNullOrEmpty(hinh) || !String.IsNullOrEmpty(namXB) || !String.IsNullOrEmpty(moTa))
                    {
                        p.MaLoaiSP = int.Parse(loaiSP);
                        p.MaTacGia = int.Parse(tacGia);
                        p.MaNXB = int.Parse(nxb);
                        p.MaKhuyenMai = int.Parse(khuyenMai);
                        p.TenSanPham = tenSP;
                        p.DonGia = decimal.Parse(donGia);
                        p.SoLuongTon = int.Parse(slTon);
                        p.HinhAnh = hinh;
                        p.NamXB = int.Parse(namXB);
                        p.MoTa = moTa;
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Sửa sản phẩm thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriSanPham", "AdminSanPham");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        [HttpPost]
        public ActionResult ThemSanPham(FormCollection f)
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    var loaiSP = f["loaiSP"];
                    var tacGia = f["tacGia"];
                    var nxb = f["NXB"];
                    var khuyenMai = f["khuyenMai"];
                    var tenSP = f["tenSP"].Trim();
                    var donGia = f["donGia"];
                    var slTon = f["slTon"];
                    var hinh = f["hinhAnh"].Trim();
                    var namXB = f["namXB"];
                    var moTa = f["moTa"].Trim();
                    if (!String.IsNullOrEmpty(loaiSP) || !String.IsNullOrEmpty(tacGia) || !String.IsNullOrEmpty(nxb) || !String.IsNullOrEmpty(khuyenMai)
                        || !String.IsNullOrEmpty(tenSP) || !String.IsNullOrEmpty(hinh) || !String.IsNullOrEmpty(namXB) || !String.IsNullOrEmpty(moTa))
                    {
                        SANPHAM p = new SANPHAM();
                        p.MaLoaiSP = int.Parse(loaiSP);
                        p.MaTacGia = int.Parse(tacGia);
                        p.MaNXB = int.Parse(nxb);
                        p.MaKhuyenMai = int.Parse(khuyenMai);
                        p.TenSanPham = tenSP;
                        p.DonGia = decimal.Parse(donGia);
                        p.SoLuongTon = int.Parse(slTon);
                        p.HinhAnh = hinh;
                        p.NamXB = int.Parse(namXB);
                        p.MoTa = moTa;
                        db.SANPHAMs.InsertOnSubmit(p);
                        db.SubmitChanges();
                        TempData["ThongBao"] = "Thêm sản phẩm thành công";
                        TempData["LoaiTB"] = "alert-success";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("QuanTriSanPham", "AdminSanPham");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        //Thêm nhiều sản phẩm bằng Excel
        public ActionResult ThemSanPhamBangFileExcel(HttpPostedFileBase filePath)
        {
            if (filePath == null || filePath.ContentLength == 0)
            {
                return Json(new { success = false, message = "Không nhận được file!!" });
            }

            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    try
                    {
                        string tempFilePath = Path.Combine(Server.MapPath("~/App_Data/ExcelTemp"), Path.GetFileName(filePath.FileName));
                        filePath.SaveAs(tempFilePath);

                        Excel.Application excelApp = new Excel.Application();
                        Excel.Workbook workbook = excelApp.Workbooks.Open(tempFilePath);
                        Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

                        int rowCount = worksheet.UsedRange.Rows.Count;

                        for (int i = 2; i <= rowCount; i++)
                        {
                            bool re = false;
                            int maLoaiSP = int.Parse(((Excel.Range)worksheet.Cells[i, 1]).Text);
                            int maTacGia;
                            if (int.TryParse(((Excel.Range)worksheet.Cells[i, 2]).Text, out maTacGia))
                            {
                                re = true;
                            }
                            int maNXB;
                            if (int.TryParse(((Excel.Range)worksheet.Cells[i, 3]).Text, out maNXB))
                            {
                                re = true;
                            }
                            int maNCC;
                            if (int.TryParse(((Excel.Range)worksheet.Cells[i, 4]).Text, out maNCC))
                            {
                                re = true;
                            }
                            int maKhuyenMai = int.Parse(((Excel.Range)worksheet.Cells[i, 5]).Text);
                            string tenSanPham = ((Excel.Range)worksheet.Cells[i, 6]).Text;
                            decimal donGia = decimal.Parse(((Excel.Range)worksheet.Cells[i, 7]).Text);
                            string hinhAnh = ((Excel.Range)worksheet.Cells[i, 8]).Text;
                            string moTa = ((Excel.Range)worksheet.Cells[i, 9]).Text;
                            int soLuongTon = int.Parse(((Excel.Range)worksheet.Cells[i, 10]).Text);
                            int namXB;
                            if (int.TryParse(((Excel.Range)worksheet.Cells[i, 11]).Text, out namXB))
                            {
                                re = true;
                            }
                            string xuatXu = ((Excel.Range)worksheet.Cells[i, 12]).Text;

                            var sp = db.SANPHAMs.SingleOrDefault(x => x.TenSanPham.Equals(tenSanPham));

                            if (sp != null)
                            {
                                sp.SoLuongTon += soLuongTon;
                            }
                            else
                            {
                                if (re == true)
                                {
                                    SANPHAM sanPham = new SANPHAM
                                    {
                                        MaLoaiSP = maLoaiSP,
                                        MaTacGia = maTacGia,
                                        MaNXB = maNXB,                                       
                                        MaKhuyenMai = maKhuyenMai,
                                        TenSanPham = tenSanPham,
                                        DonGia = donGia,
                                        HinhAnh = hinhAnh,
                                        MoTa = moTa,
                                        SoLuongTon = soLuongTon,
                                        NamXB = namXB
                                    };
                                    db.SANPHAMs.InsertOnSubmit(sanPham);
                                }
                                else
                                {
                                    SANPHAM sanPham = new SANPHAM
                                    {
                                        MaLoaiSP = maLoaiSP,
                                        MaNCC = maNCC,
                                        MaKhuyenMai = maKhuyenMai,
                                        TenSanPham = tenSanPham,
                                        DonGia = donGia,
                                        HinhAnh = hinhAnh,
                                        MoTa = moTa,
                                        SoLuongTon = soLuongTon,
                                        XuatXu = xuatXu
                                    };
                                    db.SANPHAMs.InsertOnSubmit(sanPham);
                                }
                            }
                        }
                        db.SubmitChanges();

                        workbook.Close(false);
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                        return Json(new { success = true, message = "Nhập sản phẩm với Excel thành công" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Lỗi xử lý file Excel: " + ex.Message });
                    }
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

    }
}