namespace DynamicIsland.Application.Media;

public record MediaSessionsSnapshot(
    string SessionId,
    bool IsPlaying,
    DateTimeOffset LastChangedUtc);
