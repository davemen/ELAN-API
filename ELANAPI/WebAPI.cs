using ELANAPI.Models;
using RestSharp;
using RestSharp.Extensions;
using System;

namespace ELANAPI
{
    public class WebAPI
    {
        //static RestClient client;
        static string theIPAddress;
        public static string WebElan(PlayMusic mymusic)
        {

            //initialize the web client
            theIPAddress = mymusic.IPAddress;

            //send the request to the Rest Sharp IP address
            //   client = new RestClient("http://" + theIPAddress + "/api");

            //set the room to play the music in
            string theCommand = @"setzone """ + mymusic.Room + @"""";
            ProcessWebPost(theCommand);

            // power the speaker on
            ProcessWebPost("Power on");

            //set the source to main
            theCommand = @"setsource ""Main""";
            ProcessWebPost(theCommand);

            //Set the instance to the Main player
            ProcessWebPost("setInstance/Main");

            //clear the queue
            ProcessWebPost("ClearNowPlaying");

            //clear the queue
            ProcessWebPost("Stop");

            //send Search to Spotify
            Rootobject results = ProcessWebGet("searchforservice/spotify/'" + mymusic.Artist + "'");

            string ArtistGuid = results.browse.Items[0].Guid;

            //Pick Artists 
            theCommand = "AckPickItem/" + ArtistGuid;
            results = ProcessWebGet(theCommand);

            //Pick first Artist
            ArtistGuid = results.browse.Items[0].Guid;
            theCommand = "AckPickItem/" + ArtistGuid;
            results = ProcessWebGet(theCommand);

            //Pick Top Tracks
            string TopTracks = results.browse.Items[1].Guid;
            theCommand = "AckPickItem/" + TopTracks;
            results = ProcessWebGet(theCommand);

            //Pick PlayAll
            string PlayAll = results.browse.Items[0].Guid;
            theCommand = "AckPickItem/" + PlayAll;
            results = ProcessWebGet(theCommand);

            //PickPlayNow
            string PlayNow = results.browse.Items[0].Guid;
            theCommand = "AckPickItem/" + PlayNow;
            results = ProcessWebGet(theCommand);

            //Shuffle
            ProcessWebPost("Shuffle/true");

            //Play
            ProcessWebPost("Play");

            //Skip
            ProcessWebPost("SkipNext");

            return "OK";
        }

        public static async System.Threading.Tasks.Task<string> WebElanPandoraAsync(PlayMusic mymusic)
        {

            await SendMessageToElan.InitializeElan(mymusic);

            //initialize the web client
            theIPAddress = mymusic.IPAddress;

            //send the request to the Rest Sharp IP address
           // client = new RestClient("http://" + theIPAddress + "/api");

            //set the room to play the music in
            string theCommand = @"setzone """ + mymusic.Room + @"""";
            ProcessWebPost(theCommand);

            // power the speaker on
            ProcessWebPost("Power on");

            //set the source to main
            theCommand = @"setsource ""Main""";
            ProcessWebPost(theCommand);

            //Set the instance to the Main player
            ProcessWebPost("setInstance/Main");

            //clear the queue
            ProcessWebPost("ClearNowPlaying");

            //clear the queue
            ProcessWebPost("Stop");

            //send Station to Pandora
            Rootobject results = ProcessWebGet("PlayRadioStation/" + mymusic.Artist + " Radio");

            //Add switch to spotify if no station
            if (results.events.Length < 3)
            {
                WebElan(mymusic);
            }

            return "OK";
        }

        private static string ProcessWebPost(string theMessage)
        {
            //This is really a faux Post - it's a get that doesn't do a second call
            // to get the results

            //this one calls the media streamer and passes in the message
            theMessage = theMessage.UrlEncode();
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("http://" + theIPAddress + "/api" + "/" + theMessage);
            var request = new RestRequest();

            IRestResponse myresponse = client.Execute(request);

            return "OK";
        }

        private static Rootobject ProcessWebGet(string theMessage)
        {
            ////this one calls the media streamer and passes in the message
            string clientString = "http://" + theIPAddress + "/api/" + theMessage;
            var client = new RestClient(clientString);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cookie", "clientId=5504ba32-149c-4bac-9ced-bc775684940d; clientId=5504ba32-149c-4bac-9ced-bc775684940d");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content); ;

            //Get the results - you have to send a blank request in to get the results from the API Request

            RestClient client2 = new RestClient("http://" + theIPAddress + "/api/");
            client.Timeout = -1;
            RestRequest request2 = new RestRequest(Method.GET);
            request2.AddHeader("Cookie", "clientId=5504ba32-149c-4bac-9ced-bc775684940d");
            IRestResponse response2 = client2.Execute(request2);

            // deserialize the results and return them.
            Rootobject myresults = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(response2.Content);
            return myresults;
        }
    }
}

