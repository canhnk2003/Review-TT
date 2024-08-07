using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.CukCuk.Infrastructure.Repository;

namespace MISA.CukCuk.Infrastructure.Service
{
    /// <summary>
    /// Thực hiện các công việc ở IPositionService
    /// </summary>
    /// Created By: NKCanh - 07/08/2024
    public class PositionService : BaseService<Position>, IPositionService
    {
        IPositionRepository _positionRepository;
        public PositionService(IPositionRepository positionRepository) : base(positionRepository)
        {
            _positionRepository = positionRepository;
        }

        /// <summary>
        /// Dịch vụ thêm mới
        /// </summary>
        /// <param name="data">dữ liệu</param>
        /// <returns>Số bản ghi thêm thành công</returns>
        /// <exception cref="ErrorValidDataException">Lỗi dữ liệu hợp lệ</exception>
        /// <exception cref="ErrorCreateException">Lỗi thêm mới</exception>
        public override int InsertService(Position data)
        {
            //Kiểm tra dữ liệu hợp lệ
            var checkData = _positionRepository.CheckDataValidateForInsert(data);

            //Nếu dữ liệu không hợp lệ thì thông báo lỗi, nếu hợp lệ thì cho phép thêm
            if (checkData.Count > 0)
            {
                throw new ErrorValidDataException(checkData);
            }
            else
            {
                int res = _positionRepository.Insert(data);

                if (res > 0)
                {
                    return res;
                }
                else
                {
                    throw new ErrorCreateException();
                }
            }

        }

        /// <summary>
        /// Dịch vụ sửa
        /// </summary>
        /// <param name="data">Dữ liệu</param>
        /// <param name="id">id</param>
        /// <returns>Số dữ liệu sửa thành công</returns>
        /// <exception cref="ErrorNotFoundException">Lỗi không tìm thấy trong database</exception>
        /// <exception cref="ErrorValidDataException">Lỗi dữ liệu không hợp lệ</exception>
        /// <exception cref="ErrorEditException">Lỗi sửa dữ liệu</exception>
        public override int UpdateService(Position data, Guid id)
        {
            //Kiểm tra xem có id trong database không
            var check = _positionRepository.GetById(id);

            //Nếu không có thì lỗi, nếu nó thì cho sửa
            if (check == null)
            {
                throw new ErrorNotFoundException();
            }
            else
            {
                //Kiểm tra dữ liệu hợp lệ
                var checkData = _positionRepository.CheckDataValidate(data);

                //Nếu hợp lệ thì cho thêm, không thì lỗi
                if (checkData.Count > 0)
                {
                    throw new ErrorValidDataException(checkData);
                }
                else
                {
                    int res = _positionRepository.Update(data, id);

                    if (res > 0)
                    {
                        return res;
                    }
                    else
                    {
                        throw new ErrorEditException();
                    }
                }
            }
        }
    }
}
