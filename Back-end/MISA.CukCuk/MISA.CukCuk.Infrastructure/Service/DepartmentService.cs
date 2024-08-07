using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.CukCuk.Core;
using System.Data;
using MySqlConnector;
using Dapper;
using MISA.CukCuk.Core.Resources;
using System.Net;
using MISA.CukCuk.Core.Exceptions;

namespace MISA.CukCuk.Infrastructure.Service
{
    /// <summary>
    /// Thực hiện các công việc ở IDepartmentService
    /// </summary>
    /// Created By: NKCanh - 07/08/2024
    public class DepartmentService : BaseService<Department>, IDepartmentService
    {
        IDepartmentRepository _departmentRepository;
        public DepartmentService(IDepartmentRepository departmentRepository) : base(departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// Dịch vụ thêm mới
        /// </summary>
        /// <param name="data">Dữ liệu</param>
        /// <returns>Số bản ghi xóa thành công</returns>
        /// <exception cref="ErrorValidDataException">Lỗi dữ liệu không hợp lệ</exception>
        /// <exception cref="ErrorCreateException">Lỗi thêm mới không thành công</exception>
        public override int InsertService(Department data)
        {
            //Kiểm tra dữ liệu hợp lệ
            var checkData = _departmentRepository.CheckDataValidateForInsert(data);

            //Nếu hợp lệ thì cho thêm, không thì lỗi
            if (checkData.Count > 0)
            {
                throw new ErrorValidDataException(checkData);
            }
            else
            {
                int res = _departmentRepository.Insert(data);

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
        public override int UpdateService(Department data, Guid id)
        {
            //Kiểm tra xem có id trong database không
            var check = _departmentRepository.GetById(id);

            //Nếu không có thì lỗi, nếu nó thì cho sửa
            if(check == null)
            {
                throw new ErrorNotFoundException();
            }
            else
            {
                //Kiểm tra dữ liệu hợp lệ
                var checkData = _departmentRepository.CheckDataValidate(data);

                //Nếu hợp lệ thì cho thêm, không thì lỗi
                if (checkData.Count > 0)
                {
                    throw new ErrorValidDataException(checkData);
                }
                else
                {
                    int res = _departmentRepository.Update(data, id);

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
