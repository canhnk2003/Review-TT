using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CukCuk.Infrastructure.DBContext
{
    /// <summary>
    /// Làm việc với dữ liệu bằng MySQL
    /// </summary>
    /// Created By: NKCanh - 26/07/2024
    public class MySqlDBContext : IDBContext
    {
        /// <summary>
        /// Chuỗi kết nối
        /// </summary>
        public IDbConnection Connection { get; }

        /// <summary>
        /// Khởi tạo chuỗi kết nối
        /// </summary>
        /// <param name="config"></param>
        public MySqlDBContext(IConfiguration config)
        {
            Connection = new MySqlConnection(config.GetConnectionString("Database1"));
        }

        /// <summary>
        /// Xóa dữ liệu theo id
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <param name="id">id</param>
        /// <returns>Số bản ghi xóa thành công</returns>
        public int Delete<T>(Guid id)
        {
            //Lấy ra tên đối tượng
            string className = typeof(T).Name;

            //Câu lệnh truy vấn xóa
            var sql = $"DELETE {className} FROM {className} WHERE {className}Id = @id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@id", id);

            //Xóa dữ liệu
            int res = Connection.Execute(sql, parameters);

            //Trả về kết quả
            return res;
        }

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <param name="ids">danh sách id</param>
        /// <returns>Số bản ghi xóa thành công</returns>
        public int DeleteAny<T>(Guid[] ids)
        {
            //Lấy ra tên đối tượng
            var className = typeof(T).Name;

            //Câu lệnh truy vấn xóa nhiều bản ghi
            var sql = $"DELETE FROM {className} WHERE {className}Id IN (@ids)";

            //Khởi tạo các biến cần thiết
            DynamicParameters parameters = new DynamicParameters();
            var idsArray = "";

            //Lấy ra danh sách id
            foreach ( var id in ids)
            {
                idsArray += $"{id},";
            }

            //Bỏ ký tự "," cuối cùng
            idsArray = idsArray.Substring(0, idsArray.Length - 1);

            parameters.Add("@ids", idsArray);

            //Xóa
            int res = Connection.Execute(sql, parameters);

            //Trả về dữ liệu
            return res;
        }

        /// <summary>
        /// Lấy ra danh sách các bản ghi
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <returns>Danh sách các bản ghi</returns>
        public IEnumerable<T> GetAll<T>()
        {
            //Lấy ra tên đối tượng
            var className = typeof(T).Name;

            //Câu lệnh truy vấn
            var sql = $"SELECT * FROM {className}";

            //Lấy dữ liệu
            var list = Connection.Query<T>(sql);

            //Trả về dữ liệu
            return list;
        }

        /// <summary>
        /// Lấy ra 1 bản ghi với id tương ứng
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <param name="id">id</param>
        /// <returns>Dữ liệu của id tương ứng</returns>
        public T? GetById<T>(Guid id)
        {
            var className = typeof(T).Name;

            //Câu lệnh truy vấn
            var sql = $"SELECT * FROM {className} WHERE {className}Id = @id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@id", id);

            //Lấy dữ liệu
            var department = Connection.QueryFirstOrDefault<T>(sql, parameters);

            return department;
        }

        /// <summary>
        /// Thêm mới 1 bản ghi
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <param name="data">dữ liệu</param>
        /// <returns>Số bản ghi thêm thành công</returns>
        public int Insert<T>(T data)
        {
            var className = typeof(T).Name;

            //Tạo mới 1 id
            var propertyId = data.GetType().GetProperties()[0];
            propertyId.SetValue(data, Guid.NewGuid());

            //Chuỗi truy vấn thêm dữ liệu
            var sql = "Proc_Add" + className;

            //Thêm dữ liệu
            int res = AddAndEditData(data, Connection, sql);

            return res;
        }

        /// <summary>
        /// Thêm mới hoặc sửa 1 bản ghi
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <param name="data">dữ liệu</param>
        /// <param name="connection">chuỗi kết nối</param>
        /// <param name="sql">câu lệnh truy vấn</param>
        /// <returns>Số bản ghi đã thêm hoặc sửa thành công</returns>
        private int AddAndEditData<T>(T data, IDbConnection connection, string sql)
        {
            //Mở kết nối đến database
            connection.Open();

            // Tạo đối tượng DynamicParameters để lưu các tham số
            var parameters = new DynamicParameters();

            // Lấy danh sách các thuộc tính từ đối tượng Department
            var properties = data.GetType().GetProperties();

            foreach (var prop in properties)
            {
                // Tạo tên tham số cho DynamicParameters
                var paramName = $"@m_{prop.Name}";
                var entityProperty = data.GetType().GetProperty(prop.Name);
                if (entityProperty != null)
                {
                    var propValue = prop.GetValue(data);
                    // Thêm tham số vào DynamicParameters
                    parameters.Add(paramName, propValue);
                }
                else
                {
                    parameters.Add(paramName, null);
                }
            }

            //Thêm hoặc sửa dữ liệu
            int res = connection.Execute(sql: sql, param: parameters, commandType: CommandType.StoredProcedure);

            return res;
        }

        /// <summary>
        /// Sửa 1 bản ghi theo id
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <param name="data">dữ liệu</param>
        /// <param name="id">id</param>
        /// <returns>Số bản ghi sửa thành công</returns>
        public int Update<T>(T data, Guid id)
        {
            var className = typeof(T).Name;

            //Giữ id cũ
            var propertyId = data.GetType().GetProperties()[0];
            propertyId.SetValue(data, id);

            //Chuỗi truy vấn sửa
            var sql = "Proc_Update" + className;

            //Sửa dữ liệu
            int res = AddAndEditData(data, Connection, sql);

            return res;
        }

        /// <summary>
        /// Kiểm tra mã không cho trùng
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <param name="code">mã</param>
        /// <returns>
        /// true - trùng
        /// false - không trùng
        /// </returns>
        public bool CheckCode<T>(string code)
        {
            var className = typeof(T).Name;
            //Truy vấn dữ liệu
            var sql = $"SELECT {className}Code FROM {className} WHERE {className}Code = @code";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@code", code);

            //Lấy dữ liệu
            var checkCode = Connection.QueryFirstOrDefault<string>(sql, parameters);

            //Trả về kết quả
            if (checkCode != null)
                return true;
            return false;
        }

        /// <summary>
        /// Kiểm tra CMTND-CCCD không được trùng
        /// </summary>
        /// <typeparam name="T">kiểu đối tượng</typeparam>
        /// <param name="identityNumber">số CMTND/CCCD</param>
        /// <returns>
        /// true - trùng
        /// false - không trùng
        /// </returns>
        public bool CheckIdentityNumber<T>(string identityNumber)
        {
            var className = typeof(T).Name;
            //Truy vấn dữ liệu
            var sql = $"SELECT IdentityNumber FROM {className} WHERE IdentityNumber = @identityNumber";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@identityNumber", identityNumber);

            //Lấy dữ liệu
            var check = Connection.QueryFirstOrDefault<string>(sql, parameters);

            //Trả về kết quả
            if (check != null)
                return true;
            return false;
        }
    }
}
