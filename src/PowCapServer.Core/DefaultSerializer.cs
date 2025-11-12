#if !NETSTANDARD2_0

using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PowCapServer.Abstractions;

namespace PowCapServer;

public class DefaultSerializer : ISerializer
{
    public async Task<byte[]> SerializeAsync<T>(T value, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, value, cancellationToken: cancellationToken).ConfigureAwait(false);
        return stream.ToArray();
    }

    public async Task<T> DeserializeAsync<T>(byte[] data, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(data);
        var result = await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result!;
    }
}

#endif
