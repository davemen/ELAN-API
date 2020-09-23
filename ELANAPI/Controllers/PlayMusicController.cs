using ELANAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

            if (mymusic.Service == "Pandora")
            {
                // await SendMessageToElan.InitializeElan(mymusic);
                // await SendMessageToElan.SendMessagePandora(mymusic);
                 string results = WebAPI.WebElanPandora(mymusic);
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
}
