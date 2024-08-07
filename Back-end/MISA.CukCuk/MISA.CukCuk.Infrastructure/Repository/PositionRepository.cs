using Microsoft.Extensions.Configuration;
using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Infrastructure.Repository
{
    /// <summary>
    /// Thực hiện các công việc ở IPositionRepository
    /// </summary>
    /// Created By: NKCanh - 07/08/2024
    public class PositionRepository : BaseRepository<Position>, IPositionRepository
    {
        public PositionRepository(IDBContext dbContext) : base(dbContext)
        {
           
        }

        /// <summary>
        /// Kiểm tra dữ liệu khi sửa
        /// </summary>
        /// <param name="position">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        public Dictionary<string, string>? CheckDataValidate(Position position)
        {
            var errorData = new Dictionary<string, string>();

            //1. Kiểm tra các thông tin không được trống
            //Mã phòng ban
            if (string.IsNullOrEmpty(position.PositionCode))
            {
                errorData.Add("PositionCode", ResourceVN.Error_PositionCodeNotEmpty);
            }

            //Tên phòng ban
            if (string.IsNullOrEmpty(position.PositionName))
            {
                errorData.Add("PositionName", ResourceVN.Error_PositionNameNotEmpty);
            }

            //2. Kiểm tra dữ liệu hợp lệ
            //Tên không được có số
            if (position.PositionName.Any(char.IsDigit))
            {
                errorData.Add("PositionName", ResourceVN.Error_PositionNameNotNumber);
            }

            return errorData;
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm
        /// </summary>
        /// <param name="position">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        public Dictionary<string, string>? CheckDataValidateForInsert(Position position)
        {
            var errorData = CheckDataValidate(position);

            bool checkCode = CheckCode(position.PositionCode);

            //Kiểm tra mã không được trùng
            if (checkCode)
            {
                errorData.Add("PositionCode", ResourceVN.Error_PositionCodeDuplicated);
            }

            return errorData;
        }
    }
}
