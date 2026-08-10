using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ProfiDesktop.Models;

namespace ProfiDesktop.Services;

public class ApiService
{
    public static readonly ApiService Instance = new();

    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOpts = new()
        { PropertyNameCaseInsensitive = true };

    public string? AccessToken {get; private set; }
    public string? RefreshToken {get; private set; }
    public string? Role {get; private set; }

    private ApiService(string baseUrl = "http://localhost:5222")
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }


    public async Task<string?> LoginAsync(string email, string password)
    {
        try
        {
            var responce = await _http.PostAsJsonAsync("/auth/login",
                new LoginRequest(email, password));

            var result = await responce.Content.ReadFromJsonAsync<ApiResponce<AuthData>>(JsonOpts);

            if (result is null) return "Не удалось разобрать ответ сервера";

            if (!result.success || result.data is null)
                return result.message ?? "Неверный email или пароль";

            AccessToken = result.data.AccessToken;
            RefreshToken = result.data.RefreshToken;
            Role = result.data.Role;

            _http.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            return null;
        }
        catch (HttpRequestException)
        {
            return "Сервер недоступен. Проверьте подключение";
        }
        catch (Exception ex)
        {
            return $"Ошибка - {ex.Message}";
        }
    }

    public async Task<User?> GetMe()
    {
        try
        {
            var responce = await _http.GetFromJsonAsync<ApiResponce<User>>("/users/me");
            return responce?.data;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<Activ?> GetSummary()
    {
        try
        {
            var responce = await _http.GetFromJsonAsync<ApiResponce<Activ>>("/dashboard/summary");
            return responce?.data;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<TopTechList?> GetTopTechList()
    {
        try
        {
            var responce = await _http.GetFromJsonAsync<ApiResponce<TopTechList>>("/skills/top");
            return responce?.data;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }


    public async Task<ApiResponce<UserSearch>> GetUsersSearchAsync(
        string? technology = null,
        int minLevel = 0,
        int maxLevel = 10,
        double minRating = 0,
        int minExp = 0,
        int page = 1
    )
    {
        try
        {
            var responce = await _http.GetFromJsonAsync<ApiResponce<UserSearch>>($"/users/search?minLevel={minLevel}&minRating={minRating}&minExp={minExp}&maxLevel={maxLevel}&technology={technology}&page={page}"); 
            return responce;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<List<string>?> GetSuggestSearchAsync(string query)
    {
        try
        {
            var responce = await _http.GetFromJsonAsync<ApiResponce<List<String>>>($"/skills/suggest?q={query}");
            return responce?.data;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    } 

}



public class SearchUserResultItem
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string[] Skills { get; set; }
    public double CompetencyIndex { get; set; }
    public double TrustLevel { get; set; }
    public string Trend { get; set; }  // "up", "stable", "down"
}

public record UserSearch(int Total, List<SearchUserResultItem> Users);

public class TopTech
{
    public string Name { get; set; }
    public int Count { get; set; }
    public double Percent { get; set; }
}
public class TopTechList
{
    public List<TopTech> Items { get; set; } 
}


public class Activ
{
    public int ActiveProfiles { get; set; }
    public int ProfilesDelta { get; set; }
    public int ProfilesDeltaWeek { get; set; }
    public double AvgRating { get; set; }
    public double AvgRatingDelta { get; set; }
    public double AvgRatingDeltaWeek { get; set; }
    public double VacancyMatch { get; set; }
    public double VacancyMatchDelta { get; set; }
    public double VacancyMatchDeltaWeek { get; set; }
}

public partial class User
{
    public int Id { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Role { get; set; }

    public DateTime RegisteredAt { get; set; }
    
    public Rating  Rating { get; set; }
    public List<Skill> Skills { get; set; } = null!;
}

public class Rating
{
    public int Id { get; set; }
    public decimal CompetencyIndex { get; set; }
    public decimal TrustLevel { get; set; }
    public int ConfirmsCount { get; set; }
    public DateTime CalculateAt { get; set; }
}
public class Skill
{
    public string? Technology{ get; set; }
    public short? Skilllevel { get; set; }
}