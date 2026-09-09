using CommonLibrary.Core.Domain.Entities.Common;
using CommonLibrary.Infrastructure.JwtManagers;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace CommonLibrary.Infrastructure.Utils.MiddleWares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private HttpLog httpLog;

        public LoggerMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            httpLog = new HttpLog();
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string guid = string.Empty;
            var accessToken = context.Request.Headers["Authorization"];
            JwtSecurityToken jsonToken = new JwtSecurityToken();
            if (accessToken.Count > 0)
            {
                string jwt = accessToken[0].Replace("Bearer ", string.Empty);
                var simplePrinciple = JwtManager.GetPrincipal(jwt);
                var identity = simplePrinciple?.Identity as ClaimsIdentity;
                if (identity != null)
                    guid = identity.FindFirst("uid")?.Value ?? null;
            }
            try
            {
                CalculateElapsedTime(context);
                httpLog.UserName = guid;
                httpLog.MicroServiceName = Assembly.GetEntryAssembly()?.GetName()?.Name ?? "";
                httpLog.Host = context.Connection.RemoteIpAddress.ToString();
                httpLog.AgentInfo = context.Request.Headers["User-Agent"].ToString();
                await FormatRequest(context.Request);
                //await _next(context);
                //await FormatResponse(context.Response);
                //var originalBodyStream = context.Response.Body;
                //using (var responseBody = new MemoryStream())
                //{
                //    context.Response.Body = responseBody;
                //    await FormatResponse(context.Response);
                //    await responseBody.CopyToAsync(originalBodyStream);
                //}
                Stream originalBody = context.Response.Body;
                try
                {
                    using var memStream = new MemoryStream();
                    context.Response.Body = memStream;

                    // call to the following middleware 
                    // response should be produced by one of the following middlewares
                    await _next(context);
                    memStream.Position = 0;
                    string responseBody = new StreamReader(memStream).ReadToEnd();
                    httpLog.ResponseStatusCode = context.Response.StatusCode;
                    httpLog.ResponseDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    httpLog.ResponseContentType = context.Response.ContentType;
                    if (context.Response.StatusCode != 200)
                    {
                        httpLog.ResponseContent = responseBody;
                    }
                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }
                finally
                {
                    context.Response.Body = originalBody;
                }
            }
            finally
            {
                httpLog.UserName = httpLog.UserName;
                httpLog.ResponseContentType = httpLog.ResponseContentType;
                httpLog.ResponseContent = httpLog.ResponseContent;
                httpLog.ElapsedTime = httpLog.ElapsedTime;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("PaymentMonitoringConnectionString")))
                {
                    string sqlQuery = string.Empty;
                    sqlQuery = @"INSERT INTO [dbo].[HttpLogs]
                    ([UserName],[RequestMethod],[RequestUri], [QueryString],[RequestDate], [RequestContentType],[RequestContent], [ResponseStatusCode], [ResponseDate],[ResponseContentType], [ResponseContent],[ElapsedTime], [Host], [AgentInfo], [MicroServiceName])
            VALUES(@UserName,@RequestMethod,@RequestUri,@QueryString,@RequestDate,@RequestContentType,@RequestContent,@ResponseStatusCode,@ResponseDate,@ResponseContentType,@ResponseContent,@ElapsedTime,@Host,@AgentInfo,@MicroServiceName)";
                    await db.ExecuteAsync(sqlQuery, httpLog);
                }
            }
        }
        private void CalculateElapsedTime(HttpContext context)
        {
            var watch = new Stopwatch();
            watch.Start();
            context.Response.OnStarting(() =>
            {
                watch.Stop();
                httpLog.ElapsedTime = watch.ElapsedMilliseconds;
                return Task.CompletedTask;
            });
        }

        private async Task FormatRequest(HttpRequest request)
        {
            try
            {
                request.EnableBuffering();
                string requestBody = await new StreamReader(request.Body, Encoding.UTF8).ReadToEndAsync();
                request.Body.Position = 0;
                httpLog.RequestContent = requestBody;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception reading request: {ex.Message}");
            }
            //var body = request.Body;
            //request.EnableBuffering();
            //var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            //await request.Body.ReadAsync(buffer, 0, buffer.Length);
            //var bodyAsText = Encoding.UTF8.GetString(buffer);
            //request.Body = body;
            httpLog.RequestContentType = request.ContentType;
            httpLog.RequestMethod = request.Method;
            httpLog.RequestUri = request.Path;
            httpLog.RequestDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            //httpLog.RequestContent = bodyAsText;
            httpLog.QueryString = request.QueryString.Value;
        }

        private async Task FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            httpLog.ResponseStatusCode = response.StatusCode;
            httpLog.ResponseDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            if(response.StatusCode != 200)
                httpLog.ResponseContent = body;
            httpLog.ResponseContentType = response.ContentType;
        }
    }
}
