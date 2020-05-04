using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Functional.ApiCallers
{
    public class ApiCaller
    {
        protected readonly HttpClient Client;
        public const string PasswordForUsers = "123";

        public ApiCaller(HttpClient client)
        {
            Client = client;
        }

        protected async Task<TRes> ReadResponse<TRes>(Task<HttpResponseMessage> response) =>
            await ReadResponse<TRes>(await response);

        protected async Task<TRes> ReadResponse<TRes>(HttpResponseMessage response)
        {
            string stringResponse = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<TRes>(stringResponse);
        }

        protected async Task EnsureSuccess(Task<HttpResponseMessage> responseTask)
        {
            HttpResponseMessage response = await responseTask;
            response.EnsureSuccessStatusCode();
        }

        protected string CombineUrl(string url, int? pageNumber = null, int? itemsPerPage = null)
        {
            var query = new Dictionary<string, string>();

            if (pageNumber != null)
                query.Add("pageNumber", pageNumber.Value.ToString());

            if (itemsPerPage != null)
                query.Add("itemsPerPage", itemsPerPage.Value.ToString());

            return QueryHelpers.AddQueryString(url, query);
        }
    }
}
