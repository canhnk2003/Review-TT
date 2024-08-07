using Microsoft.AspNetCore.Http;
using MISA.CukCuk.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MISA.CukCuk.Core.Entities;
using System.Net;

namespace MISA.CukCuk.Core.Exceptions
{
    /// <summary>
    /// Xử lý các lỗi ngoại lệ
    /// </summary>
    /// Created By: NKCanh - 06/08/2024
    public class HandleExceptionMiddleware
    {
        private RequestDelegate _next;

        public HandleExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ErrorValidDataException ex)
            {
                Service errorService = new Service();
                errorService.UserMsg = ex.Message;
                errorService.Data = ex.Data;
                errorService.Error = ex.ErrorValidData;
                var res = JsonConvert.SerializeObject(errorService);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(res);
            }
            catch (ErrorNotFoundException ex)
            {
                await ErrorException(context, ex, 404);
            }
            catch (ErrorDeleteException ex)
            {
                await ErrorException(context, ex, 200);
            }
            catch (ErrorCreateException ex)
            {
                await ErrorException(context, ex, 200);
            }
            catch (ErrorEditException ex)
            {
                await ErrorException(context, ex, 200);
            }
            catch (Exception ex)
            {
                Service errorService = new Service();
                errorService.DevMsg = ex.Message;
                errorService.UserMsg = Resources.ResourceVN.Error_Exception;
                errorService.Data = ex.Data;
                var res = JsonConvert.SerializeObject(errorService);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(res);
            }
        }

        private async Task ErrorException(HttpContext context, Exception ex, int statusCode)
        {
            Service errorService = new Service();
            errorService.UserMsg = ex.Message;
            errorService.Data = ex.Data;
            var res = JsonConvert.SerializeObject(errorService);
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(res);
        }
    }
}
