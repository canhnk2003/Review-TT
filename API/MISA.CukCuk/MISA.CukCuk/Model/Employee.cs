namespace MISA.CukCuk.Model
{
    /// <summary>
    /// Lớp Nhân viên
    /// Created By: NKCanh - 20/07/2024
    /// </summary>
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
        public int? Gender { get; set; }
        public string? GenderName
        {
            get
            {
                if (Gender == 0)
                {
                    return "Nam";
                }
                if (Gender == 1)
                {
                    return "Nữ";
                }
                if (Gender == 2)
                {
                    return "Chưa xác định";
                }
                return null;
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
        #endregion

        #region Constructor

        /// <summary>
        /// Không có tham số
        /// </summary>
        public Employee() { }

        /// <summary>
        /// Đầy đủ tham số
        /// </summary>
        /// <param name="employeeId">Nhân viên Id, khóa chính</param>
        /// <param name="employeeCode">Mã nhân viên</param>
        /// <param name="fullName">Họ tên</param>
        /// <param name="gender">Giới tính</param>
        /// <param name="dateOfBirth">Ngày sinh</param>
        /// <param name="positionId">Vị trí Id</param>
        /// <param name="identityNumber">Số CMTND, CCCD</param>
        /// <param name="identityDate">Ngày cấp</param>
        /// <param name="departmentId">Phòng ban Id</param>
        /// <param name="identityPlace">Nơi cấp</param>
        /// <param name="address">Địa chỉ</param>
        /// <param name="phoneNumber">Số điện thoại</param>
        /// <param name="email">Email</param>
        /// <param name="bankAccount">Tài khoản ngân hàng</param>
        /// <param name="bankName">Tên ngân hàng</param>
        /// <param name="branch">Chi nhánh ngân hàng</param>
        public Employee(Guid employeeId, string employeeCode, string fullName, int? gender, DateTime? dateOfBirth, Guid? positionId, string identityNumber, DateTime? identityDate, Guid? departmentId, string? identityPlace, string? address, string phoneNumber, string email, string? bankAccount, string? bankName, string? branch)
        {
            EmployeeId = employeeId;
            EmployeeCode = employeeCode;
            FullName = fullName;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            PositionId = positionId;
            IdentityNumber = identityNumber;
            IdentityDate = identityDate;
            DepartmentId = departmentId;
            IdentityPlace = identityPlace;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
            BankAccount = bankAccount;
            BankName = bankName;
            Branch = branch;
        }

        /// <summary>
        /// 6 tham số bắt buộc
        /// </summary>
        /// <param name="employeeId">Nhân viên Id, khóa chính</param>
        /// <param name="employeeCode">Mã nhân viên</param>
        /// <param name="fullName">Họ tên</param>
        /// <param name="identityNumber">Số CMTND, CCCD</param>
        /// <param name="phoneNumber">Số điện thoại</param>
        /// <param name="email">Email</param>
        public Employee(Guid employeeId, string employeeCode, string fullName, string identityNumber, string phoneNumber, string email)
        {
            EmployeeId = employeeId;
            EmployeeCode = employeeCode;
            FullName = fullName;
            IdentityNumber = identityNumber;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        #endregion
    }

}
