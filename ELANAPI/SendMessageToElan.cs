using ELANAPI.Models;
using PrimS.Telnet;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ELANAPI
{

    // server IP Address: 73.140.54.240
    public class SendMessageToElan
    {

        public static async Task InitializeElan(PlayMusic myMusic)
        {
            using (Client client2 = new Client(myMusic.IPAddress, 5006, new System.Threading.CancellationToken()))
            {

                //set the room - Send the command via Telnet to the streamer
                string theCommand = @"setzone """ + myMusic.Room + @"""";
                await client2.WriteLine(theCommand);

                // power the speaker on
                theCommand = "Power on";
                await client2.WriteLine(theCommand);

                //set the source to main
                theCommand = @"setsource ""Main""";
                await client2.WriteLine(theCommand);
            }
        }

        public static async Task SendMessagePandora(PlayMusic myMusic)
        {

            //TODO: Replace the hardcoded IP Address
            //Create telnet client to send command to control Mirage Media Streamer
            using (Client client = new Client(myMusic.IPAddress, 5004, new System.Threading.CancellationToken()))
            {

                string theCommand = "Ping";
                await client.WriteLine(theCommand);
                string u = await client.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(2000));
                Debug.WriteLine(u);

                theCommand = "setInstance Main";
                await client.WriteLine(theCommand);

                theCommand = "ClearRadioFilter";
                await client.WriteLine(theCommand);
                u = await client.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(2000));
                Debug.WriteLine(u);

                theCommand = "ClearNowPlaying";
                await client.WriteLine(theCommand);

                //Play Pandora radio station
                 theCommand = @"PlayRadioStation """ + myMusic.Artist + @" Radio""";
                await client.WriteLine(theCommand);
                u = await client.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(800));
                Debug.WriteLine(u);
                int errorPos = u.IndexOf("Station not found");
                if (errorPos > 0)
                {
                    //No radio station, so play it on spotify.
                    await SendMessageSpotify(myMusic);
                }

                theCommand = "Play";
                await client.WriteLine(theCommand);

            }
        }

        public static async Task SendMessageSpotify(PlayMusic myMusic)
        {

            using (Client client2 = new Client(myMusic.IPAddress, 5004, new System.Threading.CancellationToken()))
            {
                string theCommand = "Ping";
                await client2.WriteLine(theCommand);
                string u = await client2.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(2000));
                Debug.WriteLine(u);

                theCommand = "setInstance Main";
                await client2.WriteLine(theCommand);

                theCommand = "ClearRadioFilter";
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(2000));
                Debug.WriteLine(u);

                theCommand = "ClearNowPlaying";
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(5000));
                Debug.WriteLine(u);

                //set picklist length to 5 items
                theCommand = "SetPickListCount 5";
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(5000));
                Debug.WriteLine(u);

                // Switch to Spotify
                theCommand = @"SetRadioFilter Source=""Spotify""";
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("\r\n", TimeSpan.FromMilliseconds(5000));
                Debug.WriteLine(u);

                //Top Menu for Spotify 
                theCommand = "BrowseRadioGenres 1 32";
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("EndPickList More", TimeSpan.FromMilliseconds(5000));
                Debug.WriteLine(u);

                //Top Menu for Spotify 
                theCommand = "BrowseRadioGenres 1 32";
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("EndPickList More", TimeSpan.FromMilliseconds(5000));
                Debug.WriteLine(u);

                //Pick Search 
                theCommand = "AckPickItem 42843413-6acc-0327-2d2b-4aa28b75a5d3";
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("AckPickItem Ok\r\n", TimeSpan.FromMilliseconds(10000));
                Debug.WriteLine(u);
                int index1 = u.IndexOf("AckButton ") + 10;
                int length = 36;
                string theButton = u.Substring(index1, length);
                Debug.WriteLine("theButton:" + theButton);

                //Enter Search Box and click the button 
                theCommand = "AckButton " + theButton + " OK " + @"""" + myMusic.Artist + @"""";
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("EndPickList NoMore", TimeSpan.FromMilliseconds(10000));
                Debug.WriteLine(u);
                index1 = u.IndexOf("PickListItem") + 14;
                length = 36;
                string guidForArtist = u.Substring(index1, length);

                //Select the Artist 
                theCommand = "AckPickItem " + guidForArtist;
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("EndPickList More", TimeSpan.FromMilliseconds(10000));
                Debug.WriteLine(u);
                index1 = u.IndexOf("PickListItem") + 14;
                length = 36;
                string guidForfirstArtist = u.Substring(index1, length);

                //Select the first artist in the list
                theCommand = "AckPickItem " + guidForfirstArtist;
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("EndPickList NoMore", TimeSpan.FromMilliseconds(10000));
                Debug.WriteLine(u);
                //get the second pick list item - Top Tracks
                index1 = u.IndexOf("PickListItem") + 15;
                int index2 = u.IndexOf("PickListItem", index1) + 14;
                length = 36;
                string guidForTopTracks = u.Substring(index2, length);

                //Select Top Tracks in the list
                theCommand = "AckPickItem " + guidForTopTracks;
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("EndPickList More", TimeSpan.FromMilliseconds(10000));
                Debug.WriteLine(u);
                //get "Replace Queue" from the picklist
                index1 = u.IndexOf("Play next") + 14;
                index2 = u.IndexOf("PickListItem", index1) + 14;
                length = 36;
                string guidForReplaceQueue = u.Substring(index2, length);

                //Select PlayAll
                theCommand = "AckPickItem " + guidForReplaceQueue;
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("EndPickList NoMore", TimeSpan.FromMilliseconds(10000));
                Debug.WriteLine(u);
                index1 = u.IndexOf("PickListItem") + 14;
                length = 36;
                string guidForPlayAll = u.Substring(index1, length);

                //Call PlayAll
                theCommand = "AckPickItem " + guidForPlayAll;
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("EndPickList More", TimeSpan.FromMilliseconds(10000));
                Debug.WriteLine(u);

                //set shuffle mode on
                theCommand = "Shuffle true";
                await client2.WriteLine(theCommand);
                u = await client2.TerminatedReadAsync("###", TimeSpan.FromMilliseconds(1000));
                Debug.WriteLine(u);

                theCommand = "SkipNext";
                await client2.WriteLine(theCommand);

            }
        }
    }
}

