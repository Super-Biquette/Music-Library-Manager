using Music_Labrary_Manager;
using Music_Labrary_Manager.Models;
using Music_Labrary_Manager.Services;
using MusicLibraryManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;   
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace Music_Labrary_Manager.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<Artist> Artists { get; set; } = new ObservableCollection<Artist>();
        public ObservableCollection<Artist> FilteredArtists { get; set; } = new ObservableCollection<Artist>();
        public ObservableCollection<string> Genres { get; set; } =
            new ObservableCollection<string> { "All", "Pop", "Rock", "Electronic" };
        public string SelectedGenre { get; set; } = "All";
        public Artist SelectedArtist { get; set; }
        public Album SelectedAlbum { get; set; }
        public ICommand SortAZCommand { get; set; }
        public ICommand SortDateCommand { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand ViewDetailsCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        private MusicPlayer player = new MusicPlayer();

        public MainViewModel()
        {
            SeedData();
            ApplyFilters();
            SortAZCommand = new Command(_ => SortAZ());
            SortDateCommand = new Command(_ => SortByDate());
            PlayCommand = new Command(_ => Play());
            ViewDetailsCommand = new Command(_ => OpenDetails());
            SaveCommand = new Command(_ => Save());
        }

        private void Save()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(Artists, options);
                File.WriteAllText("Music.json", json);

                MessageBox.Show("Saved successfully!");
            }
            catch
            {
                MessageBox.Show("Error saving file");
            }
        }
        private void OpenDetails()
        {
            if (SelectedArtist != null)
            {
                var window = new ArtistDetailsWindow(SelectedArtist);
                window.Show();
            }
        }
        private void Play()
        {
            if (SelectedAlbum != null && SelectedAlbum.Songs.Any())
            {
                var song = SelectedAlbum.Songs.First();
                player.Play(song.FilePath);
            }
        }
        private void SortAZ()
        {
            var sorted = FilteredArtists.OrderBy(a => a.Name).ToList();
            FilteredArtists.Clear();
            foreach (var a in sorted)
            {
                FilteredArtists.Add(a);
            }
        }

        private void SortByDate()
        {
            var sorted = FilteredArtists.OrderBy(a => a.DebutDate).ToList();
            FilteredArtists.Clear();
            foreach (var a in sorted)
            {
                FilteredArtists.Add(a);
            }
        }
        public void ApplyFilters(string searchText = "")
        {
            var filtered = Artists.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedGenre) && SelectedGenre != "All")
                filtered = filtered.Where(a => a.Genre == SelectedGenre);

            if (!string.IsNullOrEmpty(searchText))
                filtered = filtered.Where(a => a.Name.ToLower().Contains(searchText.ToLower()));

            FilteredArtists.Clear();
            foreach (var artist in filtered)
            {
                FilteredArtists.Add(artist);
            }
        }

        private void SeedData()
        {
            var daftPunk = new BandArtist
            {
                Name = "Daft Punk",
                Genre = "Electronic",
                DebutDate = new DateTime(1993, 1, 1),
                MemberCount = 2
            };

            var discovery = new Album
            {
                Title = "Discovery",
                ReleaseDate = new DateTime(2001, 3, 13)
            };

            discovery.Songs.Add(new Song
            {
                Title = "One More Time",
                Duration = TimeSpan.FromMinutes(5)
            });

            discovery.Songs.Add(new Song
            {
                Title = "Harder Better Faster Stronger",
                Duration = TimeSpan.FromMinutes(4)
            });

            daftPunk.Albums.Add(discovery);
            Artists.Add(daftPunk);

            var adele = new SoloArtist
            {
                Name = "Adele",
                Genre = "Pop",
                DebutDate = new DateTime(2006, 1, 1),
                RealName = "Adele Laurie Blue Adkins"
            };

            var album21 = new Album
            {
                Title = "21",
                ReleaseDate = new DateTime(2011, 1, 24)
            };

            album21.Songs.Add(new Song
            {
                Title = "Rolling in the Deep",
                Duration = TimeSpan.FromMinutes(3)
            });

            adele.Albums.Add(album21);
            Artists.Add(adele);

            var coldplay = new BandArtist
            {
                Name = "Coldplay",
                Genre = "Rock",
                DebutDate = new DateTime(1996, 1, 1),
                MemberCount = 4
            };

            var parachutes = new Album
            {
                Title = "Parachutes",
                ReleaseDate = new DateTime(2000, 7, 10)
            };

            parachutes.Songs.Add(new Song
            {
                Title = "Yellow",
                Duration = TimeSpan.FromMinutes(4)
            });

            coldplay.Albums.Add(parachutes);
            Artists.Add(coldplay);

            var imagineDragons = new BandArtist
            {
                Name = "Imagine Dragons",
                Genre = "Rock",
                DebutDate = new DateTime(2008, 1, 1),
                MemberCount = 4
            };

            var evolve = new Album
            {
                Title = "Evolve",
                ReleaseDate = new DateTime(2017, 6, 23)
            };

            evolve.Songs.Add(new Song
            {
                Title = "Believer",
                Duration = TimeSpan.FromMinutes(3)
            });

            imagineDragons.Albums.Add(evolve);
            Artists.Add(imagineDragons);

            ApplyFilters();
        }
    }
}
