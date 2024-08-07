using MISA.CukCuk.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Core.Entities
{
    /// <summary>
    /// Thông tin nhân viên
    /// </summary>
    /// Created by: NKCanh - 30/07/2024
    public class Employee
    {
        #region Properties
        /// <summary>
        /// Khóa chính
        /// </summary>
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Họ tên
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Giới tính: 0-Nam, 1-Nữ, 2-Chưa xác định
        /// </summary>
        public Gender Gender { get; set; }
        public string? GenderName
        {
            get
            {
                switch (Gender)
                {
                    case (Gender.MALE):
                        return "Nam";
                    case (Gender.FEMALE):
                        return "Nữ";
                    case (Gender.OTHER):
                        return "Khác";
                    default:
                        return "Không xác định";
                }
            }
        }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Vị trí Id
        /// </summary>
        public Guid? PositionId { get; set; }

        /// <summary>
        /// Số CMTND, CCCD
        /// </summary>
        public string IdentityNumber { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? IdentityDate { get; set; }

        /// <summary>
        /// Phòng ban Id
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Nơi tạo
        /// </summary>
        public string? IdentityPlace { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Số điện thoại cố định
        /// </summary>
        public string? LandlineNumber { get; set; }

        /// <summary>
        /// Địa chỉ Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Tài khoản ngân hàng
        /// </summary>
        public string? BankAccount { get; set; }

        /// <summary>
        /// Tên ngân hàng
        /// </summary>
        public string? BankName { get; set; }

        /// <summary>
        /// Chi nhánh ngân hàng
        /// </summary>
        public string? Branch { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal Salary { get; set; }
        #endregion

    }
}
