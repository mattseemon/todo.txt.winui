using Newtonsoft.Json;

namespace Seemon.Todo.Helpers.Common;

public static class JsonHelper
{
    public static async Task<T> ToObjectAsync<T>(string value)
        => await Task.Run<T>(() =>
        {
            return JsonConvert.DeserializeObject<T>(value);
        });

    public static async Task<string> StringifyAsync(object value)
        => await Task.Run(() =>
        {
            return JsonConvert.SerializeObject(value);
        });
}
