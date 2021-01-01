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

namespace CalendarQuickstart
{
    class GoogleEvent
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = {
            CalendarService.Scope.CalendarEvents               
                //CalendarService.Scope.Calendar                , 
            
                //,CalendarService.Scope.CalendarEventsReadonly
                //,CalendarService.Scope.CalendarReadonly
                //,CalendarService.Scope.CalendarSettingsReadonly
        };
        static string ApplicationName = "Google Calendar API .NET Quickstart";
        string calendarID = "2n7op2kiaukjv3labvr48i9g7c@group.calendar.google.com";

        private CalendarService service ;

        private Events events ;

        public GoogleEvent(){
                UserCredential credential;
            using (var stream =
                new FileStream("Google\\credentials-sb.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.

                //string credPath = "drsb-1606547415805-ed16202aae7d.json";
                string credPath = "Google\\token-pb.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                //ApiKey= "ed16202aae7d40843779e372738a5e1e290dd62c"
            });

            

        }


        //https://developers.google.com/calendar/quickstart/dotnet#prerequisites
        public Events EventList()
        {
            
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List(calendarID); //.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            events = request.Execute();
            // Console.WriteLine("Upcoming events:");
            // if (events.Items != null && events.Items.Count > 0)
            // {
            //     foreach (var eventItem in events.Items)
            //     {
            //         string when = eventItem.Start.DateTime.ToString();
            //         if (String.IsNullOrEmpty(when))
            //         {
            //             when = eventItem.Start.Date;
            //         }
            //         Console.WriteLine("{0} ({1})", eventItem.Summary, when);
            //     }
            // }
            // else
            // {
            //     Console.WriteLine("No upcoming events found.");
            // }

            return events;

        }

         public void CreateEvent()
        {
            Event eventNew = new Event();
            eventNew.Status = "confirmed";
            eventNew.Summary = "Google I/O 2016"; //*
            eventNew.Location = "800 Howard St., San Francisco, CA 94103"; //*

            eventNew.Description = "A chance to hear more about Google's developer products.";

            EventDateTime evenStart = new EventDateTime();
            evenStart.DateTime = DateTime.Now; //Convert.ToDateTime("20-12-2020 22:23:04"); //*
            evenStart.TimeZone = "Asia/Calcutta";

            eventNew.Start = evenStart;

            EventDateTime evenEnd = new EventDateTime();
            evenEnd.DateTime = DateTime.Now.AddMinutes(30);
            evenEnd.TimeZone = "Asia/Calcutta"; //"America/Los_Angeles"; // "India Standard Time";

            eventNew.End = evenEnd;

            //            String[] recurrence = new String[] { "RRULE:FREQ=DAILY;COUNT=2" };
            //event.setRecurrence(Arrays.asList(recurrence));

            EventAttendee[] attendeesAdd = new EventAttendee[1];
            //{
            //    new EventAttendee() => x.Email = "bobhate.pradip@gmail.com");//,
            //    //new EventAttendee().Email ="bobhate.pradip@gmail.com",
            //};

            var eventAttendee = new EventAttendee();
            eventAttendee.Email = "bobhate.pradip@gmail.com";

            attendeesAdd[0] = eventAttendee;

            //eventAttendee.Email = "dr.shweta.bobhate@gmail.com";
            //attendees[1] = eventAttendee;


            eventNew.Attendees = attendeesAdd;


            EventReminder[] reminderOverrides = new EventReminder[2];
            //            {
            //    new EventReminder().setMethod("email").setMinutes(24 * 60),
            //    new EventReminder().setMethod("popup").setMinutes(10),
            //};
            var eventReminder = new EventReminder();
            eventReminder.Method = "email";
            eventReminder.Minutes = (24 * 60);
            reminderOverrides[0] = eventReminder;

            eventReminder.Method = "popup";
            eventReminder.Minutes = (10);
            reminderOverrides[1] = eventReminder;

            Event.RemindersData reminders = new Event.RemindersData();
            reminders.UseDefault = false;
            reminders.Overrides = reminderOverrides.ToList();
            eventNew.Reminders = reminders;

            Console.WriteLine("$$$$ ----- END: Event read ---------$$$");
            //Console.Read();
            //events.Items.Add(eventNew);

            //service.Events.Insert(eventNew, calendarID).Execute();

            if (!(events.Items.Select(x=> x.Summary).ToList().Contains(eventNew.Summary)))
            {
                Event recurringEvent = service.Events.Insert(eventNew, calendarID).Execute();

                Console.WriteLine("$$$$ ----- END: Event Added ---------$$$");
            }else{
                 Console.WriteLine("$$$$ ----- END: Event Already Exists!!!! ---------$$$");
            }

        }
    }
}