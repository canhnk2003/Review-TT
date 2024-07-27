﻿namespace MISA.CukCuk.Model
{
    /// <summary>
    /// Thông tin phòng ban
    /// Created by: NKCanh - 25/07/2024
    /// </summary>
    public class Department
    {
        #region Properties
        //Phòng ban id
        public Guid DepartmentId { get; set; }

        //Mã phòng ban
        public string DepartmentCode { get; set; }
        
        //Tên phòng ban
        public string DepartmentName { get; set;}

        //Người tạo
        public string? CreatedBy { get; set; }

        //Ngày tạo
        public DateTime? CreatedDate { get; set; }

        //Người sửa
        public string? ModifiedBy { get; set; }

        //Ngày sửa
        public DateTime? ModifiedDate { get; set; }
        
        //Mô tả
        public string? Description { get; set; }
        #endregion
    }
}
