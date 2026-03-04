using ClosedXML.Excel;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHang.Forms
{
    public partial class HoaDon : Form
    {
        QLBHDbContext context = new QLBHDbContext();
        int id;
        public HoaDon()
        {
            InitializeComponent();
        }

        private void HoaDon_Load(object sender, EventArgs e)
        {
            dataGridView.AutoGenerateColumns = false;
            List<DanhSachHoaDon> hd = new List<DanhSachHoaDon>();
            hd = context.HoaDon.Select(r => new DanhSachHoaDon
            {
                ID = r.ID,
                NhanVienID = r.NhanVienID,
                HoVaTenNhanVien = r.NhanVien.HoVaTen,
                KhachHangID = r.KhachHangID,
                HoVaTenKhachHang = r.KhachHang.HoVaTen,
                NgayLap = r.NgayLap,
                GhiChuHoaDon = r.GhiChuHoaDon,
                TongTienHoaDon = r.HoaDon_ChiTiet.Sum(r => r.SoLuongBan * r.DonGiaBan),
                XemChiTiet = "Xem chi tiết"
            }).ToList();
            dataGridView.DataSource = hd;
        }

        private void btnLapHoaDon_Click(object sender, EventArgs e)
        {
            using (frmHoaDon_chitiet chiTiet = new frmHoaDon_chitiet())
            {
                chiTiet.ShowDialog();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            using (frmHoaDon_chitiet chiTiet = new frmHoaDon_chitiet())
            {
                chiTiet.ShowDialog();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa.");
                return;
            }

            int id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);

            if (MessageBox.Show("Xác nhận xóa hóa đơn này?",
                                "Xóa",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var hoaDon = context.HoaDon.Find(id);

                if (hoaDon != null)
                {
                    try
                    {
                        // Xóa chi tiết trước
                        var chiTiet = context.HoaDon_ChiTiet
                                             .Where(ct => ct.HoaDonID == id);

                        context.HoaDon_ChiTiet.RemoveRange(chiTiet);

                        // Xóa hóa đơn
                        context.HoaDon.Remove(hoaDon);

                        context.SaveChanges();

                        MessageBox.Show("Xóa thành công.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                    }
                }

                HoaDon_Load(null, null);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnnhap_Click(object sender, EventArgs e)
        { 
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Nhập dữ liệu Hóa Đơn từ Excel";
            openFileDialog.Filter = "Tập tin Excel|*.xlsx";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook workbook = new XLWorkbook(openFileDialog.FileName))
                    {
                        // ================= SHEET HoaDon =================
                        var sheetHoaDon = workbook.Worksheet("HoaDon");
                        foreach (var row in sheetHoaDon.RowsUsed().Skip(1))
                        {
                            var hd = new QuanLyBanHang.Data.Entity.HoaDon();
                            hd.ID = int.Parse(row.Cell(1).Value.ToString());
                            hd.NhanVienID = int.Parse(row.Cell(2).Value.ToString());
                            hd.KhachHangID = int.Parse(row.Cell(3).Value.ToString());
                            hd.NgayLap = DateTime.Parse(row.Cell(4).Value.ToString());
                            hd.GhiChuHoaDon = row.Cell(5).Value.ToString();

                            context.HoaDon.Add(hd);
                        }

                        context.SaveChanges();

                        // ================= SHEET HoaDon_ChiTiet =================
                        var sheetChiTiet = workbook.Worksheet("HoaDon_ChiTiet");
                        foreach (var row in sheetChiTiet.RowsUsed().Skip(1))
                        {
                            var ct = new HoaDon_ChiTiet();
                            ct.ID = int.Parse(row.Cell(1).Value.ToString());
                            ct.HoaDonID = int.Parse(row.Cell(2).Value.ToString());
                            ct.SanPhamID = int.Parse(row.Cell(3).Value.ToString());
                            ct.SoLuongBan = int.Parse(row.Cell(4).Value.ToString());
                            ct.DonGiaBan = int.Parse(row.Cell(5).Value.ToString());

                            context.HoaDon_ChiTiet.Add(ct);
                        }

                        context.SaveChanges();
                    }

                    MessageBox.Show("Nhập dữ liệu thành công!",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    HoaDon_Load(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        

        private void btnXuat_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Xuất dữ liệu Hóa Đơn ra Excel";
            saveFileDialog.Filter = "Tập tin Excel|*.xlsx";
            saveFileDialog.FileName = "HoaDon_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        // ================= SHEET 1: HoaDon =================
                        DataTable dtHoaDon = new DataTable();
                        dtHoaDon.Columns.Add("ID", typeof(int));
                        dtHoaDon.Columns.Add("NhanVienID", typeof(int));
                        dtHoaDon.Columns.Add("KhachHangID", typeof(int));
                        dtHoaDon.Columns.Add("NgayLap", typeof(DateTime));
                        dtHoaDon.Columns.Add("GhiChuHoaDon", typeof(string));

                        var hoaDons = context.HoaDon.ToList();
                        foreach (var hd in hoaDons)
                        {
                            dtHoaDon.Rows.Add(
                                hd.ID,
                                hd.NhanVienID,
                                hd.KhachHangID,
                                hd.NgayLap,
                                hd.GhiChuHoaDon
                            );
                        }

                        wb.Worksheets.Add(dtHoaDon, "HoaDon");


                        // ================= SHEET 2: HoaDon_ChiTiet =================
                        DataTable dtChiTiet = new DataTable();
                        dtChiTiet.Columns.Add("ID", typeof(int));
                        dtChiTiet.Columns.Add("HoaDonID", typeof(int));
                        dtChiTiet.Columns.Add("SanPhamID", typeof(int));
                        dtChiTiet.Columns.Add("SoLuongBan", typeof(int));
                        dtChiTiet.Columns.Add("DonGiaBan", typeof(double));

                        var chiTiets = context.HoaDon_ChiTiet.ToList();
                        foreach (var ct in chiTiets)
                        {
                            dtChiTiet.Rows.Add(
                                ct.ID,
                                ct.HoaDonID,
                                ct.SanPhamID,
                                ct.SoLuongBan,
                                ct.DonGiaBan
                            );
                        }

                        wb.Worksheets.Add(dtChiTiet, "HoaDon_ChiTiet");

                        wb.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Xuất dữ liệu thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
    
}
