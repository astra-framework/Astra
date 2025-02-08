namespace Astra.Data;

internal class Notification(string title, string message, float duration, NotificationLocation location)
{
    public string Title { get; } = title;
    public string Message { get; } = message;
    public DateTime StartTime { get; } = DateTime.Now;
    public float Duration { get; } = duration;
    public NotificationLocation Location { get; } = location;
}

public enum NotificationLocation
{
    TopCenter,
    TopRight,
    TopLeft,
    BottomRight,
    BottomLeft,
}