using System.Numerics;
using Astra.Components.Internal;
using Astra.Data;
using Astra.Styles;
using Astra.Util;
using Hexa.NET.ImGui;

namespace Astra.Managers;

public static unsafe class NotificationManager
{
    private static readonly List<Notification> notifications = [];
    private static NotificationStyle style;

    public static void SetStyle(NotificationStyle notificationStyle) => style = notificationStyle;

    public static void AddNotification(string title, string message, float duration, NotificationLocation location)
    {
        notifications.Add(new Notification(title, message, duration, location));
    }

    public static void ShowNotification(string title, string message, float duration)
    {
        AddNotification(title, message, duration, NotificationLocation.BottomRight);
    }

    internal static void RenderNotifications()
    {
        var currentTime = DateTime.Now;
        notifications.RemoveAll(n => (currentTime - n.StartTime).TotalSeconds > n.Duration);

        for (int i = 0; i < notifications.Count; i++)
        {
            var notification = notifications[i];
            float elapsed = (float)(currentTime - notification.StartTime).TotalSeconds;
            float alpha = Math.Min(elapsed / 0.5f, 1.0f); // Fade in for 0.5 seconds

            // Calculate fade-out effect
            if (elapsed > notification.Duration - 0.5f)
            {
                alpha = Math.Min((notification.Duration - elapsed) / 0.5f, 1.0f);
            }
            ImGui.PushFont(style.Font.GetImFont());
            Vector2 position = getNotificationPosition(notification, i, ImGui.CalcTextSize(notification.Message).X);
            ImGui.SetNextWindowPos(position, ImGuiCond.Always, new Vector2());
            ImGui.SetNextWindowBgAlpha(alpha);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, style.Radius);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, style.BorderThickness);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, style.BackgroundColor.ToVector4((byte)(alpha * 255)));
            ImGui.PushStyleColor(ImGuiCol.Border, style.BorderColor.ToVector4((byte)(alpha * 255)));
            ImGui.PushStyleColor(ImGuiCol.Text, style.TextColor.ToVector4((byte)(alpha * 255)));
            if (ImGui.Begin($"ntf_{notification.Title}_{notification.GetHashCode()}", null, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar))
            {
                ImGui.Text(notification.Message);
            }
            ImGui.PopFont();
            ImGui.PopStyleColor(3);
            ImGui.PopStyleVar(3);
            ImGui.End();
        }
    }

    private static Vector2 getNotificationPosition(Notification notification, int index, float textWidth)
    {
        ImGuiIO* io = ImGui.GetIO();
        Vector2 position = new Vector2();
        float offset = index * 30;

        switch (notification.Location)
        {
            case NotificationLocation.TopCenter:
                position = new Vector2((io->DisplaySize.X - textWidth) / 2, Titlebar.GetHeight(Application.InternalWindow) + 10 + offset);
                break;
            case NotificationLocation.TopRight:
                position = new Vector2(io->DisplaySize.X - (20 + textWidth), Titlebar.GetHeight(Application.InternalWindow) + 10 + offset);
                break;
            case NotificationLocation.TopLeft:
                position = new Vector2(10, Titlebar.GetHeight(Application.InternalWindow) + 10 + offset);
                break;
            case NotificationLocation.BottomRight:
                position = new Vector2(io->DisplaySize.X - (20 + textWidth), io->DisplaySize.Y - 40 - offset);
                break;
            case NotificationLocation.BottomLeft:
                position = new Vector2(10, io->DisplaySize.Y - 40 - offset);
                break;
        }
        return position;
    }
}