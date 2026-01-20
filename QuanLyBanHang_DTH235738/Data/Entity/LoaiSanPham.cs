using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanHang.Data.Entity
{
    internal class LoaiSanPham
    {
        public int ID { get; set; }
        public string? TenLoai { get; set; }
        public virtual ObservableCollectionListSource<SanPham> SanPhams { get; } = new();

    }
}
