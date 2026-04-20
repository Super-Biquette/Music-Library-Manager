using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_Labrary_Manager.Models
{
    public abstract class Artist
    {
        public string Name {get; set;}
        public string Genre {get; set;}
        public DateTime DebutDate {get; set;}
        public ObservableCollection<Album> Albums {get; set;} = new ObservableCollection<Album>();

        public override string ToString()
        {
            return Name;
        }
    }

    public class SoloArtist : Artist
    {
        public string RealName { get; set; }
    }

    public class BandArtist : Artist
    {
        public int MemberCount { get; set; }
    }
}
