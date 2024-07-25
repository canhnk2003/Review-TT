namespace MISA.CukCuk.Model
{
    /// <summary>
    /// Thông tin vị trí
    /// Created by: NKCanh - 22/07/2024
    /// </summary>
    public class Position
    {
        #region Properties
        /// <summary>
        /// Khóa chính - vị trí id
        /// </summary>
        public Guid PositionId { get; set; }

        /// <summary>
        /// Mã vị trí
        /// </summary>
        public string PositionCode { get; set; }

        /// <summary>
        /// Tên vị trí
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Description { get; set; }

        #endregion

        #region Constructor
        public Position() { }

        public Position(Guid positionId, string positionCode, string positionName, string? createdBy, DateTime? createdDate)
        {
            PositionId = positionId;
            PositionCode = positionCode;
            PositionName = positionName;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }

        #endregion
    }
}
