using MISA.CukCuk.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Core.Interfaces
{
    /// <summary>
    /// Interface DepartmentRepository
    /// </summary>
    /// Created By: NKCanh - 06/08/2024
    public interface IDepartmentRepository:IBaseRepository<Department>
    {
        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi sửa
        /// </summary>
        /// <param name="department">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        Dictionary<string, string>? CheckDataValidate(Department department);

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm mới
        /// </summary>
        /// <param name="department">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        Dictionary<string, string>? CheckDataValidateForInsert(Department department);
    }
}
