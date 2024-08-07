using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Core.Exceptions
{
    /// <summary>
    /// Lỗi khi tạo không thành công
    /// </summary>
    /// Created By: NKCanh - 06/08/2024
    public class ErrorCreateException : Exception
    {
        /// <summary>
        /// Ghi đè Thông báo lỗi
        /// </summary>
        public override string Message
        {
            get
            {
                return Resources.ResourceVN.Error_Create;
            }
        }
    }
}
