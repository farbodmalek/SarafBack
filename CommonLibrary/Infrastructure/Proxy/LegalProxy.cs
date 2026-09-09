using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto.Legal;
using CommonLibrary.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace CommonLibrary.Infrastructure.Proxy
{
    public class LegalProxy : ILegalProxy
    {
        private readonly IAccessManager accessManager;
        private IConfiguration configuration { set; get; }
        private string host;
        public LegalProxy(IConfiguration configuration, IAccessManager accessManager)
        {
            this.configuration = configuration;
            this.accessManager = accessManager;
            host = this.configuration.GetSection("LegalHub_URL").Value;
        }
        public async Task<bool> SetPrimaryLegalDoc(PrimaryLegalDocDto primaryLegalDocDto)
        {
            RestClient client = new RestClient();
            //host = this.configuration.GetSection("LegalHub_URL").Value;
            var requestUri = host + "set/primaryLegalDoc";

            var request = new RestRequest(requestUri, Method.Post);
            request.AddHeader("Authorization", "Bearer " + accessManager.GetRequestToken());
            //request.AddJsonBody(primaryLegalDocDto);

            var stringPayload = Newtonsoft.Json.JsonConvert.SerializeObject(primaryLegalDocDto);
            request.AddStringBody(stringPayload, DataFormat.Json);
            request.AddHeader("content-type", "application/json; charset=utf-8");
            var responce = await client.ExecuteAsync<ResultObject<int>>(request);
            var data = ((RestResponse<ResultObject<int>>)responce).Data;
            if (data != null && (data.ServerErrors == null || data.ServerErrors?.Count == 0))
                return true;
            return false;
        }

        public async Task<bool> SetGuarantorsLegal(int PmId)
        {
            RestClient client = new RestClient();
            var requestUri = host + @$"set/Guarantors/{PmId}";
            var request = new RestRequest(requestUri, Method.Get);
            request.AddHeader("Authorization", "Bearer " + accessManager.GetRequestToken());
            var responce = await client.ExecuteAsync<ResultObject<int>>(request);
            var data = ((RestResponse<ResultObject<int>>)responce).Data;
            if (data != null && (data.ServerErrors == null || data.ServerErrors?.Count == 0))
                return true;
            return false;
        }
        public async Task<bool> SetCustomerLegal(int PmId)
        {
            RestClient client = new RestClient();
            var requestUri = host + @$"set/customers/{PmId}";

            var request = new RestRequest(requestUri, Method.Get);
            request.AddHeader("Authorization", "Bearer " + accessManager.GetRequestToken());
            var responce = await client.ExecuteAsync<ResultObject<int>>(request);
            var data = ((RestResponse<ResultObject<int>>)responce).Data;
            if (data != null && (data.ServerErrors == null || data.ServerErrors?.Count == 0))
                return true;
            return false;
        }
        public async Task<bool> SetGuarantessLegal(int PmId)
        {
            RestClient client = new RestClient();
            var requestUri = host + @$"set/guarantess/{PmId}";
            var request = new RestRequest(requestUri, Method.Get);
            request.AddHeader("Authorization", "Bearer " + accessManager.GetRequestToken());
            var responce = await client.ExecuteAsync<ResultObject<int>>(request);
            var data = ((RestResponse<ResultObject<int>>)responce).Data;
            if (data != null && (data.ServerErrors == null || data.ServerErrors?.Count == 0))
                return true;
            return false;
        }
    }
    public class LegalResultObject<T> 
    {
        public T Result { get; set; }

        public List<ServerErr> ServerErrors { get; set; } = new List<ServerErr>();
    }
    public class ServerErr
    {
        public string Hint { get; set; }
        public int Type { get; set; }
        public Object stackTrace { get; set; }
        public string LineNumber { get; set; }
        public string FileName { get; set; }
    }
}
