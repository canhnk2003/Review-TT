namespace MISA.CukCuk.Model
{
    /// <summary>
    /// Lớp lỗi dịch vụ
    /// Created By: NKCanh - 20/07/2024
    /// </summary>
    public class ErrorService
    {
        /// <summary>
        /// Thông báo cho dev
        /// </summary>
        public string? DevMsg { get; set; }

        /// <summary>
        /// Thông báo cho người dùng
        /// </summary>
        public string? UserMsg { get; set; }
        
        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public object? Data { get; set; }
    }
}
