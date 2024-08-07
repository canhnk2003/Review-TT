using MISA.CukCuk.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Core.Interfaces
{
    /// <summary>
    /// Interface EmployeeRepository
    /// </summary>
    /// Created By: NKCanh - 06/08/2024
    public interface IEmployeeRepository:IBaseRepository<Employee>
    {
        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi sửa
        /// </summary>
        /// <param name="employee">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        Dictionary<string, string>? CheckDataValidate(Employee employee);

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm mới
        /// </summary>
        /// <param name="employee">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        Dictionary<string, string>? CheckDataValidateForInsert(Employee employee);
    }
}
