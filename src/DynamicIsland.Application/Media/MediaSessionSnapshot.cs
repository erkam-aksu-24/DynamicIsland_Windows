namespace DynamicIsland.Application.Media;

public record MediaSessionSnapshot(
    string SessionId,
    bool IsPlaying,
    DateTimeOffset LastChangedUtc);
