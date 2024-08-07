using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Core.Resources;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Infrastructure.Repository
{
    /// <summary>
    /// Thực hiện các công việc ở IDepartmentRepository
    /// </summary>
    /// Created By: NKCanh - 07/08/2024
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IDBContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi sửa
        /// </summary>
        /// <param name="department">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        public Dictionary<string, string>? CheckDataValidate(Department department)
        {
            var errorData = new Dictionary<string, string>();

            //1. Kiểm tra các thông tin không được trống
            //Mã phòng ban
            if (string.IsNullOrEmpty(department.DepartmentCode))
            {
                errorData.Add("DepartmentCode", ResourceVN.Error_DepartmentCodeNotEmpty);
            }

            //Tên phòng ban
            if (string.IsNullOrEmpty(department.DepartmentName))
            {
                errorData.Add("DepartmentName", ResourceVN.Error_DepartmentNameNotEmpty);
            }

            //2. Kiểm tra dữ liệu hợp lệ
            //Tên không được có số
            if (department.DepartmentName.Any(char.IsDigit))
            {
                errorData.Add("DepartmentName", ResourceVN.Error_DepartmentNameNotNumber);
            }

            return errorData;
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm mới
        /// </summary>
        /// <param name="department">Dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        public Dictionary<string, string>? CheckDataValidateForInsert(Department department)
        {
            var errorData = CheckDataValidate(department);

            //Kiểm tra mã không được trùng
            bool checkCode = CheckCode(department.DepartmentCode);

            if (checkCode)
            {
                errorData.Add("DepartmentCode", ResourceVN.Error_DepartmentCodeDuplicated);
            }

            return errorData;
        }
    }
}
