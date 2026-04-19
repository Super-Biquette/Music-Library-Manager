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
        private string selectedGenre { get; set; } = "All";
        public string SelectedGenre
        {
            get => selectedGenre;
            set
            {
                selectedGenre = value;
                ApplyFilters();
            }
        }
        public Artist SelectedArtist { get; set; }
        public Album SelectedAlbum { get; set; }
        public ICommand SortAZCommand { get; set; }
        public ICommand SortDateCommand { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand ViewDetailsCommand { get; set; }
        public ICommand SavePlaylistCommand { get; set; }
        public ICommand LoadPlaylistCommand { get; set; }
        public ICommand StopCommand { get; }
        public ICommand VolumeCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        private MusicPlayer player = new MusicPlayer();
        private string searchText;
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                ApplyFilters();
            }
        }
        public Song SelectedSong { get; set; }

        private DeezerAPI deezer = new DeezerAPI();
        private RapidAPI rapid = new RapidAPI();
        public ICommand LoadAPICommand { get; }
        public ICommand GetSongInfoCommand { get; }
        public string ApiSearchText { get; set; } = "rock";
        public ObservableCollection<Song> Playlist { get; set; } = new ObservableCollection<Song>();
        public ICommand AddToPlaylistCommand { get; }
        public ICommand RemoveFromPlaylistCommand { get; }
        public ObservableCollection<Song> APISongs { get; set; } = new ObservableCollection<Song>();
        private readonly string playlistFile = "playlist.json";

        public MainViewModel()
        {
            SeedData();
            ApplyFilters();
            SortAZCommand = new Command(_ => SortAZ());
            SortDateCommand = new Command(_ => SortByDate());
            PlayCommand = new Command(_ => Play());
            ViewDetailsCommand = new Command(_ => OpenDetails());
            SavePlaylistCommand = new Command(_ => SavePlaylist());
            LoadPlaylistCommand = new Command(_ => LoadPlaylist());
            StopCommand = new Command(_ => player.Stop());
            VolumeCommand = new Command(v => player.SetVolume((double)v));
            NextCommand = new Command(_ => Next());
            PreviousCommand = new Command(_ => Previous());
            LoadAPICommand = new Command(async _ => await LoadFromAPI());
            GetSongInfoCommand = new Command(async _ => await rapid.GetSongInfo());
            AddToPlaylistCommand = new Command(_ => AddToPlaylist());
            RemoveFromPlaylistCommand = new Command(_ => RemoveFromPlaylist());
        }

        private double volume = 0.5;
        public double Volume
        {
            get => volume;
            set
            {
                volume = value;
                player.SetVolume(volume);
            }
        }

        private void AddToPlaylist()
        {
            if (SelectedSong != null && !Playlist.Contains(SelectedSong))
            {
                Playlist.Add(SelectedSong);
            }
        }

        private void RemoveFromPlaylist()
        {
            if (SelectedSong != null)
            {
                Playlist.Remove(SelectedSong);
            }
        }

        private async Task LoadFromAPI()
        {
            try
            {
                var songs = await deezer.SearchSongsAsync(ApiSearchText);

                APISongs.Clear();

                foreach (var song in songs)
                {
                    APISongs.Add(song);
                }

                if (APISongs.Count == 0)
                {
                    MessageBox.Show("No results found.");
                }
            }
            catch
            {
                MessageBox.Show("Error loading data from API.");
            }
        }

        private void Next()
        {
            if (SelectedAlbum == null || SelectedSong == null) return;

            var songs = SelectedAlbum.Songs.ToList();
            int index = songs.IndexOf(SelectedSong);

            if (index < songs.Count - 1)
            {
                SelectedSong = songs[index + 1];
                player.Play(SelectedSong.FilePath);
            }
        }

        private void Previous()
        {
            if (SelectedAlbum == null || SelectedSong == null) return;

            var songs = SelectedAlbum.Songs.ToList();
            int index = songs.IndexOf(SelectedSong);

            if (index > 0)
            {
                SelectedSong = songs[index - 1];
                player.Play(SelectedSong.FilePath);
            }
        }

        private void SavePlaylist()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(Playlist, options);
                File.WriteAllText(playlistFile, json);

                MessageBox.Show("Playlist saved!");
            }
            catch
            {
                MessageBox.Show("Error saving playlist");
            }
        }

        private void LoadPlaylist()
        {
            try
            {
                if (!File.Exists(playlistFile)) return;

                string json = File.ReadAllText(playlistFile);

                var list = JsonSerializer.Deserialize<List<Song>>(json);

                Playlist.Clear();

                foreach (var song in list)
                {
                    Playlist.Add(song);
                }
            }
            catch
            {
                MessageBox.Show("Error loading playlist");
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
            if (SelectedSong != null)
            { 
                player.Play(SelectedSong.FilePath);
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
            {
                filtered = filtered.Where(a => a.Genre == SelectedGenre);
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(a => a.Name.ToLower().Contains(searchText.ToLower()));
            }

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

            discovery.Songs.Add(new Song{ Title = "One More Time",Duration = TimeSpan.FromMinutes(5), FilePath = "Songs/onemoretime.mp3" });
            discovery.Songs.Add(new Song{ Title = "Harder Better Faster Stronger",Duration = TimeSpan.FromMinutes(4), FilePath = "Songs/harderbetterfasterstronger.mp3" });
            discovery.Songs.Add(new Song{ Title = "Digital Love",Duration = TimeSpan.FromMinutes(4), FilePath = "Songs/digitallove.mp3" });
            discovery.Songs.Add(new Song{ Title = "Get Lucky",Duration = TimeSpan.FromMinutes(4), FilePath = "Songs/getlucky.mp3" });

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

            album21.Songs.Add(new Song{Title = "Rolling in the Deep",Duration = TimeSpan.FromMinutes(3), FilePath = "Songs/rollinginthedeep.mp3"});
            album21.Songs.Add(new Song{Title = "Someone Like You",Duration = TimeSpan.FromMinutes(4), FilePath = "Songs/someonelikeyou.mp3"});
            album21.Songs.Add(new Song{Title = "Set Fire to the Rain",Duration = TimeSpan.FromMinutes(4), FilePath = "Songs/setfiretotherain.mp3"});
            album21.Songs.Add(new Song{Title = "Skyfall",Duration = TimeSpan.FromMinutes(3), FilePath = "Songs/skyfall.mp3"});
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

            parachutes.Songs.Add(new Song{Title = "Viva La Vida",Duration = TimeSpan.FromMinutes(5), FilePath = "Songs/vivalavida.mp3"});
            parachutes.Songs.Add(new Song{Title = "Paradise",Duration = TimeSpan.FromMinutes(4), FilePath = "Songs/paradise.mp3"});
            parachutes.Songs.Add(new Song{Title = "A Sky Full Of Stars",Duration = TimeSpan.FromMinutes(5), FilePath = "Songs/askyfullofstars.mp3"});
            
            coldplay.Albums.Add(parachutes);
            Artists.Add(coldplay);

            var imagineDragons = new BandArtist
            {
                Name = "Imagine Dragons",
                Genre = "Pop",
                DebutDate = new DateTime(2008, 1, 1),
                MemberCount = 4
            };

            var evolve = new Album
            {
                Title = "Evolve",
                ReleaseDate = new DateTime(2017, 6, 23)
            };

            evolve.Songs.Add(new Song{Title = "Believer",Duration = TimeSpan.FromMinutes(3), FilePath = "Songs/believer.mp3"});
            evolve.Songs.Add(new Song{Title = "Thunder",Duration = TimeSpan.FromMinutes(4), FilePath = "Songs/thunder.mp3"});
            evolve.Songs.Add(new Song{Title = "Radioactive",Duration = TimeSpan.FromMinutes(3), FilePath = "Songs/radioactive.mp3"});

            imagineDragons.Albums.Add(evolve);
            Artists.Add(imagineDragons);

            ApplyFilters();
        }

    }
}
