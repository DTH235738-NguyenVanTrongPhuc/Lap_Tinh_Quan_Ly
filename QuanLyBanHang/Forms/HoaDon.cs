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
    }
}
