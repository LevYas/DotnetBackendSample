using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SugarCounter.Api.Controllers.Food
{
    // specification: https://trackapi.nutritionix.com/docs/
    // docs: https://docs.google.com/document/d/1_q-K-ObMTZvO0qUEAxROrN3bwMujwAN25sLHwJzliK0/edit#
    // demo: https://www.nutritionix.com/natural-demo

    public class NutritionixClient : INutritionDataProvider
    {
        private readonly HttpClient _client;
        private readonly ILogger<NutritionixClient> _logger;

        public NutritionixClient(IConfiguration configuration, HttpClient client, ILogger<NutritionixClient> logger)
        {
            _client = client;
            _logger = logger;

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            IConfigurationSection keys = configuration.GetSection("NutritionixKeys");

            _client.DefaultRequestHeaders.Add("x-app-id", keys["AppId"]);
            _client.DefaultRequestHeaders.Add("x-app-key", keys["AppKey"]);
            _client.DefaultRequestHeaders.Add("x-remote-user-id", "0");
            _client.BaseAddress = new Uri($"{keys["ServerUrl"]}v2/");
        }

        public async Task<int?> GetSugarInFood(string foodDescription)
        {
            var requestDto = new NutritionRequestDto(foodDescription);
            NutritionResponseDto resultDto;

            try
            {
                using HttpResponseMessage response = await _client.PostAsJsonAsync("natural/nutrients", requestDto);
                resultDto = await readResponse<NutritionResponseDto>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to receive data from Nutritionix");
                return null;
            }

            return (int)Math.Round(resultDto.Foods.Select(f => f.Sugar).Sum());
        }

        private async Task<TRes> readResponse<TRes>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            string stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TRes>(stringResponse);
        }
    }

    public class NutritionRequestDto
    {
        public NutritionRequestDto(string query) => Query = query;

        [JsonProperty("query")]
        public string Query { get; set; }
    }

    public class NutritionixFoodItemDto
    {
        [JsonProperty("food_name")]
        public string FoodName { get; set; } = default!;

        [JsonProperty("nf_sugars")]
        public float Sugar { get; set; }
    }

    public class NutritionResponseDto
    {
        [JsonProperty("foods")]
        public List<NutritionixFoodItemDto> Foods { get; set; } = new List<NutritionixFoodItemDto>();
    }
}
