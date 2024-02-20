using Newtonsoft.Json;
using System.Text;

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

    public static void SerializeAndWrite<T>(this Stream stream, T objectToSerialize)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead) throw new NotSupportedException("can't read the stream.");
        using var streamWriter = new StreamWriter(stream, new UTF8Encoding(), 1024, true);
        using var jsonTextWriter = new JsonTextWriter(streamWriter);
        var jsonSerializer = new JsonSerializer();
        jsonSerializer.Serialize(jsonTextWriter, objectToSerialize);
        jsonTextWriter.Flush();
    }
}
