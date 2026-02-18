using System.Text.Json;
using System.Text.RegularExpressions;
using PremierLeague_Backend.Models.SelectListItems;

namespace PremierLeague_Backend.Helper;

public static class NationalityLoader
{
    public static NationalityList LoadFromPath(string jsonFilePath)
    {
        var raw = File.ReadAllText(jsonFilePath);
        raw = raw.Trim();

        if (raw.Length > 0 && raw[0] == '"' && (raw.Contains("\\n") || raw.Contains("\\r")))
        {
            try
            {
                var once = JsonSerializer.Deserialize<string>(raw);
                return JsonSerializer.Deserialize<NationalityList>(once!, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
            catch
            {
                var unescaped = Regex.Unescape(raw.Trim('"'));
                return JsonSerializer.Deserialize<NationalityList>(unescaped, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
        }

        return JsonSerializer.Deserialize<NationalityList>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}
