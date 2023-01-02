﻿using Plugin.LocalNotification;

namespace asagiv.pushrocket.ui
{
    public class NotificationService : common.Interfaces.INotificationService
    {
        public void ShowNotification(string title, string body, string args) 
        {
            var notification = new NotificationRequest
            {
                Title = title,
                Description = body,
                Android =
                {
                    IconSmallName =
                    {
                        ResourceName = "pushrocket"
                    }
                }
            };

            LocalNotificationCenter.Current.Show(notification);
        } 
    }
}