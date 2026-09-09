using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace CommonLibrary.Infrastructure.Validators
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Code { get; set; }
        public string Hint { get; set; }

        public ValidationError(string field, int code, string message)
        {
            Field = field != string.Empty ? field : null;
            Code = code != 0 ? code : 55;
            Hint = message;
        }
    }

    public class ValidationResultModel
    {
        public bool Result
        {
            get => this.ServerErrors.Count == 0;
        }
        public int Data { get; set; }

        public List<ValidationError> ServerErrors { get; }

        public ValidationResultModel(ModelStateDictionary modelState)
        {
            ServerErrors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, 0, x.ErrorMessage)))
                    .ToList();
        }
    }
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState)
            : base(new ValidationResultModel(modelState))
        {
            StatusCode = StatusCodes.Status200OK; //change the http status code to 422.
        }
    }
}
