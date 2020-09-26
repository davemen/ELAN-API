using ELANAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace ELANAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayMusicController : ControllerBase
    {

        private readonly ILogger<PlayMusicController> _logger;

        public PlayMusicController(ILogger<PlayMusicController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> GetAsync(String IPAddress, String Service, string Artist, String Room)
        {
            PlayMusic mymusic = new PlayMusic();

            mymusic.Artist = Artist;
            mymusic.IPAddress = IPAddress;
            mymusic.Room = Room;
            mymusic.Service = Service;

            //clean up the data
            mymusic.Artist = char.ToUpper(mymusic.Artist[0]) + mymusic.Artist.Substring(1);


            if (mymusic.Service == "Pandora")
            {
                // await SendMessageToElan.InitializeElan(mymusic);
                // await SendMessageToElan.SendMessagePandora(mymusic);
                string results = await WebAPI.WebElanPandoraAsync(mymusic);
            }
            else
            {
                string x = WebAPI.WebElan(mymusic);
                //  await SendMessageToElan.SendMessageSpotify(mymusic);
            }

            //Create Telnet client connected to the Mirage Media Streamer 
            return "OK";
        }
    }
    [ApiController]
    [Route("[controller]")]
    public class StopController : ControllerBase
    {

      //  private readonly ILogger<StopMusicController> _logger;

        public StopController(ILogger<PlayMusicController> logger)
        {
         //   _logger = (ILogger<StopMusicController>)logger;
        }

        [HttpGet]
        public string Get(String IPAddress)
        {
            RestClient client = new RestClient();
            //send the request to the Rest Sharp IP address
            client = new RestClient("http://" + IPAddress + "/api/stop");

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Cookie", "clientId=5504ba32-149c-4bac-9ced-bc775684940d; clientId=5504ba32-149c-4bac-9ced-bc775684940d");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content); ;

            return "ok";

        }
    }
}
