using System.Runtime.CompilerServices;
using System.Text;

namespace AiSdk.Core.Streaming;

/// <summary>
/// Parses Server-Sent Events (SSE) from a stream.
/// </summary>
public static class ServerSentEventsParser
{
    /// <summary>
    /// Parses Server-Sent Events from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of server-sent events.</returns>
    public static async IAsyncEnumerable<ServerSentEvent> ParseAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        var currentEvent = new ServerSentEventBuilder();

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (line is null)
            {
                break;
            }

            // Empty line indicates end of event
            if (string.IsNullOrEmpty(line))
            {
                if (currentEvent.HasData)
                {
                    yield return currentEvent.Build();
                    currentEvent = new ServerSentEventBuilder();
                }
                continue;
            }

            // Ignore comments
            if (line.StartsWith(':'))
            {
                continue;
            }

            // Parse field
            var colonIndex = line.IndexOf(':');
            if (colonIndex == -1)
            {
                currentEvent.AppendField(line, string.Empty);
            }
            else
            {
                var field = line[..colonIndex];
                var value = colonIndex + 1 < line.Length && line[colonIndex + 1] == ' '
                    ? line[(colonIndex + 2)..]
                    : line[(colonIndex + 1)..];

                currentEvent.AppendField(field, value);
            }
        }

        if (currentEvent.HasData)
        {
            yield return currentEvent.Build();
        }
    }
}

/// <summary>
/// Represents a Server-Sent Event.
/// </summary>
public record ServerSentEvent
{
    /// <summary>
    /// Gets the event type.
    /// </summary>
    public string? Event { get; init; }

    /// <summary>
    /// Gets the event data.
    /// </summary>
    public required string Data { get; init; }

    /// <summary>
    /// Gets the event ID.
    /// </summary>
    public string? Id { get; init; }
}

/// <summary>
/// Builder for constructing ServerSentEvent instances.
/// </summary>
internal class ServerSentEventBuilder
{
    private readonly StringBuilder _data = new();
    private string? _event;
    private string? _id;

    public bool HasData => _data.Length > 0;

    public void AppendField(string field, string value)
    {
        switch (field)
        {
            case "event":
                _event = value;
                break;
            case "data":
                if (_data.Length > 0)
                {
                    _data.Append('\n');
                }
                _data.Append(value);
                break;
            case "id":
                _id = value;
                break;
        }
    }

    public ServerSentEvent Build()
    {
        return new ServerSentEvent
        {
            Event = _event,
            Data = _data.ToString(),
            Id = _id
        };
    }
}
