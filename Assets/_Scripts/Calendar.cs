using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CalendarQuickstart {
	class Calendar : MonoBehaviour {
		// If modifying these scopes, delete your previously saved credentials
		// at ~/.credentials/calendar-dotnet-quickstart.json
		static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
		static string ApplicationName = "Google Calendar API .NET Quickstart";

		private void Start() {
			/*
			UserCredential credential;

			using (var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read)) {
				// The file token.json stores the user's access and refresh tokens, and is created
				// automatically when the authorization flow completes for the first time.
				string credPath = "token.json";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
				Debug.Log("Credential file saved to: " + credPath);
			}*/
			

			// Create Google Calendar API service.
			var service = new CalendarService(new BaseClientService.Initializer() {
				ApiKey = "AIzaSyCAkCz0rjx4gw1omV7CuWR8wq_0dhKg2ww",
				ApplicationName = ApplicationName,
			});

			// Define parameters of request.
			EventsResource.ListRequest request = service.Events.List("3a0qhm7bbojktseqv6779snb8k@group.calendar.google.com");
			request.TimeMin = WeekAtGlance.GetStart();
			request.ShowDeleted = false;
			request.SingleEvents = true;
			request.MaxResults = 10;
			request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

			// List events.
			Events events = request.Execute();
			Debug.Log("Upcoming events:");
			if (events.Items != null && events.Items.Count > 0) {
				foreach (var eventItem in events.Items) {
					string when = eventItem.Start.DateTime.ToString();
					if (String.IsNullOrEmpty(when)) {
						when = eventItem.Start.Date;
					}

					Debug.Log(string.Format("{0} ({1})", eventItem.Summary, when));
				}
			} else {
				Debug.Log("No upcoming events found.");
			}
	
		}
		

		
	}
}