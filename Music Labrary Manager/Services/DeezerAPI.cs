using Music_Labrary_Manager.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Music_Labrary_Manager.Services
{
    public class DeezerAPI
    {
        private readonly HttpClient client = new HttpClient();
        //searches song from Deezer API using query
        public async Task<List<Song>> SearchSongsAsync(string query)
        {
            var songs = new List<Song>();

            try
            {
                string url = $"https://api.deezer.com/search?q={query}";
                var response = await client.GetStringAsync(url);

                var json = JObject.Parse(response);

                foreach (var item in json["data"])
                {
                    songs.Add(new Song
                    {
                        Title = item["title"].ToString(),
                        Duration = TimeSpan.FromSeconds((int)item["duration"]),
                        FilePath = ""
                    });
                }
            }
            catch { }

            return songs;
        }
    }
}
