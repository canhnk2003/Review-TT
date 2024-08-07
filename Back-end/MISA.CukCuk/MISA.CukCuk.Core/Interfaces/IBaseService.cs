using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Core.Interfaces
{
    /// <summary>
    /// Interface service bố
    /// </summary>
    /// <typeparam name="T">Kiểu đối tượng</typeparam>
    /// Created By: NKCanh - 06/08/2024
    public interface IBaseService<T> where T : class
    {
        /// <summary>
        /// Dịch vụ thêm
        /// </summary>
        /// <param name="data">Dữ liệu</param>
        /// <returns>Số bản ghi thêm thành công</returns>
        int InsertService(T data);

        /// <summary>
        /// Dịch vụ sửa
        /// </summary>
        /// <param name="data">Dữ liệu</param>
        /// <param name="id">id của bản ghi</param>
        /// <returns>Số bản ghi sửa thành công</returns>
        int UpdateService(T data, Guid id);

        /// <summary>
        /// Dịch vụ xóa
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Số bản ghi xóa thành công</returns>
        int DeleteService(Guid id);
    }
}
