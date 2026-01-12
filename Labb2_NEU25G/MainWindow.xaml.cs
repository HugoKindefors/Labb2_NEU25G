using Labb2_NEU25G.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Labb2_NEU25G.Models
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadArtists();
        }

        private void LoadArtists()
        {
            using var db = new MusicContext();

            var artists = db.Artists
                .Where(artist => artist.Albums.Count > 2)
                .Include(artist => artist.Albums)
                .ThenInclude(album => album.Tracks)  // ✅ Avkommenterad för att ladda tracks
                .ToList();

            myTreeView.ItemsSource = new ObservableCollection<Artist>(artists);
        }

        private void LoadTracks(Album album)
        {
            using var db = new MusicContext();

            var tracks = db.Tracks
                .Include(t => t.Album)
                .Where(t => t.AlbumId == album.AlbumId)
                .Select(t => new 
                { 
                    Name = t.Name, 
                    Composer = t.Composer, 
                    Length = TimeSpan.FromMilliseconds(t.Milliseconds).ToString(@"mm\:ss") 
                })
                .ToList();

            myDataGrid.ItemsSource = new ObservableCollection<object>(tracks);
        }

        private void myTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Album album)
            {
                LoadTracks(album);
            }
        }
    }
}