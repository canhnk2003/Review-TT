using Microsoft.AspNetCore.Mvc;
using MISA.CukCuk.Model;
using System.Text.RegularExpressions;

namespace MISA.CukCuk.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        /// <summary>
        /// Tạo một danh sách để lưu các nhân viên
        /// </summary>
        static List<Employee> employees = new List<Employee>();

        /// <summary>
        /// Thêm vào danh sách 1 số bản ghi
        /// </summary>
        private static void FakeData()
        {
            employees.Add(new Employee(Guid.Parse("b7af7682-c1ea-43f5-bddd-ab3d84b2e3ec"), "NV-99999", "Nguyễn Khắc Cảnh", 1, DateTime.Now, Guid.Parse("b7af7682-c1ea-43f5-bddd-ab3d84b2e3ec"), "0202020202", DateTime.Today, Guid.Parse("b7af7682-c1ea-43f5-bddd-ab3d84b2e3ec"), "", "Bắc Ninh", "1234567890", "canhnk2003@gmail.com", "1234567890", "MB Bank", "Yên Phong"));
            employees.Add(new Employee(Guid.Parse("124be742-1f93-4946-8942-51db9bdd8051"), "NV-00001", "Nguyễn Văn A", "0909090909", "1111111111", "vana@gmail.com"));
            employees.Add(new Employee(Guid.Parse("19e9d8d9-ca2a-4fff-bab1-75ac59a5ccda"), "NV-00002", "Trần Thị Bình", "0808080808", "2222222222", "binhtran@gmail.com"));
            employees.Add(new Employee(Guid.Parse("527849e8-8b5f-406a-8d4c-313fdfaf99f4"), "NV-00003", "Lê Quang Cường", "0707070707", "3333333333", "cuongle@gmail.com"));
            employees.Add(new Employee(Guid.Parse("1ac5418f-43d9-4509-8176-31c850ca302d"), "NV-00004", "Phạm Thị Dung", "0606060606", "4444444444", "dungpham@gmail.com"));
            employees.Add(new Employee(Guid.Parse("e75cfdb7-cfea-47be-bbf4-7b2664e4e67b"), "NV-00005", "Hoàng Minh Đức", "0505050505", "5555555555", "minhduc@gmail.com"));
            employees.Add(new Employee(Guid.Parse("db996653-b421-41c1-9a15-478d9bb34473"), "NV-00006", "Mai Thị Hà", "0404040404", "6666666666", "hamai@gmail.com"));
            employees.Add(new Employee(Guid.Parse("31a64db0-cc3a-498e-b2d8-203634b16e1c"), "NV-00007", "Nguyễn Văn Kiên", "0303030303", "7777777777", "kiennguyen@gmail.com"));
            employees.Add(new Employee(Guid.Parse("45eb8fa6-54b8-4fa9-82e3-acf5a279225f"), "NV-00008", "Trần Thị Lan", "0101010101", "8888888888", "lantran@gmail.com"));
            employees.Add(new Employee(Guid.Parse("9ac73b00-a22b-48f6-8eca-e54d92fd7a0a"), "NV-00009", "Lê Quốc Minh", "0909090909", "9999999999", "minhle@gmail.com"));
            employees.Add(new Employee(Guid.Parse("f4e853a0-882d-4544-8370-3153bc81acc2"), "NV-00010", "Nguyễn Thị Ngọc", "0808080808", "0000000000", "ngocnguyen@gmail.com"));
        }

        /// <summary>
        /// Gọi FakeData() trong Controller
        /// </summary>
        public EmployeesController()
        {
            FakeData();
        }

        /// <summary>
        /// Lấy ra danh sách nhân viên
        /// </summary>
        /// <returns>
        /// 200 - Lấy ra danh sách thành công
        /// 500 - Badrequest, Lỗi phía server
        /// </returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
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
        [HttpGet("{EmployeeId}")]
        public IActionResult Get([FromRoute] Guid EmployeeId)
        {
            try
            {
                //1. Tìm xem có nhân viên với id đầu vào có trong danh sách không
                Employee employee = employees.FirstOrDefault(e => e.EmployeeId == EmployeeId);

                //2. Trả về status tương ứng Ok-200, ErrorNotFound-404
                if (employee != null)
                {
                    return Ok(employee);
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
        /// Xử lý lỗi không tìm thấy dữ liệu
        /// </summary>
        /// <returns>Mã lỗi 404 - NotFound</returns>
        private IActionResult ErrorNotFound()
        {
            ErrorService e = new ErrorService();
            e.UserMsg = Resources.ResourceVN.Error_NotFound;
            return NotFound(e);
        }

        /// <summary>
        /// Thêm mới 1 đối tượng nhân viên
        /// </summary>
        /// <param name="e">Tham số đầu vào, được nhập từ Body</param>
        /// <returns>
        /// 201 - Tạo thành công
        /// 500 - Lỗi phía Server
        /// </returns>
        [HttpPost]
        public IActionResult Post([FromBody] Employee e)
        {
            try
            {
                //1. Kiểm tra dữ liệu hợp lệ
                Dictionary<string, string> errorData = CheckData(e);

                //2. Nếu dữ liệu không hợp lệ trả về danh sách lỗi, hợp lệ thì cho phép tạo
                if (errorData.Count > 0)
                {
                    //2.1. Không hợp lệ trả về lỗi
                    return ErrorInvalid(errorData);
                }
                else
                {
                    //2.2. Hợp lệ thì tạo mới nhân viên
                    e.EmployeeId = Guid.NewGuid();
                    employees.Add(e);
                    return Created("201", 1);
                }
            }
            catch (Exception ex)
            {
                return ErrorException(ex);
            }
        }

        /// <summary>
        /// Xử lý lỗi không hợp lệ dữ liệu
        /// </summary>
        /// <param name="errorData">Danh sách lỗi</param>
        /// <returns>Lỗi</returns>
        private IActionResult ErrorInvalid(Dictionary<string, string> errorData)
        {
            ErrorService errorService = new ErrorService();
            errorService.UserMsg = "Dữ liệu không hợp lệ!";
            errorService.Data = errorData;
            return BadRequest(errorService);
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ
        /// </summary>
        /// <param name="e">Đối tượng truyền vào</param>
        /// <returns>Danh sách lỗi</returns>
        private Dictionary<string, string> CheckData(Employee e)
        {
            var errorData = new Dictionary<string, string>();

            //1. Kiểm tra 1 số thông tin không được trống
            //1.1. Mã nhân viên không được để trống
            if (string.IsNullOrEmpty(e.EmployeeCode))
            {
                errorData.Add("EmployeeCode", "Mã nhân viên không phép để trống!");
            }

            //1.2. Họ tên nhân viên không được để trống
            if (string.IsNullOrEmpty(e.FullName))
            {
                errorData.Add("FullName", "Họ tên không được phép để trống!");
            }

            //1.3. Số CMTND không được để trống
            if (string.IsNullOrEmpty(e.IdentityNumber))
            {
                errorData.Add("IdentityNumber", "Số CMTND không được phép để trống!");
            }

            //1.4. Số điện thoại không được để trống
            if (string.IsNullOrEmpty(e.PhoneNumber))
            {
                errorData.Add("PhoneNumber", "Số điện thoại không được phép để trống!");
            }

            //1.5. Email không được để trống
            if (string.IsNullOrEmpty(e.Email))
            {
                errorData.Add("Email", "Email không được phép để trống!");
            }
            else
            {
                //Kiểm tra Email đúng định dạng
                if (CheckEmailValid(e.Email) == false)
                {
                    errorData.Add("Email", "Email không đúng định dạng!");
                }
            }

            //Kiểm tra EmployeeCode đã tồn tại chưa
            var checkEmployeeCode = employees.FirstOrDefault(x => x.EmployeeCode == e.EmployeeCode);
            if (checkEmployeeCode != null)
            {
                errorData.Add("EmployeeCode", "Mã nhân viên đã tồn tại!");
            }

            //2. Thực hiện validate dữ liệu
            //2.1. Họ tên không được có số
            if (e.FullName.Any(char.IsDigit))
            {
                errorData.Add("FullName", "Họ tên không được có ký tự số!");
            }

            //2.2. Số CMTND không được có chữ
            if (e.IdentityNumber.Any(char.IsLetter))
            {
                errorData.Add("IdentityNumber", "Số CMTND không được có ký tự chữ!");
            }

            //2.3. Số điện thoại không được có chữ
            if (e.PhoneNumber.Any(char.IsLetter))
            {
                errorData.Add("PhoneNumber", "Số điện thoại không được có ký tự chữ!");
            }

            //2.4. Ngày sinh không được lớn hơn ngày hiện tại
            if (e.DateOfBirth > DateTime.Now)
            {
                errorData.Add("DateOfBirth", "Ngày sinh không được lớn hơn ngày hiện tại!");
            }

            //2.5. Ngày cấp không được lớn hơn ngày hiện tại
            if (e.IdentityDate > DateTime.Now)
            {
                errorData.Add("IdentityDate", "Ngày cấp không được lớn hơn ngày hiện tại!");
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
        /// 404 - Lỗi không tìm thấy
        /// 500 - Lỗi phía server
        /// </returns>
        [HttpPut("{EmployeeId}")]
        public IActionResult PUT([FromRoute] Guid EmployeeId, [FromBody] Employee e)
        {
            try
            {
                //1. Kiểm tra xem có nhân viên với id tương ứng có trong danh sách không
                Employee employee = employees.FirstOrDefault(x => x.EmployeeId == EmployeeId);

                //2. Nếu có thì kiểm tra dữ liệu, nếu không thì thông báo không có
                if (employee != null)
                {
                    //2.1. Thực hiện validate dữ liệu hợp lệ
                    if (CheckData(e).Count > 0)
                    {
                        return ErrorInvalid(CheckData(e));
                    }
                    else
                    {
                        //4.1. Xóa nhân viên có id tương ứng trong danh sách
                        employees.Remove(employee);

                        //4.2. Giữ nguyên id cũ của nhân viên và thêm vào danh sách
                        e.EmployeeId = EmployeeId;
                        employees.Add(e);

                        return StatusCode(201, 1);
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
        /// 200 - Xóa thành công
        /// 404 - Lỗi không tìm thấy
        /// 500 - Lỗi phía server
        /// </returns>
        [HttpDelete("{EmployeeId}")]
        public IActionResult DELETE([FromRoute] Guid EmployeeId)
        {
            try
            {
                //1. Kiểm tra xem có nhân viên với id tương ứng có trong danh sách không
                var e = employees.FirstOrDefault(x => x.EmployeeId == EmployeeId);

                //2. Trả về mã lỗi tương ứng
                if (e != null)
                {
                    //Xóa nhân viên có id tương ứng trong danh sách
                    employees.Remove(e);
                    return StatusCode(200, 1);
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
