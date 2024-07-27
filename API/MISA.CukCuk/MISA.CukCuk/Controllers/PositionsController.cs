using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using MySqlConnector;
using MISA.CukCuk.Model;

namespace MISA.CukCuk.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        /// <summary>
        /// Lấy ra danh sách các vị trí
        /// </summary>
        /// <returns>
        /// 200 - Lấy thành công
        /// 500 - Có lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 24/07/2024
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

                //3. Khai báo câu truy vấn lấy dữ liệu
                var sql = "SELECT * FROM Position";

                //4. Lấy dữ liệu
                var positions = connection.Query<Position>(sql);

                //5. Trả dữ liệu về client
                return Ok(positions);
            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Lấy ra 1 đối tượng với id tương ứng
        /// </summary>
        /// <param name="PositionId">id của vị trí truyền vào trên router</param>
        /// <returns>
        /// 200 - Lấy ra thành công
        /// 404 - Lỗi không tìm thấy
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 24/07/2024
        [HttpGet("{PositionId}")]
        public IActionResult Get([FromRoute] Guid PositionId)
        {
            try
            {
                //1. Kiểm tra xem có Id trong database không
                var positionById = CheckPositionId(PositionId);

                //2. Nếu có trả về Ok, nếu không trả về NotFound
                if (positionById != null)
                {
                    return Ok(positionById);
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
        /// Kiểm tra PositionId có trong database không
        /// </summary>
        /// <param name="PositionId">Tham số đầu vào</param>
        /// <returns>Đối tượng Position</returns>
        /// Created by: NKCanh - 24/07/2024
        private static Position? CheckPositionId(Guid PositionId)
        {
            //1. Khai báo chuỗi kết nối
            string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                "Database = HAUI_2021604405_NguyenKhacCanh; " +
                "User Id = manhnv; Password = 12345678";

            //2. Khởi tạo chuỗi kết nối
            var connection = new MySqlConnection(connectionString);

            //3. Khai báo câu truy vấn lấy dữ liệu
            var sqlCommand = $"SELECT * FROM Position WHERE PositionId = @PositionId";

            //4. Lấy dữ liệu
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PositionId", PositionId);

            var positionById = connection.QueryFirstOrDefault<Position>(sql: sqlCommand, param: parameters);

            return positionById;
        }

        /// <summary>
        /// Thêm vào 1 position
        /// </summary>
        /// <param name="position">Đối tượng đầu vào</param>
        /// <returns>
        /// 201 - Thêm thành công
        /// 200 - Thêm thất bại
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 24/07/2024
        [HttpPost]
        public IActionResult POST([FromBody] Position position)
        {
            try
            {
                //1. Kiểm tra dữ liệu hợp lệ
                Dictionary<string, string> errorData = CheckData(position);

                //Mã vị trí không được phép trùng
                if (CheckPositionCode(position.PositionCode))
                {
                    errorData.Add("PositionCode", Resources.ResourceVN.Error_PositionCodeDuplicated);
                }

                //2. Nếu không hợp lệ thì trả về lỗi, hợp lệ cho phép tạo mới 1 vị trí
                if (errorData.Count > 0)
                {
                    return ErrorInvalid(errorData);
                }
                else
                {
                    //Tạo mới 1 PositionId
                    position.PositionId = Guid.NewGuid();

                    //2.1. Khai báo chuỗi kết nối tới máy chủ
                    string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                    "Database = HAUI_2021604405_NguyenKhacCanh; " +
                    "User Id = manhnv; Password = 12345678";

                    //2.2. Khởi tạo chuỗi kết nối
                    var connection = new MySqlConnection(connectionString);

                    //2.3. Truy vấn thêm dữ liệu
                    var sqlText = "Proc_AddPosition";

                    //2.4. Thêm dữ liệu

                    var res = AddAndEditData(position, connection, sqlText);

                    if (res != 0)
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
        /// Cập nhật 1 position theo id
        /// </summary>
        /// <param name="PositionId">PositionId đầu vào</param>
        /// <param name="position">Đối tượng sửa</param>
        /// <returns>
        /// 201 - Sửa thành công
        /// 200 - Sửa thất bại
        /// 404 - Không có id này trong Database
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 24/07/2024
        [HttpPut("{PositionId}")]
        public IActionResult PUT([FromRoute] Guid PositionId, [FromBody] Position position)
        {
            try
            {
                //1. Kiểm tra xem có id vị trí trong database không
                var checkPositionId = CheckPositionId(PositionId);

                if (checkPositionId != null)
                {
                    //2. Validate dữ liệu
                    var errorData = CheckData(position);
                    if (errorData.Count > 0)
                    {
                        return ErrorInvalid(errorData);
                    }
                    else
                    {
                        //3. Sửa dữ liệu
                        //3.1. Lấy ID cũ
                        position.PositionId = PositionId;

                        //3.2. Khai báo chuỗi kết nối
                        string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                            "Database = HAUI_2021604405_NguyenKhacCanh; " +
                            "User Id = manhnv; Password = 12345678";

                        //3.3. Khởi tạo chuỗi kết nối
                        var connection = new MySqlConnection(connectionString);

                        //3.4. Thực hiện sửa dữ liệu
                        var sqlText = "Proc_UpdatePosition";

                        int res = AddAndEditData(position, connection, sqlText);

                        if (res != 0)
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
        /// Xóa 1 Position theo id
        /// </summary>
        /// <param name="PositionId">Id của đối tượng muốn xóa</param>
        /// <returns>
        /// 201 - Xóa thành công
        /// 200 - Xóa thất bại
        /// 404 - Không có id này trong Database
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 24/07/2024
        [HttpDelete("{PositionId}")]
        public IActionResult DELETE([FromRoute] Guid PositionId)
        {
            try
            {
                //3. Kiểm tra xem có trong database không
                var positionId = CheckPositionId(PositionId);
                if (positionId != null)
                {
                    //1. Khai báo chuỗi kết nối
                    string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                            "Database = HAUI_2021604405_NguyenKhacCanh; " +
                            "User Id = manhnv; Password = 12345678";

                    //2. Khởi tạo chuỗi kết nối
                    var connection = new MySqlConnection(connectionString);

                    //4. Truy vấn xóa dữ liệu
                    var sql = "DELETE Position FROM Position WHERE PositionId = @PositionId";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@PositionId", PositionId);

                    //5. Xóa dữ liệu
                    var res = connection.Execute(sql: sql, param: parameters);

                    if(res > 0 )
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

        /// <summary>
        /// Thêm hoặc sửa 1 position
        /// </summary>
        /// <param name="position">Dữ liệu của đối tượng</param>
        /// <param name="connection">Kết nối</param>
        /// <param name="sqlText">Chuỗi truy vấn</param>
        /// <returns>
        /// Số bản ghi đã thêm hoặc sửa được
        /// </returns>
        /// Created by: NKCanh - 24/07/2024
        private static int AddAndEditData(Position position, MySqlConnection connection, string sqlText)
        {
            //Mở kết nối đến database
            connection.Open();

            //Đọc các tham số đầu vào của store
            var sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = sqlText;
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            MySqlCommandBuilder.DeriveParameters(sqlCommand);

            DynamicParameters parameters = new DynamicParameters();
            foreach (MySqlParameter param in sqlCommand.Parameters)
            {
                //Tên của tham số
                var paramName = param.ParameterName;
                var propName = paramName.Replace("@m_", "");
                var entityProperty = position.GetType().GetProperty(propName);
                if (entityProperty != null)
                {
                    var propValue = position.GetType().GetProperty(propName).GetValue(position);

                    //Thực hiện gán giá trị cho các param
                    parameters.Add(paramName, propValue);
                }
                else
                {
                    parameters.Add(paramName, null);
                }
            }

            var res = connection.Execute(sql: sqlText, param: parameters);

            return res;
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lê
        /// </summary>
        /// <param name="p">Dữ liệu đối tượng</param>
        /// <returns>
        /// Danh sách lỗi
        /// </returns>
        /// Created by: NKCanh - 24/07/2024
        private static Dictionary<string, string> CheckData(Position p)
        {
            var errorData = new Dictionary<string, string>();

            //1. Kiểm tra 1 số thông tin không được trống
            //1.1. Mã vị trí
            if (string.IsNullOrEmpty(p.PositionCode))
            {
                errorData.Add("PositionCode", Resources.ResourceVN.Error_PositionCodeNotEmpty);
            }

            //1.2. Tên vị trí
            if (string.IsNullOrEmpty(p.PositionName))
            {
                errorData.Add("PositionName", Resources.ResourceVN.Error_PositionNameNotEmpty);
            }

            //2. Kiểm tra dữ liệu hợp lệ
            //Tên vị trí không được phép có số
            if (p.PositionName.Any(char.IsDigit))
            {
                errorData.Add("PositionName", Resources.ResourceVN.Error_PositionNameNotNumber);
            }

            return errorData;
        }

        /// <summary>
        /// Kiểm tra mã vị trí có trong database chưa
        /// </summary>
        /// <param name="positionCode">Mã vị trí kiểm tra</param>
        /// <returns>
        /// true - đã có
        /// false - chưa có
        /// </returns>
        /// Created by: NKCanh - 24/07/2024
        private static bool CheckPositionCode(string positionCode)
        {
            //1. Khai báo thông tin máy chủ
            string connectionString = "Host = 8.222.228.150; Port = 3306; " +
                    "Database = HAUI_2021604405_NguyenKhacCanh; " +
                    "User Id = manhnv; Password = 12345678";

            //2. Khởi tạo chuỗi kết nối
            var connection = new MySqlConnection(connectionString);

            //3. lấy dữ liệu
            var sqlCommand = "SELECT PositionCode FROM Position WHERE PositionCode = @PositionCode";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PositionCode", positionCode);

            //4. Kiểm tra PositionCode đã tồn tại chưa
            var res = connection.QueryFirstOrDefault<string>(sql: sqlCommand, param: parameters);

            if (res != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hiển thị thông báo lỗi khi dữ liệu không hợp lệ
        /// </summary>
        /// <param name="errorData">Danh sách lỗi</param>
        /// <returns>400 - BadRequest</returns>
        /// Created by: NKCanh - 24/07/2024
        private IActionResult ErrorInvalid(Dictionary<string, string> errorData)
        {
            ErrorService error = new ErrorService();
            error.UserMsg = Resources.ResourceVN.Error_ValidData;
            error.Data = errorData;
            return BadRequest(error);
        }

        /// <summary>
        /// Lỗi khi không thấy trong database
        /// </summary>
        /// <returns>404 - NotFound</returns>
        /// Created by: NKCanh - 24/07/2024
        private IActionResult ErrorNotFound()
        {
            ErrorService error = new ErrorService();
            error.UserMsg = Resources.ResourceVN.Error_NotFound;
            return NotFound(error);
        }

        /// <summary>
        /// Lỗi ngoại lệ
        /// </summary>
        /// <param name="ex">Lỗi</param>
        /// <returns>500 - Lỗi phía Server</returns>
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
