using Newtonsoft.Json;

namespace Movies.Client;
public static class StreamExtensions
{
    public static T ReadAndDeserializeFromJson<T>(this Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead) throw new NotSupportedException("can't read the stream.");
        using var streamReader = new StreamReader(stream);
        using var jsonTextReader = new JsonTextReader(streamReader);
        var jsonSerializer = new JsonSerializer();
        return jsonSerializer.Deserialize<T>(jsonTextReader)!;
    }
}
