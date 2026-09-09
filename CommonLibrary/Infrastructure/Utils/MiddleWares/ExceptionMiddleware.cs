using CommonLibrary.Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace CommonLibrary.Infrastructure.Utils.MiddleWares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ExceptionMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                var task = await setSystemLog(httpContext);
                if (task && httpContext.Response.StatusCode != 401)
                    await _next(httpContext);
            }
            catch (SqlException sql_exception)
            {
                await HandleExceptionAsync(httpContext, sql_exception);
            }
            catch (OverflowException ex)
            {
                await HandleExceptionAsync(httpContext, ex, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(httpContext, ex, (int)HttpStatusCode.Unauthorized);
            }
            catch (MemberAccessException ex)
            {
                await HandleExceptionAsync(httpContext, ex, (int)HttpStatusCode.Forbidden);
            }
            catch (DivideByZeroException ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task<bool> setSystemLog(HttpContext httpContext)
        {
            int id = 0;
            return true;
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception, int responseCode = (int)HttpStatusCode.OK)
        {
            context.Response.ContentType = "application/json";
            string message = exception.Message;
            if (responseCode == (int)HttpStatusCode.Forbidden)
            {
                message = "شما دسترسی لازم را  ندارید";
                responseCode = (int)HttpStatusCode.OK;
            }
             else if (responseCode == (int)HttpStatusCode.TooManyRequests)
            {
                message = "تعداد فراخوانی زیاد، بعد از چند ثانیه دوباره تلاش کنید";
                responseCode = (int)HttpStatusCode.OK;
            }

            //else 
            //{
            //    message = "خطا به راهبر سیستم گزارش دهید";
              
            //}

            context.Response.StatusCode = responseCode;
            ResultObject<bool> resultObject = new ResultObject<bool>();
            resultObject.ServerErrors.Add(new ServerError() { Code = responseCode, Hint = message });
            //saveLogText(context, exception);
            await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(resultObject, new Newtonsoft.Json.JsonSerializerSettings()
            { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
        private void saveLogText(HttpContext context, Exception e)
        {
            var erpath = @"wwwroot\exceptionLogexception.txt";
            using (StreamWriter sw = new StreamWriter(erpath))
            {
                var inner = "";
                if (e.InnerException != null)
                    inner = e.InnerException.ToString();
                sw.Write(DateTime.Now +
                    Environment.NewLine + e.ToString() +
                    Environment.NewLine + inner +
                    Environment.NewLine + "--- " + e.ToString()
                    );
            }

        }

    }
}
