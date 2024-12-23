using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Models;
using PagedList;

namespace Website_QuanLyNhaSachNguyenVanCu.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext();

        public ActionResult DanhMucPartial()
        {
            var dsDM = db.DANHMUCs.OrderBy(x => x.MaDanhMuc).ToList();
            var dsLSP = db.LOAISANPHAMs.OrderBy(y => y.MaDanhMuc).ToList();

            var model = new DanhSachTheoSanPham
            {
                dsDanhMuc = dsDM,
                dsLoaiSanPham = dsLSP
            };
            return PartialView(model);
        }

        public ActionResult SanPham(int page = 1, int pageSize = 8)
        {
            var dsSP = db.SANPHAMs.OrderByDescending(x => x.MaSanPham).ToPagedList(page, pageSize);
            return View(dsSP);
        }

        public ActionResult ChiTietSanPham(int maSP)
        {
            var sp = db.SANPHAMs.Single(x => x.MaSanPham == maSP);
            return View(sp);
        }

        public ActionResult SanPhamTheoDanhMuc(int maDanhMuc, int page = 1, int pageSize = 8)
        {
            var dsSP = db.SANPHAMs.Where(x => x.LOAISANPHAM.DANHMUC.MaDanhMuc == maDanhMuc).OrderByDescending(y => y.MaSanPham).ToPagedList(page, pageSize);
            ViewBag.maDM = maDanhMuc;
            DANHMUC dm = db.DANHMUCs.Single(x => x.MaDanhMuc == maDanhMuc);
            ViewBag.Ten = dm.TenDanhMuc;
            return View(dsSP);
        }

        public ActionResult SanPhamTheoLoai(int maLSP, int page = 1, int pageSize = 8)
        {
            var dsSP = db.SANPHAMs.Where(x => x.LOAISANPHAM.MaLoaiSP == maLSP).OrderByDescending(y => y.MaSanPham).ToPagedList(page, pageSize);
            ViewBag.maL = maLSP;
            LOAISANPHAM loai = db.LOAISANPHAMs.Single(x => x.MaLoaiSP == maLSP);
            ViewBag.Ten = loai.TenLoaiSP;
            return View(dsSP);
        }

        public JsonResult HienThiGoiY(string search)
        {
            var goiY = db.SANPHAMs.Where(x => x.TenSanPham.Contains(search))
                .Select(x => new { label = x.TenSanPham, id = x.MaSanPham }).ToList();
            if (goiY.Count == 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(goiY, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchSanPham(string searchSP, int page = 1, int pageSize = 8)
        {
            if (searchSP == null)
                searchSP = "";
            if (searchSP.Equals(""))
            {
                var dsSP = db.SANPHAMs.OrderByDescending(x => x.MaSanPham).ToPagedList(page, pageSize);
                var slAll = db.SANPHAMs.OrderByDescending(x => x.MaSanPham).ToList();
                ViewBag.Search = searchSP;
                ViewBag.TongSL = slAll.Count;
                return View(dsSP);
            }
            var dsSPS = db.SANPHAMs.Where(x => x.TenSanPham.Contains(searchSP) || x.TACGIA.TenTacGia.Contains(searchSP)).OrderByDescending(y => y.MaSanPham).ToPagedList(page, pageSize);
            var slS = db.SANPHAMs.Where(x => x.TenSanPham.Contains(searchSP) || x.TACGIA.TenTacGia.Contains(searchSP)).OrderByDescending(x => x.MaSanPham).ToList();
            ViewBag.Search = searchSP;
            ViewBag.TongSL = slS.Count;
            return View(dsSPS);
        }



    }
}