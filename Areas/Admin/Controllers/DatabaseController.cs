using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_QuanLyNhaSachNguyenVanCu.Models;

namespace Website_QuanLyNhaSachNguyenVanCu.Areas.Admin.Controllers
{
    public class DatabaseController : Controller
    {
        // GET: Admin/Database

        public ActionResult Backup()
        {
            return View();
        }

        [HttpGet]
        public ActionResult BackupDatabase()
        {
            if (Session["TaiKhoan"] != null)
            {
                NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
                if (nv.Quyen.Equals("ADMIN"))
                {
                    try
                    {
                        using (QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext())
                        {
                            DateTime time = DateTime.Now;
                            string strTime = "" + time.ToString("yyyyMMdd") + "_" + time.Hour + "h_" + time.Minute + "p";
                            string backupFileName = "QuanLyNhaSach_" + strTime + ".bak";
                            string backupPath = Path.Combine(Server.MapPath("~/App_Data/Backup"), backupFileName);

                            db.ExecuteCommand("BACKUP DATABASE QUANLYNHASACH TO DISK = '" + backupPath + "'");
                            TempData["ThongBao"] = "Sao lưu cơ sở dữ liệu thành công";
                            TempData["LoaiTB"] = "alert-success";
                            TempData["ht"] = "block";
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ThongBao"] = "Sao lưu thất bại: " + ex.Message;
                        TempData["LoaiTB"] = "alert-danger";
                        TempData["ht"] = "block";
                    }
                }
                return RedirectToAction("Backup", "Database");
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }

        public ActionResult Restore()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RestoreDatabase(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
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
                        using (QuanLyNhaSachDataContext db = new QuanLyNhaSachDataContext())
                        {
                            DateTime time = DateTime.Now;
                            string strTime = "" + time.ToString("yyyyMMdd") + "_" + time.Hour + "h_" + time.Minute + "p";
                            string restorePath = Path.Combine(Server.MapPath("~/App_Data/Backup"), fileName);
                            string file_mdf = "QUANLYNHASACH_" + strTime + ".mdf";
                            string file_ldf = "QUANLYNHASACH_" + strTime + "_log.ldf";
                            string path_mdf = Path.Combine(Server.MapPath("~/App_Data/Restore"), file_mdf); ;
                            string path_ldf = Path.Combine(Server.MapPath("~/App_Data/Restore"), file_ldf); ;
                            db.ExecuteCommand("USE master RESTORE DATABASE NHASACH_" + strTime + " FROM DISK = '" + restorePath + "'WITH MOVE 'QUANLYNHASACH' TO '" + path_mdf + "', MOVE 'QUANLYNHASACH_log' TO '" + path_ldf + "'");

                            return Json(new { success = true, message = "Phục hổi cơ sở dữ liệu thành công" });
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Phục hổi cơ sở dữ liệu thất bại: " + ex.Message });
                    }
                }
            }
            return RedirectToAction("DangNhap", "AdminNguoiDung");
        }
    }
}