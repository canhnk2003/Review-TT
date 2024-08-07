using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Core.Exceptions;

namespace MISA.CukCuk.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        IDepartmentRepository _departmentsRepository;
        IDepartmentService _departmentsService;

        public DepartmentsController(IDepartmentRepository departmentsRepository, IDepartmentService departmentsService)
        {
            _departmentsRepository = departmentsRepository;
            _departmentsService = departmentsService;
        }

        /// <summary>
        /// Lấy tất cả dữ liệu
        /// </summary>
        /// <returns>
        /// 200 - Lấy thành công
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created By: NKCanh - 31/07/2024
        [HttpGet]
        public IActionResult GET()
        {
            var departments = _departmentsRepository.GetAll();

            return Ok(departments);
        }

        /// <summary>
        /// Lấy theo id
        /// </summary>
        /// <param name="DepartmentId">id muốn lấy</param>
        /// <returns>
        /// 200 - Lấy thành công
        /// 404 - Không có
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created By: NKCanh - 31/07/2024
        [HttpGet("{DepartmentId}")]
        public IActionResult GET([FromRoute] Guid DepartmentId)
        {
            var department = _departmentsRepository.GetById(DepartmentId);

            if(department == null)
            {
                throw new ErrorNotFoundException();
            }
            return Ok(department);

        }

        /// <summary>
        /// Thêm mới 1 phòng ban
        /// </summary>
        /// <param name="department">Dữ liệu phòng ban</param>
        /// <returns>
        /// 201 - Thêm thành công
        /// 200 - Thêm thất bại
        /// 400 - Lỗi phía client
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created By: NKCanh - 31/07/2024
        [HttpPost]
        public IActionResult POST([FromBody] Department department)
        {
            var res = _departmentsService.InsertService(department);

            return StatusCode(201, res);
        }

        /// <summary>
        /// Sửa thông tin theo id
        /// </summary>
        /// <param name="department">dữ liệu</param>
        /// <param name="DepartmentId">id muốn sửa</param>
        /// <returns>
        /// 201 - Sửa thành công
        /// 200 - Sửa thất bại
        /// 400 - Lỗi phía client
        /// 404 - Lỗi không tìm thấy trong database
        /// 500 - Lỗi phía client
        /// </returns>
        [HttpPut("{DepartmentId}")]
        public IActionResult PUT([FromBody] Department department, [FromRoute] Guid DepartmentId)
        {
            var res = _departmentsService.UpdateService(department, DepartmentId);

            return StatusCode(201, res);
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
        /// Created By: NKCanh - 01/08/2024
        [HttpDelete("{DepartmentId}")]
        public IActionResult DELETE([FromRoute] Guid DepartmentId)
        {
            var res = _departmentsService.DeleteService(DepartmentId);

            return StatusCode(201, res);
        }
    }
}
