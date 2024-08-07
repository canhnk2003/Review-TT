﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Core.Exceptions
{
    /// <summary>
    /// Lỗi khi có dữ liệu đầu vào không hợp lệ
    /// </summary>
    /// Created By: NKCanh - 06/08/2024
    public class ErrorValidDataException:Exception
    {
        /// <summary>
        /// Các thông báo lỗi
        /// </summary>
        public Dictionary<string, string>? ErrorValidData { get; set; }

        //Hàm khởi tạo các thông báo lỗi
        public ErrorValidDataException(Dictionary<string, string>? errorValidData)
        {
            ErrorValidData = errorValidData;
        }

        //Ghi đè thông báo lỗi
        public override string Message
        {
            get
            {
                return Resources.ResourceVN.Error_ValidData;
            }
        }
    }
}
