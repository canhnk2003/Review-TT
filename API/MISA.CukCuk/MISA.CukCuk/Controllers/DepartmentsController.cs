using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.CukCuk.Model;
using MySqlConnector;

namespace MISA.CukCuk.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        /// <summary>
        /// Lấy ra tất cả phòng ban
        /// </summary>
        /// <returns>
        /// 200 - Lấy ra thành công
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 25/07/2024
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
                var connection = new MySqlConnection(connectionString);

                //3. Truy vấn dữ liệu
                var sql = "SELECT * FROM Department";

                //4. Lấy dữ liệu
                var departments = connection.Query<Department>(sql: sql);

                //5. Trả về kết quả
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Lấy ra phòng ban theo id
        /// </summary>
        /// <param name="DepartmentId">id muốn lấy ra</param>
        /// <returns>
        /// 200 - Lấy ra thành công
        /// 404 - Không tìm thấy
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 25/07/2024
        [HttpGet("{DepartmentId}")]
        public IActionResult Get([FromRoute] Guid DepartmentId)
        {
            try
            {
                //1. Kiểm tra xem có trong database không
                var departmentById = CheckDepartmentById(DepartmentId);

                if (departmentById == null)
                {
                    return ErrorNotFound();
                }
                else
                {
                    return Ok(departmentById);
                }
            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Thêm mới 1 phòng ban
        /// </summary>
        /// <param name="department">Dữ liệu</param>
        /// <returns>
        /// 201 - Thêm thành công
        /// 200 - Thêm không thành công
        /// 500 - Lỗi phía server
        /// </returns>
        [HttpPost]
        public IActionResult POST([FromBody] Department department)
        {
            try
            {
                //1. Check dữ liệu hợp lệ
                Dictionary<string, string> errorData = CheckData(department);

                //Kiểm tra mã có bị trùng không
                if (CheckDepartmentCode(department.DepartmentCode))
                {
                    errorData.Add("DepartmentCode", Resources.ResourceVN.Error_DepartmentCodeDuplicated);
                }

                //2. Nếu không hợp lệ trả về lỗi, hợp lệ thì cho phép thêm mới
                if (errorData.Count > 0)
                {
                    return ErrorInvalid(errorData);
                }
                else
                {
                    //Tạo mới 1 id
                    department.DepartmentId = Guid.NewGuid();

                    //2.1. Khai báo chuỗi kết nối
                    string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                                        "Database = HAUI_2021604405_NguyenKhacCanh; " +
                                        "User Id = manhnv; Password = 12345678";

                    //2.2. Khởi tạo chuỗi
                    MySqlConnection connection = new MySqlConnection(connectionString);

                    //2.3. Chuỗi truy vấn thêm dữ liệu
                    var sql = "Proc_AddDepartment";

                    //2.4. Thêm dữ liệu
                    int res = AddAndEditData(department, connection, sql);
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
        /// Sửa dữ liệu theo id
        /// </summary>
        /// <param name="DepartmentId">id của phòng ban muốn sửa</param>
        /// <param name="department">dữ liệu</param>
        /// <returns>
        /// 201 - Sửa thành công
        /// 200 - sửa không thành công
        /// 404 - Không có id trong database
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 26/07/2024
        [HttpPut("{DepartmentId}")]
        public IActionResult PUT([FromRoute] Guid DepartmentId, [FromBody] Department department)
        {
            try
            {
                //1. Kiểm tra có id trong database không
                var checkDepartmentById = CheckDepartmentById(DepartmentId);

                //2. Nếu có thì cho phép sửa, nếu không thì thông báo không có
                if (checkDepartmentById == null)
                {
                    return ErrorNotFound();
                }
                else
                {
                    //Kiểm tra dữ liệu hợp lệ
                    var errorData = CheckData(department);

                    //Hợp lệ cho sửa, không hợp lệ thông báo lỗi
                    if (errorData.Count > 0)
                    {
                        return ErrorInvalid(errorData);
                    }
                    else
                    {
                        //Lấy id cũ
                        department.DepartmentId = DepartmentId;

                        //2.1. Khai báo chuỗi kết nối
                        string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                                            "Database = HAUI_2021604405_NguyenKhacCanh; " +
                                            "User Id = manhnv; Password = 12345678";

                        //2.2. Khởi tạo chuỗi kết nối
                        var connection = new MySqlConnection(connectionString);

                        //2.3. Chuỗi truy vấn sửa dữ liệu
                        var sqlText = "Proc_UpdateDepartment";

                        //2.4. Sửa dữ liệu
                        var res = AddAndEditData(department: department, connection: connection, sql: sqlText);

                        if(res > 0 )
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

            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Xóa dữ liệu theo id
        /// </summary>
        /// <param name="DepartmentId">id phòng ban muốn xóa</param>
        /// <returns>
        /// 201 - Xóa thành công
        /// 200 - Xóa không thành công
        /// 404 - Không có trong database
        /// 500 - Lỗi phía server
        /// </returns>
        [HttpDelete("{DepartmentId}")]
        public IActionResult DELETE([FromRoute] Guid DepartmentId)
        {
            try
            {
                //1. Kiểm tra có trong database không
                var checkDepartmentById = CheckDepartmentById(DepartmentId);

                //2. Nếu có cho phép xóa, nếu không thông báo lỗi
                if(checkDepartmentById == null)
                {
                    return ErrorNotFound();
                }
                else
                {
                    //1. Khai báo chuỗi kết nối
                    string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                    "Database = HAUI_2021604405_NguyenKhacCanh; " +
                    "User Id = manhnv; Password = 12345678";

                    //2. Khởi tạo chuỗi kết nối
                    MySqlConnection connection = new MySqlConnection(connectionString);

                    //3. Chuỗi truy vấn xóa
                    var sqlText = "DELETE Department FROM Department WHERE DepartmentId = @DepartmentId";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@DepartmentId", DepartmentId);

                    //4. Xóa dữ liệu
                    int res = connection.Execute(sqlText, parameters);

                    if(res > 0)
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
            }
            catch(Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Thêm hoặc sửa 1 phòng ban
        /// </summary>
        /// <param name="department">Dữ liệu của đối tượng</param>
        /// <param name="connection">Kết nối</param>
        /// <param name="sql">Chuỗi truy vấn</param>
        /// <returns>
        /// Số bản ghi đã thêm hoặc sửa được
        /// </returns>
        /// Created by: NKCanh - 25/07/2024
        private int AddAndEditData(Department department, MySqlConnection connection, string sql)
        {
            //Mở kết nối đến database
            connection.Open();

            //Đọc các tham số đầu vào của store
            var sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            MySqlCommandBuilder.DeriveParameters(sqlCommand);

            DynamicParameters parameters = new DynamicParameters();
            foreach (MySqlParameter param in sqlCommand.Parameters)
            {
                //Tên của tham số
                var paramName = param.ParameterName;
                var propName = paramName.Replace("@m_", "");
                var entityProperty = department.GetType().GetProperty(propName);
                if (entityProperty != null)
                {
                    var propValue = entityProperty.GetValue(department);

                    //Thực hiện gán giá trị cho các param
                    parameters.Add(paramName, propValue);
                }
                else
                {
                    parameters.Add(paramName, null);
                }
            }

            //Thêm hoặc sửa dữ liệu
            int res = connection.Execute(sql: sql, param: parameters);

            return res;
        }

        /// <summary>
        /// Hiển thị thông báo lỗi dữ liệu
        /// </summary>
        /// <param name="errorData">Danh sách lỗi</param>
        /// <returns>400 - Badreques</returns>
        /// Created by: NKCanh - 25/07/2024
        private IActionResult ErrorInvalid(Dictionary<string, string> errorData)
        {
            ErrorService error = new ErrorService();
            error.UserMsg = Resources.ResourceVN.Error_ValidData;
            error.Data = errorData;
            return BadRequest(error);
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ
        /// </summary>
        /// <param name="department">Dữ liệu cần kiểm tra</param>
        /// <returns>Danh sách lỗi</returns>
        /// Created by: NKCanh - 25/07/2024
        private Dictionary<string, string> CheckData(Department department)
        {
            Dictionary<string, string> errorData = new Dictionary<string, string>();

            //1. Kiểm tra 1 số thông tin không được trống
            //1.1. DepartmentCode
            if (string.IsNullOrEmpty(department.DepartmentCode))
            {
                errorData.Add("DepartmentCode", Resources.ResourceVN.Error_DepartmentCodeNotEmpty);
            }

            //1.2. DepartmentName
            if (string.IsNullOrEmpty(department.DepartmentName))
            {
                errorData.Add("DepartmentName", Resources.ResourceVN.Error_DepartmentNameNotEmpty);
            }

            //2. Kiểm tra dữ liệu hợp lệ
            // Tên phòng ban không được có số
            if (department.DepartmentName.Any(char.IsDigit))
            {
                errorData.Add("DepartmentName", Resources.ResourceVN.Error_DepartmentNameNotNumber);
            }

            return errorData;
        }

        /// <summary>
        /// Kiểm tra mã phòng ban không được phép trùng
        /// </summary>
        /// <param name="departmentCode">Mã muốn kiểm tra</param>
        /// <returns>
        /// true - bị trùng
        /// false - không bị trùng
        /// </returns>
        /// Created by: NKCanh - 25/07/2024
        private bool CheckDepartmentCode(string departmentCode)
        {
            //1. Khai báo chuỗi kết nối
            string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                    "Database = HAUI_2021604405_NguyenKhacCanh; " +
                    "User Id = manhnv; Password = 12345678";

            //2. Khởi tạo chuỗi kết nối
            MySqlConnection connection = new MySqlConnection(connectionString);

            //3. Truy vấn
            var sql = "SELECT DepartmentCode FROM Department WHERE DepartmentCode = @DepartmentCode";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@DepartmentCode", departmentCode);

            //4. Lấy dữ liệu
            var res = connection.QueryFirstOrDefault<Department>(sql: sql, param: parameters);

            if (res != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Lỗi không tìm thấy
        /// </summary>
        /// <returns>404 - Không tìm thấy</returns>
        /// Created by: NKCanh - 25/07/2024
        private IActionResult ErrorNotFound()
        {
            ErrorService error = new ErrorService();
            error.UserMsg = Resources.ResourceVN.Error_NotFound;
            return NotFound(error);
        }

        /// <summary>
        /// Kiểm tra theo id
        /// </summary>
        /// <param name="departmentId">id muốn kiểm tra</param>
        /// <returns>Đối tượng theo id</returns>
        /// Created by: NKCanh - 25/07/2024
        private Department? CheckDepartmentById(Guid departmentId)
        {
            //1. Khai báo chuỗi kết nối
            string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                    "Database = HAUI_2021604405_NguyenKhacCanh; " +
                    "User Id = manhnv; Password = 12345678";

            //2. Khởi tạo chuỗi kết nối
            MySqlConnection connection = new MySqlConnection(connectionString);

            //3. Truy vấn
            var sql = "SELECT * FROM Department WHERE DepartmentId = @DepartmentId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@DepartmentId", departmentId);

            //4. Lấy dữ liệu
            var departmentById = connection.QueryFirstOrDefault<Department>(sql: sql, param: parameters);

            return departmentById;
        }

        /// <summary>
        /// Lỗi ngoại lệ
        /// </summary>
        /// <param name="ex">Lỗi</param>
        /// <returns>500 - Lỗi phía server</returns>
        /// Created by: NKCanh - 25/07/2024
        private IActionResult ErrorException(Exception ex)
        {
            ErrorService error = new ErrorService();
            error.DevMsg = ex.Message;
            error.UserMsg = Resources.ResourceVN.Error_Exception;
            error.Data = ex.Data;
            return StatusCode(500, error);
        }
    }
}
