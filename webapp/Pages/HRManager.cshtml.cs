using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Web_App.Authorization;
using Web_App.DTO;

namespace AspNetIdentity.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        [BindProperty]
        public List<WeatherForecastDTO> WeatherForecastItems { get; set; }

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            WeatherForecastItems = await InvokeEndPoint<List<WeatherForecastDTO>>("access_token", "WeatherForecast");
        }

        private async Task<T> InvokeEndPoint<T>(string clientName, string url)
        {
            JwtToken token = null;

            string strTokenFromSession = HttpContext.Session.GetString(clientName);
            if (string.IsNullOrWhiteSpace(strTokenFromSession))
            {
                token = await Authenticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenFromSession);
            }

            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.Now)
            {
                token = await Authenticate();
            }

            var httpClient = _httpClientFactory.CreateClient("ybWebApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return await httpClient.GetFromJsonAsync<T>(url);
        }

        private async Task<JwtToken> Authenticate()
        {
            var httpClient = _httpClientFactory.CreateClient("ybWebApi");
            var res = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" });
            res.EnsureSuccessStatusCode();
            string strJwt = await res.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", strJwt);

            return JsonConvert.DeserializeObject<JwtToken>(strJwt);
        }
    }
}
