using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.CukCuk.Core.Entities;
using MISA.CukCuk.Core.Interfaces;
using MISA.CukCuk.Infrastructure.DBContext;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MISA.CukCuk.Infrastructure.Repository
{
    /// <summary>
    /// Thực hiện các công việc ở IBaseRepository
    /// </summary>
    /// <typeparam name="T">Kiểu đối tượng</typeparam>
    /// Created By: NKCanh - 07/08/2024
    public class BaseRepository<T> : IDisposable,IBaseRepository<T> where T : class
    {
        
        IDBContext _dbContext;

        public BaseRepository(IDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Xóa dữ liệu với id tương ứng
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Số dữ liệu đã xóa thành công</returns>
        public int Delete(Guid id)
        {
            int res = _dbContext.Delete<T>(id);
            return res;
        }

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="ids">danh sách id</param>
        /// <returns>Số dữ liệu đã xóa thành công</returns>
        public int DeleteAny(Guid[] ids)
        {
            int res = _dbContext.DeleteAny<T>(ids);
            return res;
        }

        /// <summary>
        /// Lấy ra danh sách các bản ghi
        /// </summary>
        /// <returns>Danh sách các bản ghi</returns>
        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> list = _dbContext.GetAll<T>();
            return list;
        }

        /// <summary>
        /// Lấy ra đối tượng theo id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>Đối tượng tương ứng</returns>
        public T? GetById(Guid id)
        {
            var data = _dbContext.GetById<T>(id);
            return data;
        }

        public int Insert(T data)
        {
            int res = _dbContext.Insert<T>(data);
            return res;
        }

        public int Update(T data, Guid id)
        {
            int res = _dbContext.Update<T>(data, id);
            return res;
        }

        public bool CheckCode(string code)
        {
            bool checkCode = _dbContext.CheckCode<T>(code);
            return checkCode;
        }

        public bool CheckIdentityNumber(string identityNumber)
        {
            bool check = _dbContext.CheckIdentityNumber<T>(identityNumber);
            return check;
        }

        public void Dispose()
        {
            _dbContext.Connection.Dispose();
        }
    }
}
