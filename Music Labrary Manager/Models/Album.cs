using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_Labrary_Manager.Models
{
    public class Album
    {
        public string Title {get; set;}
        public DateTime ReleaseDate {get; set;}
        public string CoverImagePath {get; set;}

        public ObservableCollection<Song> Songs {get; set;} = new ObservableCollection<Song>();
        public int YearsSinceRelease => DateTime.Now.Year - ReleaseDate.Year;

        public override string ToString()
        {
            return $"{Title} ({ReleaseDate.Year})";
        }
    }
}
