using System.Text.Json;

namespace Cbd.Api.Tools;

public static class JsonExtensions
{
    public static string ToJson<T>(this T obj)
        => JsonSerializer.Serialize(obj, ops);

    static readonly JsonSerializerOptions ops = new()
    {
        WriteIndented = true
    };
}
