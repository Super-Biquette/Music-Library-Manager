using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using Music_Labrary_Manager.Models;
using System.Windows;

namespace Music_Labrary_Manager.Services
{
    public class RapidAPI
    {
        private readonly HttpClient client = new HttpClient();

        public async Task GetSongInfo()
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://songstats.p.rapidapi.com/tracks/info?spotify_track_id=3VTPi12rK7mioSLL0mmu75"),
                };

                request.Headers.Add("x-rapidapi-host", "songstats.p.rapidapi.com");
                request.Headers.Add("x-rapidapi-key", "TA_CLE_API_ICI");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();

                MessageBox.Show(body);
            }
            catch
            {
                MessageBox.Show("API error");
            }
        }
    }
}
