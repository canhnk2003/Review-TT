using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Core.Exceptions;

namespace MISA.CukCuk.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        IEmployeeRepository _employeeRepository;
        IEmployeeService _employeeService;

        public EmployeesController(IEmployeeRepository employeeRepository, IEmployeeService employeeService)
        {
            _employeeRepository = employeeRepository;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Lấy danh sách nhân viên
        /// </summary>
        /// <returns>
        /// 200 - Lấy thành công
        /// </returns>
        /// Created By: NKCanh - 01/08/2024
        [HttpGet]
        public IActionResult GET()
        {
            //Lấy dữ liệu
            var employees = _employeeRepository.GetAll();

            //Trả về kết quả
            return Ok(employees);
        }

        /// <summary>
        /// Lấy ra 1 đối tượng với id tương ứng
        /// </summary>
        /// <param name="EmployeeId">id của nhân viên</param>
        /// <returns>
        /// 200 - Lấy ra thành công
        /// 404 - Lỗi không tìm thấy
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 01/08/2024
        [HttpGet("{EmployeeId}")]
        public IActionResult GET([FromRoute] Guid EmployeeId)
        {
            //Lấy dữ liệu
            var employeeById = _employeeRepository.GetById(EmployeeId);

            //Trả về kết quả
            if (employeeById == null)
            {
                throw new ErrorNotFoundException();
            }

            return Ok(employeeById);
        }

        /// <summary>
        /// Thêm mới nhân viên
        /// </summary>
        /// <param name="employee">dữ liệu</param>
        /// <returns>
        /// 201 - Thêm thành công
        /// 200 - Thêm thất bại
        /// 400 - Lỗi dữ liệu hợp lệ
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created By: NKCanh - 02/08/2024
        [HttpPost]
        public IActionResult POST([FromBody] Employee employee)
        {
            int res = _employeeService.InsertService(employee);

            return StatusCode(201, res);
        }

        /// <summary>
        /// Sửa thông tin nhân viên
        /// </summary>
        /// <param name="EmployeeId">id muốn sửa</param>
        /// <param name="employee">dữ liệu</param>
        /// <returns>
        /// 201 - Sửa thành công
        /// 200 - Sửa thất bại
        /// 400 - Lỗi dữ liệu
        /// 404 - Không có trong database
        /// 500 - Lỗi phía client
        /// </returns>
        /// Created By: NKCanh - 02/08/2024
        [HttpPut("{EmployeeId}")]
        public IActionResult PUT([FromRoute] Guid EmployeeId, [FromBody] Employee employee)
        {
            int res = _employeeService.UpdateService(employee, EmployeeId);

            return StatusCode(201, res);
        }



        /// <summary>
        /// Xóa theo id
        /// </summary>
        /// <param name="EmployeeId">id muốn xóa</param>
        /// <returns>
        /// 201 - Xóa thành công
        /// 200 - Xóa thất bại
        /// 400 - Không có trong database
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created by: NKCanh - 01/08/2024
        [HttpDelete("{EmployeeId}")]
        public IActionResult DELETE([FromRoute] Guid EmployeeId)
        {
            var res = _employeeService.DeleteService(EmployeeId);
            return StatusCode(201, res);
        }

        //[HttpDelete("{EmployeeIds}")]
        //public IActionResult DELETEANY([FromRoute] Guid[] EmployeeIds)
        //{
        //    try
        //    {
        //        var res = _employeeRepository.DeleteAny(EmployeeIds);
        //        if (res > 0)
        //        {
        //            return StatusCode(201, res);
        //        }
        //        else
        //        {
        //            var error = _employeeService.ErrorDeleteService();
        //            return Ok(error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ErrorException(ex);
        //    }
        //}
    }
}
