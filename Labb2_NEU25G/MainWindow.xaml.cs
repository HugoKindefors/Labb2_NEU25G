using Labb2_NEU25G.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadArtistsAsync();
            await LoadPlaylistsAsync();
        }

        private async Task LoadArtistsAsync()
        {
            using var db = new MusicContext();

            var artists = await db.Artists
                .Where(artist => artist.Albums.Count > 2)
                .Include(artist => artist.Albums)
                .ThenInclude(album => album.Tracks)  
                .ToListAsync();

            myTreeView.ItemsSource = new ObservableCollection<Artist>(artists);
        }

        private async Task LoadPlaylistsAsync()
        {
            using var db = new MusicContext();
            var playlists = await db.Playlists.ToListAsync();
            cmbPlaylists.ItemsSource = new ObservableCollection<Playlist>(playlists);
        }

        private async Task LoadTracksAsync(Album album)
        {
            using var db = new MusicContext();

            var tracks = await db.Tracks
                .Include(t => t.Album)
                .Where(t => t.AlbumId == album.AlbumId)
                .Select(t => new TrackViewModel
                { 
                    Name = t.Name, 
                    Composer = t.Composer ?? "Unknown", 
                    Length = TimeSpan.FromMilliseconds(t.Milliseconds).ToString(@"mm\:ss") 
                })
                .ToListAsync();

            myDataGrid.ItemsSource = new ObservableCollection<TrackViewModel>(tracks);
        }

        private async Task LoadPlaylistTracksAsync(int playlistId)
        {
            using var db = new MusicContext();

            var tracks = await db.PlaylistTracks
                .Where(pt => pt.PlaylistId == playlistId)
                .Include(pt => pt.Track)
                .Select(pt => pt.Track)
                .ToListAsync();

            lstPlaylistTracks.ItemsSource = new ObservableCollection<Track>(tracks);
        }

        private async void myTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Album album)
            {
                await LoadTracksAsync(album);
            }
        }

        private async void cmbPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPlaylists.SelectedItem is Playlist playlist)
            {
                await LoadPlaylistTracksAsync(playlist.PlaylistId);
            }
        }

        private void AddArtist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Artist - Coming soon!");
        }

        private void EditArtist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit Artist - Coming soon!");
        }

        private void DeleteArtist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete Artist - Coming soon!");
        }

        private void AddAlbum_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Album - Coming soon!");
        }

        private void EditAlbum_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit Album - Coming soon!");
        }

        private void DeleteAlbum_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete Album - Coming soon!");
        }

        private void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Track - Coming soon!");
        }

        private void EditTrack_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Edit Track - Coming soon!");
        }

        private void DeleteTrack_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete Track - Coming soon!");
        }

        private void NewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("New Playlist - Coming soon!");
        }

        private void RenamePlaylist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rename Playlist - Coming soon!");
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete Playlist - Coming soon!");
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add to Playlist - Coming soon!");
        }

        private void RemoveFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Remove from Playlist - Coming soon!");
        }
    }
}