using System.Net.Http.Json;
using FreeParkingMaui.Models;

namespace FreeParkingMaui.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(Constants.ApiBaseUrl);
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await SecureStorage.GetAsync(Constants.AuthTokenKey);
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<VerificationCodeResponse> GetVerificationCodeAsync(string phone, string appId)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            var response = await _httpClient.PostAsync($"/api/Customer/GetVerificationCode?phoneNumber={phone}&AppId={appId}", null);
            return await response.Content.ReadFromJsonAsync<VerificationCodeResponse>();
        }
        catch (Exception ex)
        {
            return new VerificationCodeResponse { Status = 0, Message = ex.Message };
        }
    }

    public async Task<LoginResponse> LoginAsync(string phone, string code, string areaId)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            var payload = new { phone, code, areaId };
            var response = await _httpClient.PostAsJsonAsync("/api/application/user/phoneAuthorization", payload);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result;
            }
            return new LoginResponse { Status = 0, Message = "Login failed: " + response.ReasonPhrase };
        }
        catch (Exception ex)
        {
            return new LoginResponse { Status = 0, Message = ex.Message };
        }
    }

    public async Task<OrderResponse> GetLatestOrderAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync("/api/Order/newestOrder");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Token Expired
                return new OrderResponse { Status = 401, Message = "Unauthorized" };
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderResponse>();
            }
            
            return new OrderResponse { Status = 0, Message = "Request failed: " + response.ReasonPhrase };
        }
        catch (Exception ex)
        {
            return new OrderResponse { Status = 0, Message = ex.Message };
        }
    }

    public async Task<PayResponse> PayManualOrderAsync(string areaId, string carNumber)
    {
        try
        {
            await SetAuthHeaderAsync();
            var payload = new 
            { 
                PayType = 4, 
                AreaId = areaId, 
                carNumber = carNumber, 
                CarPointCode = "", 
                PayOrderIdList = new List<string>() 
            };
            var response = await _httpClient.PostAsJsonAsync("/api/OrderRefact/OrderPay", payload);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new PayResponse { Status = 401, Message = "Unauthorized" };
            }

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PayResponse>();
            }
            
            return new PayResponse { Status = 0, Message = "Payment failed: " + response.ReasonPhrase };
        }
        catch (Exception ex)
        {
            return new PayResponse { Status = 0, Message = ex.Message };
        }
    }
}
