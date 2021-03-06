using System.Collections.Generic;
using MvcMusicStore.Models;

namespace MvcMusicStore.ViewModels
{
    public class StoreManagerViewModel
    {
        public Album Album { get; set; }
        public List<Album.AlbumArtist> Artists { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
