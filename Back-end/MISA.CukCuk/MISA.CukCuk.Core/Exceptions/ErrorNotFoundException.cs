using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Core.Exceptions
{
    /// <summary>
    /// Lỗi khi không tìm thấy dữ liệu
    /// </summary>
    /// Created By: NKCanh - 06/08/2024
    public class ErrorNotFoundException:Exception
    {
        //Ghi đè thông báo lỗi
        public override string Message
        {
            get
            {
                return Resources.ResourceVN.Error_NotFound;
            }
        }
    }
}
