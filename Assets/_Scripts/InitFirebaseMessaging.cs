using UnityEngine;
using System;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS 
using Unity.Notifications.iOS;
#endif


public class InitFirebaseMessaging : MonoBehaviour {
	public void Start() {
	Application.targetFrameRate = 60;
#if UNITY_ANDROID
		var defaultChannel = new AndroidNotificationChannel() {
			Id = "default_channel",
			Name = "Default Channel",
			Description = "Default notification channel",
			Importance = Importance.Default,
		};

		AndroidNotificationCenter.RegisterNotificationChannel(defaultChannel);
#endif

		Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
		Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
	}

	

	public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
		Debug.Log("Received Registration Token: " + token.Token);
		SendAndReceive.SendDeviceToken(token.Token.ToString());
		Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/announcements");
	}

	public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
		SendNotification(e.Message.Notification.Body);

		Debug.Log("Received a new message");
	}
	void SendNotification (string messageBody) {
#if UNITY_ANDROID
		AndroidNotification notification = new AndroidNotification() {
			Title = "Insight",
			Text = messageBody,
			SmallIcon = "default",
			LargeIcon = "default",
			FireTime = DateTime.Now,
		};

		AndroidNotificationCenter.SendNotification(notification, "default_channel");
#endif

#if UNITY_IOS
		var timeTrigger = new iOSNotificationTimeIntervalTrigger() {
			TimeInterval = new TimeSpan(0, 0, 0),
			Repeats = false
		};

		var notification = new iOSNotification() {
			Title = "Insight",
			Body = messageBody,
			ShowInForeground = true,
			ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
			Trigger = timeTrigger,
		};

		iOSNotificationCenter.ScheduleNotification(notification);
#endif
	}
}
