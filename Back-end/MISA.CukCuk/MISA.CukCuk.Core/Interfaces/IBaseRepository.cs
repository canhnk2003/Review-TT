using MISA.CukCuk.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Core.Interfaces
{
    /// <summary>
    /// Interface Repository bố
    /// </summary>
    /// <typeparam name="T">Kiểu đối tượng</typeparam>
    /// Created By: NKCanh - 06/08/2024
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Lấy ra danh sách các bản ghi
        /// </summary>
        /// <returns>Danh sách các bản ghi</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Lấy ra 1 bản ghi với id tương ứng
        /// </summary>
        /// <param name="id">id của đối tượng lấy ra</param>
        /// <returns>Đối tượng tương ứng</returns>
        T? GetById(Guid id);

        /// <summary>
        /// Thêm mới 1 bản ghi
        /// </summary>
        /// <param name="data">Dữ liệu thêm</param>
        /// <returns>Số dữ liệu đã thêm thành công</returns>
        int Insert(T data);

        /// <summary>
        /// Cập nhật 1 bản ghi với id tương ứng
        /// </summary>
        /// <param name="data">Dữ liệu sửa</param>
        /// <param name="id">id của bản ghi</param>
        /// <returns>Số dữ liệu đã sửa thành công</returns>
        int Update(T data, Guid id);

        /// <summary>
        /// Xóa 1 bản ghi với id tương ứng
        /// </summary>
        /// <param name="id">id tương ứng</param>
        /// <returns>Số dữ liệu đã xóa thành công</returns>
        int Delete(Guid id);

        /// <summary>
        /// Xóa một vài bản ghi
        /// </summary>
        /// <param name="ids">Danh sách id các bản ghi</param>
        /// <returns>Số dữ liệu đã xóa thành công</returns>
        int DeleteAny(Guid[] ids);

        /// <summary>
        /// Kiểm tra Mã có trùng không
        /// </summary>
        /// <param name="code">Mã</param>
        /// <returns>
        /// True - trùng
        /// False - Không trùng
        /// </returns>
        bool CheckCode(string code);

        /// <summary>
        /// Kiểm tra Số CMTND-CCCD có trùng không
        /// </summary>
        /// <param name="identityNumber">Số CMTND-CCCD</param>
        /// <returns>
        /// True - trùng
        /// False - Không trùng
        /// </returns>
        bool CheckIdentityNumber(string identityNumber);

    }
}
