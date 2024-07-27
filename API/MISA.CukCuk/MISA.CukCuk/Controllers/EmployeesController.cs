using Dapper;
using Microsoft.AspNetCore.Mvc;
using MISA.CukCuk.Model;
using MySqlConnector;
using System.Text.RegularExpressions;

namespace MISA.CukCuk.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        /// <summary>
        /// Lấy ra danh sách nhân viên
        /// </summary>
        /// <returns>
        /// 200 - Lấy ra danh sách thành công
        /// 500 - Badrequest, Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 26/07/2024
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                //1. Khai báo chuỗi kết nối
                string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                    "Database = HAUI_2021604405_NguyenKhacCanh; " +
                    "User Id = manhnv; Password = 12345678";

                //2. Khởi tạo chuỗi kết nối
                MySqlConnection connection = new MySqlConnection(connectionString);

                //3. Truy vấn dữ liệu
                var sqlText = "SELECT * FROM Employee";

                //4. Lấy dữ liệu
                var employees = connection.Query<Employee>(sqlText);

                //5. Trả kết quả về client
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Xử lý lỗi ngoại lệ phía server
        /// </summary>
        /// <param name="ex">Thông báo lỗi</param>
        /// <returns>Mã lỗi 500 - Badrequest</returns>
        /// Created by: NKCanh - 26/07/2024
        private IActionResult ErrorException(Exception ex)
        {
            //Tạo mới 1 đối tượng lỗi
            ErrorService e = new ErrorService();

            //Gán các thuộc tính với thông báo lỗi
            e.DevMsg = ex.Message;
            e.UserMsg = Resources.ResourceVN.Error_Exception;
            e.Data = ex.Data;

            //Trả về dữ liệu
            return BadRequest(e);
        }

        /// <summary>
        /// Lấy ra 1 đối tượng với id tương ứng
        /// </summary>
        /// <param name="EmployeeId">id của nhân viên truyền vào trên router</param>
        /// <returns>
        /// 200 - Lấy ra thành công
        /// 404 - Lỗi không tìm thấy
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 26/07/2024
        [HttpGet("{EmployeeId}")]
        public IActionResult Get([FromRoute] Guid EmployeeId)
        {
            try
            {
                //1. Kiểm tra có trong database không
                var checkEmployeeById = CheckEmployeeId(EmployeeId);

                //2. Nếu không thì thông báo lỗi, nếu có thì tra về employee tương ứng
                if (checkEmployeeById == null)
                {
                    return ErrorNotFound();
                }
                else
                {
                    return Ok(checkEmployeeById);
                }
            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Kiểm tra xem có trong database không
        /// </summary>
        /// <param name="employeeId">id nhân viên</param>
        /// <returns>Nhân viên theo id</returns>
        /// Created by: NKCanh - 26/07/2024
        private Employee? CheckEmployeeId(Guid employeeId)
        {
            //1. Khai báo chuỗi kết nối
            string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                "Database = HAUI_2021604405_NguyenKhacCanh; " +
                "User Id = manhnv; Password = 12345678";

            //2. Khởi tạo chuỗi kết nối
            MySqlConnection connection = new MySqlConnection(connectionString);

            //3. Truy vấn dữ liệu
            var sqlText = "SELECT * FROM Employee WHERE EmployeeId = @EmployeeId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", employeeId);

            //4. Lấy dữ liệu
            var employeeById = connection.QueryFirstOrDefault<Employee>(sqlText, parameters);

            //5. Trả kết quả về client
            return employeeById;
        }

        /// <summary>
        /// Xử lý lỗi không tìm thấy dữ liệu
        /// </summary>
        /// <returns>Mã lỗi 404 - NotFound</returns>
        /// Created by: NKCanh - 26/07/2024
        private IActionResult ErrorNotFound()
        {
            ErrorService e = new ErrorService();
            e.UserMsg = Resources.ResourceVN.Error_NotFound;
            return NotFound(e);
        }

        /// <summary>
        /// Thêm mới 1 đối tượng nhân viên
        /// </summary>
        /// <param name="employee">Tham số đầu vào, được nhập từ Body</param>
        /// <returns>
        /// 201 - Tạo thành công
        /// 200 - Tạo không thành công
        /// 500 - Lỗi phía Server
        /// </returns>
        /// Created by: NKCanh - 26/07/2024
        [HttpPost]
        public IActionResult Post([FromBody] Employee employee)
        {
            try
            {
                //1. Kiểm tra dữ liệu hợp lệ
                Dictionary<string, string> errorData = CheckData(employee);

                //Kiểm tra mã nhân viên có bị trùng không
                if (CheckEmployeeCode(employee.EmployeeCode))
                {
                    errorData.Add("EmployeeCode", Resources.ResourceVN.Error_EmployeeCodeDuplicated);
                }

                //2. Nếu dữ liệu không hợp lệ trả về danh sách lỗi, hợp lệ thì cho phép tạo
                if (errorData.Count > 0)
                {
                    //2.1. Không hợp lệ trả về lỗi
                    return ErrorInvalid(errorData);
                }
                else
                {
                    //2.2. Hợp lệ thì tạo mới nhân viên
                    //Tạo mới 1 id cho nhân viên
                    employee.EmployeeId = Guid.NewGuid();

                    //Khai bao chuỗi kết nối
                    string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                                        "Database = HAUI_2021604405_NguyenKhacCanh; " +
                                        "User Id = manhnv; Password = 12345678";

                    //Khởi tạo chuỗi kết nối
                    MySqlConnection connection = new MySqlConnection(connectionString);

                    //Truy vấn thêm dữ liệu
                    var sqlText = "Proc_AddEmployee";

                    //Thực hiện thêm mới
                    int res = AddAndEditData(employee, connection, sqlText);

                    if (res > 0)
                    {
                        return StatusCode(201, res);
                    }
                    else
                    {
                        ErrorService error = new ErrorService();
                        error.UserMsg = Resources.ResourceVN.Error_Create;
                        return Ok(error);
                    }
                }
            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Thêm mới hoặc sửa 1 nhân viên
        /// </summary>
        /// <param name="employee">Dữ liệu nhân viên</param>
        /// <param name="connection">Chuỗi kết nối</param>
        /// <param name="sqlText">Chuỗi truy vấn</param>
        /// <returns>Số bản ghi được thêm hoặc sửa</returns>
        /// Created by: NKCanh - 26/07/2024
        private int AddAndEditData(Employee employee, MySqlConnection connection, string sqlText)
        {
            //Mở chuỗi kết nối
            connection.Open();

            //Đọc các tham số đầu vào từ store
            var sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = sqlText;
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            MySqlCommandBuilder.DeriveParameters(sqlCommand);

            DynamicParameters parameters = new DynamicParameters();
            foreach (MySqlParameter param in sqlCommand.Parameters)
            {
                var paramName = param.ParameterName;

                //Tên thuộc tính
                var propName = paramName.Replace("@m_", "");

                var entityProperty = employee.GetType().GetProperty(propName);
                if (entityProperty != null)
                {
                    var propValue = entityProperty.GetValue(employee);

                    //Gán giá trị cho các param
                    parameters.Add(paramName, propValue);
                }
                else
                {
                    parameters.Add(paramName, null);
                }
            }
            int res = connection.Execute(sql: sqlText, param: parameters);

            return res;
        }

        /// <summary>
        /// Kiểm tra mã nhân viên có bị trùng không
        /// </summary>
        /// <param name="employeeCode">Mã nhân viên</param>
        /// <returns>
        /// true - bị trùng
        /// false - không trùng
        /// </returns>
        /// Created by: NKCanh - 26/07/2024
        private bool CheckEmployeeCode(string employeeCode)
        {
            //1. Khai báo chuỗi kết nối
            string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                "Database = HAUI_2021604405_NguyenKhacCanh; " +
                "User Id = manhnv; Password = 12345678";

            //2. Khởi tạo chuỗi kết nối
            MySqlConnection connection = new MySqlConnection(connectionString);

            //3. Truy vấn dữ liệu
            var sqlText = "SELECT EmployeeCode FROM Employee WHERE EmployeeCode = @EmployeeCode";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EmployeeCode", employeeCode);

            //4. Lấy dữ liệu
            var res = connection.QueryFirstOrDefault<string>(sqlText, parameters);

            //5. Trả về kết quả
            if (res != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Xử lý lỗi không hợp lệ dữ liệu
        /// </summary>
        /// <param name="errorData">Danh sách lỗi</param>
        /// <returns>Lỗi</returns>
        /// Created by: NKCanh - 27/07/2024
        private IActionResult ErrorInvalid(Dictionary<string, string> errorData)
        {
            ErrorService errorService = new ErrorService();
            errorService.UserMsg = Resources.ResourceVN.Error_ValidData;
            errorService.Data = errorData;
            return BadRequest(errorService);
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ
        /// </summary>
        /// <param name="e">Đối tượng truyền vào</param>
        /// <returns>Danh sách lỗi</returns>
        /// Created by: NKCanh - 27/07/2024
        private Dictionary<string, string> CheckData(Employee e)
        {
            var errorData = new Dictionary<string, string>();

            //1. Kiểm tra 1 số thông tin không được trống
            //1.1. Mã nhân viên không được để trống
            if (string.IsNullOrEmpty(e.EmployeeCode))
            {
                errorData.Add("EmployeeCode", Resources.ResourceVN.Error_EmployeeCodeNotEmpty);
            }

            //1.2. Họ tên nhân viên không được để trống
            if (string.IsNullOrEmpty(e.FullName))
            {
                errorData.Add("FullName", Resources.ResourceVN.Error_FullNameNotEmpty);
            }

            //1.3. Số CMTND không được để trống
            if (string.IsNullOrEmpty(e.IdentityNumber))
            {
                errorData.Add("IdentityNumber", Resources.ResourceVN.Error_IdentityNumberNotEmpty);
            }

            //1.4. Số điện thoại không được để trống
            if (string.IsNullOrEmpty(e.PhoneNumber))
            {
                errorData.Add("PhoneNumber", Resources.ResourceVN.Error_PhoneNumberNotEmpty);
            }

            //1.5. Email không được để trống
            if (string.IsNullOrEmpty(e.Email))
            {
                errorData.Add("Email", Resources.ResourceVN.Error_EmailNotEmpty);
            }
            else
            {
                //Kiểm tra Email đúng định dạng
                if (CheckEmailValid(e.Email) == false)
                {
                    errorData.Add("Email", Resources.ResourceVN.Error_ValidEmail);
                }
            }

            //2. Thực hiện validate dữ liệu
            //2.1. Họ tên không được có số
            if (e.FullName.Any(char.IsDigit))
            {
                errorData.Add("FullName", Resources.ResourceVN.Error_EmployeeNameNotNumber);
            }

            //2.2. Số CMTND không được có chữ
            if (e.IdentityNumber.Any(char.IsLetter))
            {
                errorData.Add("IdentityNumber", Resources.ResourceVN.Error_IdentityNumberNotLetter);
            }

            //2.3. Số điện thoại không được có chữ
            if (e.PhoneNumber.Any(char.IsLetter))
            {
                errorData.Add("PhoneNumber", Resources.ResourceVN.Error_PhoneNumberNotLetter);
            }

            //2.4. Số điện thoại không được có chữ
            if (e.LandlineNumber.Any(char.IsLetter))
            {
                errorData.Add("LandlineNumber", Resources.ResourceVN.Error_PhoneNumberNotLetter);
            }

            //2.5. Ngày sinh không được lớn hơn ngày hiện tại
            if (e.DateOfBirth > DateTime.Now)
            {
                errorData.Add("DateOfBirth", Resources.ResourceVN.Error_BOfDateNotGreatNow);
            }

            //2.6. Ngày cấp không được lớn hơn ngày hiện tại
            if (e.IdentityDate > DateTime.Now)
            {
                errorData.Add("IdentityDate", Resources.ResourceVN.Error_IdentityDateNotGreatNow);
            }

            return errorData;
        }

        /// <summary>
        /// Kiểm tra Email có đúng định dạng không
        /// </summary>
        /// <param name="email">Tham số đầu vào</param>
        /// <returns>
        /// true - nếu đúng định dạng
        /// false - nếu sai định dạng
        /// </returns>
        /// Created by: NKCanh - 27/07/2024
        private bool CheckEmailValid(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if (Regex.IsMatch(email, pattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sửa thông tin 1 nhân viên với id truyền vào
        /// </summary>
        /// <param name="EmployeeId">id truyền vào từ router</param>
        /// <param name="e">truyền vào 1 đối tượng nhân viên</param>
        /// <returns>
        /// 201 - Sửa thành công
        /// 200 - Sửa thất bại
        /// 404 - Lỗi không tìm thấy
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 27/07/2024
        [HttpPut("{EmployeeId}")]
        public IActionResult PUT([FromRoute] Guid EmployeeId, [FromBody] Employee employee)
        {
            try
            {
                //1. Kiểm tra xem có nhân viên với id tương ứng có trong danh sách không
                var checkEmployeeById = CheckEmployeeId(EmployeeId);

                //2. Nếu có thì cho phép sửa, nếu không thì thông báo không có
                if (checkEmployeeById != null)
                {
                    //Thực hiện validate dữ liệu hợp lệ
                    var errorData = CheckData(employee);

                    if (errorData.Count > 0)
                    {
                        return ErrorInvalid(errorData);
                    }
                    else
                    {
                        //Lấy id cũ
                        employee.EmployeeId = EmployeeId;

                        //2.1. Khai báo chuỗi kết nối
                        string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                                       "Database = HAUI_2021604405_NguyenKhacCanh; " +
                                       "User Id = manhnv; Password = 12345678";

                        //2.2. Khởi tạo chuỗi kết nối
                        MySqlConnection connection = new MySqlConnection(connectionString);

                        //2.3. Truy vấn sửa
                        var sqlText = "Proc_UpdateEmployee";

                        //2.4. Cập nhật thông tin
                        var res = AddAndEditData(connection: connection, employee: employee, sqlText: sqlText);

                        if(res > 0)
                        {
                            return StatusCode(201, res);
                        }
                        else
                        {
                            ErrorService error = new ErrorService();
                            error.UserMsg = Resources.ResourceVN.Error_Edit;
                            return Ok(error);
                        }
                    }
                }
                else
                {
                    return ErrorNotFound();
                }
            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Xóa một nhân viên với id tương ứng
        /// </summary>
        /// <param name="EmployeeId">id truyền vào từ router</param>
        /// <returns>
        /// 201 - Xóa thành công
        /// 200 - Xóa không thành công
        /// 404 - Lỗi không tìm thấy
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 27/07/2024
        [HttpDelete("{EmployeeId}")]
        public IActionResult DELETE([FromRoute] Guid EmployeeId)
        {
            try
            {
                //1. Kiểm tra xem có nhân viên với id tương ứng có trong danh sách không
                var checkEmployeeById = CheckEmployeeId(EmployeeId);

                //2. Trả về mã lỗi tương ứng
                if (checkEmployeeById != null)
                {
                    //2.1. Khai báo chuỗi kết nối
                    string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                    "Database = HAUI_2021604405_NguyenKhacCanh; " +
                    "User Id = manhnv; Password = 12345678";

                    //2.2. Khởi tạo chuỗi kết nối
                    MySqlConnection connection = new MySqlConnection (connectionString);

                    //2.3. Truy vấn xóa
                    var sqlText = "DELETE Employee FROM Employee WHERE EmployeeId = @EmployeeId";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@EmployeeId", EmployeeId);

                    //2.4. Xóa dữ liệu
                    int res = connection.Execute(sqlText, parameters);

                    //2.5. Trả về kết quả cho client
                    if (res > 0)
                    {
                        return StatusCode(201, res);
                    }
                    else
                    {
                        ErrorService error = new ErrorService();
                        error.UserMsg = Resources.ResourceVN.Error_Delete;
                        return Ok(error);
                    }
                }
                else
                {
                    return ErrorNotFound();
                }
            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }
    }
}
