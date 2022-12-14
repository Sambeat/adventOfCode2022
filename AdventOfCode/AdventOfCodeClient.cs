using System.Text;
using AdventOfCode.Configs;
using AdventOfCode.Events.Leaderboards;
using Newtonsoft.Json;

namespace AdventOfCode.Events.Puzzles;

public class AdventOfCodeClient
{
    private HttpClient HttpClient { get; }

    internal static AdventOfCodeClient DefaultInstance => new AdventOfCodeClient(ConfigManager.AdventOfCodeConfig);

    private AdventOfCodeClient(AdventOfCodeConfig adventOfCodeConfig)
    {
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(adventOfCodeConfig.BaseUrl ?? "https://adventofcode.com")
        };
        
        if (adventOfCodeConfig.SessionConfig?.Cookie == null)
        {
            throw new ArgumentNullException("SessionConfig.Cookie", "SessionConfig.Cookie not found in appsettings.json. Please create an appsettings.json file in the AdventOfCode project and add the cookie value found in the GET adventofcode.com Request Header named 'Cookie' ");
        }
        
        HttpClient.DefaultRequestHeaders.Add("Cookie", adventOfCodeConfig.SessionConfig?.Cookie);
    }

    internal async Task<string> GetInputAsync(int year, int day)
    {
        var response = await HttpClient.GetAsync($"/{year}/day/{day}/input");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    internal async Task<string> PostAnswerAsync(int year, int day, string answer, int level)
    {
        var response = await HttpClient.PostAsync($"/{year}/day/{day}/answer", new StringContent($"level={level}&answer={answer}", Encoding.UTF8, "application/x-www-form-urlencoded"));
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
    
    internal async Task<Leaderboard?> GetLeaderboardAsync(int year)
    {
        var response = await HttpClient.GetAsync($"/{year}/leaderboard/private/view/232410.json");
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<Leaderboard>(await response.Content.ReadAsStringAsync());
    }
}