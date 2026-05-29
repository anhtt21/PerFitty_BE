using System.Net.Sockets;
using Hnanut.PerFitty.Application.Abstractions;

namespace Hnanut.PerFitty.Infrastructure.Persistence;

public sealed class SqlServerPersistenceAvailabilityProbe : IPersistenceAvailabilityProbe
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);

    private readonly string _connectionString;

    public SqlServerPersistenceAvailabilityProbe(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        var endpoint = TryParseTcpEndpoint(_connectionString);
        if (endpoint is null)
        {
            return true;
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(Timeout);

        using var client = new TcpClient();

        try
        {
            await client.ConnectAsync(endpoint.Value.Host, endpoint.Value.Port, timeout.Token);
            return true;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return false;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private static (string Host, int Port)? TryParseTcpEndpoint(string connectionString)
    {
        var dataSource = GetConnectionStringValue(connectionString, "Server")
            ?? GetConnectionStringValue(connectionString, "Data Source")
            ?? GetConnectionStringValue(connectionString, "Address")
            ?? GetConnectionStringValue(connectionString, "Addr")
            ?? GetConnectionStringValue(connectionString, "Network Address");

        if (string.IsNullOrWhiteSpace(dataSource))
        {
            return null;
        }

        dataSource = dataSource.Trim();

        if (dataSource.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase))
        {
            dataSource = dataSource[4..];
        }

        if (dataSource.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var host = dataSource;
        var port = 1433;
        var commaIndex = dataSource.LastIndexOf(',');

        if (commaIndex > 0)
        {
            host = dataSource[..commaIndex];
            if (int.TryParse(dataSource[(commaIndex + 1)..], out var parsedPort))
            {
                port = parsedPort;
            }
        }

        return string.IsNullOrWhiteSpace(host) ? null : (host.Trim(), port);
    }

    private static string? GetConnectionStringValue(string connectionString, string key)
    {
        var segments = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in segments)
        {
            var equalsIndex = segment.IndexOf('=', StringComparison.Ordinal);
            if (equalsIndex <= 0)
            {
                continue;
            }

            var segmentKey = segment[..equalsIndex].Trim();
            if (!segmentKey.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return segment[(equalsIndex + 1)..].Trim();
        }

        return null;
    }
}
