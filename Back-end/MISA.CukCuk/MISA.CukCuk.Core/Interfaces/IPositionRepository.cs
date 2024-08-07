using MISA.CukCuk.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Core.Interfaces
{
    /// <summary>
    /// Interface PositionRepository
    /// </summary>
    /// Created By: NKCanh - 06/08/2024
    public interface IPositionRepository:IBaseRepository<Position>
    {
        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi sửa
        /// </summary>
        /// <param name="position">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        Dictionary<string, string>? CheckDataValidate(Position position);

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi xóa
        /// </summary>
        /// <param name="position">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        Dictionary<string, string>? CheckDataValidateForInsert(Position position);
    }
}
