using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Exceptions;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Infrastructure.Service
{
    /// <summary>
    /// Thực hiện các công việc ở IEmployeeService
    /// </summary>
    /// Created By: NKCanh - 07/08/2024
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        IEmployeeRepository _employeeRepository;
        public EmployeeService(IEmployeeRepository employeeRepository) : base(employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Dịch vụ thêm mới
        /// </summary>
        /// <param name="data">dữ liệu</param>
        /// <returns>Số bản ghi thêm thành công</returns>
        /// <exception cref="ErrorValidDataException">Lỗi dữ liệu hợp lệ</exception>
        /// <exception cref="ErrorCreateException">Lỗi thêm mới</exception>
        public override int InsertService(Employee data)
        {
            //Kiểm tra dữ liệu hợp lệ
            var checkData = _employeeRepository.CheckDataValidateForInsert(data);

            //Nếu hợp lệ thì cho thêm, không thì lỗi
            if (checkData.Count > 0)
            {
                throw new ErrorValidDataException(checkData);
            }
            else
            {
                int res = _employeeRepository.Insert(data);
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
        public override int UpdateService(Employee data, Guid id)
        {
            //Kiểm tra xem có id trong database không
            var check = _employeeRepository.GetById(id);

            //Nếu không có thì lỗi, nếu nó thì cho sửa
            if (check == null)
            {
                throw new ErrorNotFoundException();
            }
            else
            {
                //Kiểm tra dữ liệu hợp lệ
                var checkData = _employeeRepository.CheckDataValidate(data);

                //Nếu hợp lệ thì cho thêm, không thì lỗi
                if (checkData.Count > 0)
                {
                    throw new ErrorValidDataException(checkData);
                }
                else
                {
                    int res = _employeeRepository.Update(data, id);

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
