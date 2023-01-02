﻿using asagiv.pushrocket.common.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;

namespace asagiv.pushrocket.ui
{
    public class NotificationService : INotificationService
    {
        public void ShowNotification(string title, string body, string args)
        {
            var contentBuilder = new ToastContentBuilder();

            if(!string.IsNullOrWhiteSpace(args))
            {
                contentBuilder = contentBuilder.SetProtocolActivation(new Uri(args));
            }

            contentBuilder.AddText(title, hintStyle: AdaptiveTextStyle.Header)
                .SetToastDuration(ToastDuration.Long)
                .AddText(body, hintStyle: AdaptiveTextStyle.Body)
                .Show();
        }
    }
}