using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MISA.CukCuk.Core;
using MISA.CukCuk.Core.Exceptions;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Core.Resources;

namespace MISA.CukCuk.Infrastructure.Service
{
    /// <summary>
    /// Thực hiện các công việc ở IBaseService
    /// </summary>
    /// <typeparam name="T">Kiểu đối tượng</typeparam>
    public class BaseService<T> : IBaseService<T> where T : class
    {
        IBaseRepository<T> _repository;

        public BaseService(IBaseRepository<T> baseRepository)
        {
            _repository = baseRepository;
        }

        /// <summary>
        /// Dịch vụ thêm mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual int InsertService(T data)
        {
            return -1;
        }

        /// <summary>
        /// Dịch vụ sửa
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int UpdateService(T data, Guid id)
        {
            return -1;
        }

        /// <summary>
        /// Dịch vụ xóa
        /// </summary>
        /// <param name="id">id muốn xóa</param>
        /// <returns>Số bản ghi xóa thành công</returns>
        /// <exception cref="ErrorNotFoundException">Lỗi không tìm thấy dữ liệu</exception>
        /// <exception cref="ErrorDeleteException">Lỗi xóa không thành công</exception>
        public int DeleteService(Guid id)
        {
            //Kiểm tra xem có trong database không
            var check = _repository.GetById(id);

            //Nếu có cho xóa, nếu không thì lỗi
            if (check == null)
            {
                throw new ErrorNotFoundException(); 
            }
            else
            {
                int res = _repository.Delete(id);

                if (res > 0)
                {
                    return res;
                }
                else
                {
                    throw new ErrorDeleteException();
                }
            }
        }
    }
}
