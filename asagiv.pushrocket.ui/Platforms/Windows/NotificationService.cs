using asagiv.pushrocket.ui.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;

namespace asagiv.pushrocket.ui
{
    public class NotificationService : INotificationService
    {
        public void ShowNotification(string title, string body)
        {
            new ToastContentBuilder()
                .AddToastActivationInfo(null, ToastActivationType.Foreground)
                .AddText(title, hintStyle: AdaptiveTextStyle.Header)
                .AddText(body, hintStyle: AdaptiveTextStyle.Body)
                .Show();
        }
    }
}
