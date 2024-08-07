using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Core.Exceptions;

namespace MISA.CukCuk.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        IPositionRepository _positionsRepository;
        IPositionService _positionsService;

        public PositionsController(IPositionRepository positionsRepository, IPositionService positionsService)
        {
            _positionsRepository = positionsRepository;
            _positionsService = positionsService;
        }

        /// <summary>
        /// Lấy tất cả dữ liệu
        /// </summary>
        /// <returns>
        /// 200 - Lấy thành công
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created By: NKCanh - 03/08/2024
        [HttpGet]
        public IActionResult GET()
        {
            var positions = _positionsRepository.GetAll();

            return Ok(positions);
        }


        /// <summary>
        /// Lấy theo id
        /// </summary>
        /// <param name="PositionId">id muốn lấy</param>
        /// <returns>
        /// 200 - Lấy thành công
        /// 404 - Không có
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created By: NKCanh - 03/08/2024
        [HttpGet("{PositionId}")]
        public IActionResult GET([FromRoute] Guid PositionId)
        {
            var position = _positionsRepository.GetById(PositionId);

            if (position == null)
            {
                throw new ErrorNotFoundException();
            }

            return Ok(position);
        }

        /// <summary>
        /// Thêm mới 1 vị trí
        /// </summary>
        /// <param name="Position">Dữ liệu vị trí</param>
        /// <returns>
        /// 201 - Thêm thành công
        /// 200 - Thêm thất bại
        /// 400 - Lỗi phía client
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created By: NKCanh - 03/08/2024
        [HttpPost]
        public IActionResult POST([FromBody] Position position)
        {
            var res = _positionsService.InsertService(position);

            return StatusCode(201, res);
        }

        /// <summary>
        /// Sửa thông tin theo id
        /// </summary>
        /// <param name="Position">dữ liệu</param>
        /// <param name="PositionId">id muốn sửa</param>
        /// <returns>
        /// 201 - Sửa thành công
        /// 200 - Sửa thất bại
        /// 400 - Lỗi phía client
        /// 404 - Lỗi không tìm thấy trong database
        /// 500 - Lỗi phía client
        /// </returns>
        [HttpPut("{PositionId}")]
        public IActionResult PUT([FromBody] Position position, [FromRoute] Guid PositionId)
        {
            var res = _positionsService.UpdateService(position, PositionId);

            return StatusCode(201, res);
        }

        /// <summary>
        /// Xóa dữ liệu theo id
        /// </summary>
        /// <param name="PositionId">id vị trí muốn xóa</param>
        /// <returns>
        /// 201 - Xóa thành công
        /// 200 - Xóa không thành công
        /// 404 - Không có trong database
        /// 500 - Lỗi phía server
        /// </returns>
        /// Created By: NKCanh - 01/08/2024
        [HttpDelete("{PositionId}")]
        public IActionResult DELETE([FromRoute] Guid PositionId)
        {
            var res = _positionsService.DeleteService(PositionId);
            return StatusCode(201, res);
        }
    }
}
