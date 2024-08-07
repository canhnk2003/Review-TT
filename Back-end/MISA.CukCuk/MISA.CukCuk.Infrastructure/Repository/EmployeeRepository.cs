using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Core.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.CukCuk.Infrastructure.Repository
{
    /// <summary>
    /// Thực hiện các công việc ở IEmployeeRepository
    /// </summary>
    /// Created By: NKCanh - 07/08/2024
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IDBContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi sửa
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>Danh sách lỗi</returns>
        public Dictionary<string, string>? CheckDataValidate(Employee employee)
        {
            var errorData = new Dictionary<string, string>();

            //1. Kiểm tra 1 số thông tin không được trống
            //1.1. Mã nhân viên không được để trống
            if (string.IsNullOrEmpty(employee.EmployeeCode))
            {
                errorData.Add("EmployeeCode", ResourceVN.Error_EmployeeCodeNotEmpty);
            }

            //1.2. Họ tên nhân viên không được để trống
            if (string.IsNullOrEmpty(employee.FullName))
            {
                errorData.Add("FullName", ResourceVN.Error_FullNameNotEmpty);
            }

            //1.3. Số CMTND không được để trống
            if (string.IsNullOrEmpty(employee.IdentityNumber))
            {
                errorData.Add("IdentityNumber", ResourceVN.Error_IdentityNumberNotEmpty);
            }

            //1.4. Số điện thoại không được để trống
            if (string.IsNullOrEmpty(employee.PhoneNumber))
            {
                errorData.Add("PhoneNumber", ResourceVN.Error_PhoneNumberNotEmpty);
            }

            //1.5. Email không được để trống
            if (string.IsNullOrEmpty(employee.Email))
            {
                errorData.Add("Email", ResourceVN.Error_EmailNotEmpty);
            }
            else
            {
                //Kiểm tra Email đúng định dạng
                if (CheckEmailValid(employee.Email) == false)
                {
                    errorData.Add("Email", ResourceVN.Error_ValidEmail);
                }
            }

            //2. Thực hiện validate dữ liệu
            //2.1. Họ tên không được có số
            if (employee.FullName.Any(char.IsDigit))
            {
                errorData.Add("FullName", ResourceVN.Error_EmployeeNameNotNumber);
            }

            //2.2. Số CMTND không được có chữ
            if (employee.IdentityNumber.Any(char.IsLetter))
            {
                errorData.Add("IdentityNumber", ResourceVN.Error_IdentityNumberNotLetter);
            }

            //2.3. Số điện thoại không được có chữ
            if (employee.PhoneNumber.Any(char.IsLetter))
            {
                errorData.Add("PhoneNumber", ResourceVN.Error_PhoneNumberNotLetter);
            }

            //2.4. Số điện thoại không được có chữ
            if (employee.LandlineNumber.Any(char.IsLetter))
            {
                errorData.Add("LandlineNumber", ResourceVN.Error_PhoneNumberNotLetter);
            }

            //2.5. Ngày sinh không được lớn hơn ngày hiện tại
            if (employee.DateOfBirth > DateTime.Now)
            {
                errorData.Add("DateOfBirth", ResourceVN.Error_BOfDateNotGreatNow);
            }

            //2.6. Ngày cấp không được lớn hơn ngày hiện tại
            if (employee.IdentityDate > DateTime.Now)
            {
                errorData.Add("IdentityDate", ResourceVN.Error_IdentityDateNotGreatNow);
            }

            return errorData;
        }

        /// <summary>
        /// Kiểm tra định dạng email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>
        /// true - không trùng
        /// false - trùng
        /// </returns>
        private bool CheckEmailValid(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if (Regex.IsMatch(email, pattern))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm
        /// </summary>
        /// <param name="employee">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        public Dictionary<string, string>? CheckDataValidateForInsert(Employee employee)
        {
            var errorData = CheckDataValidate(employee);

            bool checkCode = CheckCode(employee.EmployeeCode);

            bool checkIdentityNumber = CheckIdentityNumber(employee.IdentityNumber);

            //Mã không được trùng
            if (checkCode)
            {
                errorData.Add("EmployeeCode", ResourceVN.Error_EmployeeCodeDuplicated);
            }

            //Số CMTND, CCCD không được trùng
            if (checkIdentityNumber)
            {
                errorData.Add("IdentityNumber", ResourceVN.Error_IdentityNumberDuplicated);
            }

            return errorData;
        }

    }
}
