using System.Threading;
using System.Threading.Tasks;

namespace PowCapServer.Abstractions;

public interface ISerializer
{
    Task<byte[]> SerializeAsync<T>(T value, CancellationToken cancellationToken = default);

    Task<T> DeserializeAsync<T>(byte[] data, CancellationToken cancellationToken = default);
}
