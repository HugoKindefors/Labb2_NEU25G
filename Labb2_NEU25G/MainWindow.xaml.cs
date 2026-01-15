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
             //.Where(artist => artist.Albums.Count > 2)  // TOG BORT?
             .Include(artist => artist.Albums)
             .ThenInclude(album => album.Tracks)
             .OrderBy(a => a.Name)
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
                    TrackId = t.TrackId,
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

        private async void AddArtist_Click(object sender, RoutedEventArgs e)
        {
            var name = AskText("Add Artist", "Artistnamn:");
            if (string.IsNullOrWhiteSpace(name)) return;

            using var db = new MusicContext();

            var ids = await db.Artists.Select(a => a.ArtistId).ToListAsync();
            var newId = NextId(ids);

            var artist = new Artist
            {
                ArtistId = newId,
                Name = name
            };

            db.Artists.Add(artist);
            await db.SaveChangesAsync();

            await LoadArtistsAsync();
        }


        private async void EditArtist_Click(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is not Artist selectedArtist)
            {
                MessageBox.Show("Select an artist in the list first.");
                return;
            }

            var newName = AskText("Edit Artist", "New artist name:", selectedArtist.Name ?? "");
            if (string.IsNullOrWhiteSpace(newName)) return;

            using var db = new MusicContext();

            var artist = await db.Artists.FirstAsync(a => a.ArtistId == selectedArtist.ArtistId);
            artist.Name = newName;

            await db.SaveChangesAsync();
            await LoadArtistsAsync();
        }


        private async void DeleteArtist_Click(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is not Artist selectedArtist)
            {
                MessageBox.Show("Select an artist in the list first.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Remove artist '{selectedArtist.Name}'?\n\nNote: This artist has albums and cannot be deleted until they are removed.",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            using var db = new MusicContext();

            var hasAlbums = await db.Albums.AnyAsync(a => a.ArtistId == selectedArtist.ArtistId);
            if (hasAlbums)
            {
                MessageBox.Show("Can't delete an artist that has an almbum. Delete the album first.");
                return;
            }

            var artist = await db.Artists.FirstAsync(a => a.ArtistId == selectedArtist.ArtistId);
            db.Artists.Remove(artist);

            await db.SaveChangesAsync();
            await LoadArtistsAsync();
        }


        private async void AddAlbum_Click(object sender, RoutedEventArgs e)
        {
            Artist? artist = myTreeView.SelectedItem as Artist;

            if (artist == null && myTreeView.SelectedItem is Album albumNode)
                artist = albumNode.Artist;

            if (artist == null)
            {
                MessageBox.Show("Select an artist (or an album under an artist) first.");
                return;
            }

            var title = AskText("Add Album", "Album name:");
            if (string.IsNullOrWhiteSpace(title)) return;

            using var db = new MusicContext();

            var ids = await db.Albums.Select(a => a.AlbumId).ToListAsync();
            var newId = NextId(ids);

            var album = new Album
            {
                AlbumId = newId,
                Title = title,
                ArtistId = artist.ArtistId
            };

            db.Albums.Add(album);
            await db.SaveChangesAsync();

            await LoadArtistsAsync();
        }


        private async void EditAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is not Album selectedAlbum)
            {
                MessageBox.Show("Select an album in the list first.");
                return;
            }

            var newTitle = AskText("Edit Album", "New album name:", selectedAlbum.Title);
            if (string.IsNullOrWhiteSpace(newTitle)) return;

            using var db = new MusicContext();

            var album = await db.Albums.FirstAsync(a => a.AlbumId == selectedAlbum.AlbumId);
            album.Title = newTitle;

            await db.SaveChangesAsync();
            await LoadArtistsAsync();
        }


        private async void DeleteAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (myTreeView.SelectedItem is not Album selectedAlbum)
            {
                MessageBox.Show("Select an album in the list first.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Ta bort album '{selectedAlbum.Title}'?\n\nNote: If the album has songs they need to be deleted first.",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            using var db = new MusicContext();

            var hasTracks = await db.Tracks.AnyAsync(t => t.AlbumId == selectedAlbum.AlbumId);
            if (hasTracks)
            {
                MessageBox.Show("Can't remove albums that have songs. Remove the songs first.");
                return;
            }

            var album = await db.Albums.FirstAsync(a => a.AlbumId == selectedAlbum.AlbumId);
            db.Albums.Remove(album);

            await db.SaveChangesAsync();
            await LoadArtistsAsync();
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

        private async void NewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var name = AskText("New Playlist", "Playlist name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            using var db = new MusicContext();

            var ids = await db.Playlists.Select(p => p.PlaylistId).ToListAsync();
            var newId = NextId(ids);

            var playlist = new Playlist
            {
                PlaylistId = newId,
                Name = name
            };

            db.Playlists.Add(playlist);
            await db.SaveChangesAsync();

            await LoadPlaylistsAsync();

            cmbPlaylists.SelectedItem = cmbPlaylists.Items.Cast<Playlist>().FirstOrDefault(p => p.PlaylistId == newId);
        }

        private async void RenamePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPlaylists.SelectedItem is not Playlist selectedPlaylist)
            {
                MessageBox.Show("Select a playlist in the list first.", "No Playlist Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newName = AskText("Rename Playlist", "New playlist name:", selectedPlaylist.Name ?? "");
            if (string.IsNullOrWhiteSpace(newName)) return;

            using var db = new MusicContext();

            var playlist = await db.Playlists.FirstAsync(p => p.PlaylistId == selectedPlaylist.PlaylistId);
            playlist.Name = newName;

            await db.SaveChangesAsync();
            await LoadPlaylistsAsync();

            cmbPlaylists.SelectedItem = cmbPlaylists.Items.Cast<Playlist>().FirstOrDefault(p => p.PlaylistId == selectedPlaylist.PlaylistId);
        }

        private async void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPlaylists.SelectedItem is not Playlist selectedPlaylist)
            {
                MessageBox.Show("Select a playlist in the list first.", "No Playlist Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete playlist '{selectedPlaylist.Name}'?\n\nNote: All tracks in the playlist will be removed from it.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            using var db = new MusicContext();

            var playlistTracks = await db.PlaylistTracks
                .Where(pt => pt.PlaylistId == selectedPlaylist.PlaylistId)
                .ToListAsync();

            db.PlaylistTracks.RemoveRange(playlistTracks);

            var playlist = await db.Playlists.FirstAsync(p => p.PlaylistId == selectedPlaylist.PlaylistId);
            db.Playlists.Remove(playlist);

            await db.SaveChangesAsync();

            lstPlaylistTracks.ItemsSource = null;
            await LoadPlaylistsAsync();
        }

        private async void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPlaylists.SelectedItem is not Playlist selectedPlaylist)
            {
                MessageBox.Show("Select a playlist first.", "No Playlist Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (myDataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select one or more tracks to add to the playlist.", "No Tracks Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            using var db = new MusicContext();

            int addedCount = 0;
            int skippedCount = 0;

            foreach (TrackViewModel selectedTrack in myDataGrid.SelectedItems)
            {
                var exists = await db.PlaylistTracks
                    .AnyAsync(pt => pt.PlaylistId == selectedPlaylist.PlaylistId && pt.TrackId == selectedTrack.TrackId);

                if (exists)
                {
                    skippedCount++;
                    continue;
                }

                var playlistTrack = new PlaylistTrack
                {
                    PlaylistId = selectedPlaylist.PlaylistId,
                    TrackId = selectedTrack.TrackId
                };

                db.PlaylistTracks.Add(playlistTrack);
                addedCount++;
            }

            await db.SaveChangesAsync();

            await LoadPlaylistTracksAsync(selectedPlaylist.PlaylistId);

            string message = $"{addedCount} track(s) added to playlist.";
            if (skippedCount > 0)
            {
                message += $"\n{skippedCount} track(s) were already in the playlist.";
            }
            MessageBox.Show(message, "Add to Playlist", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void RemoveFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPlaylists.SelectedItem is not Playlist selectedPlaylist)
            {
                MessageBox.Show("Select a playlist first.", "No Playlist Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (lstPlaylistTracks.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select one or more tracks to remove from the playlist.", "No Tracks Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Remove {lstPlaylistTracks.SelectedItems.Count} track(s) from playlist '{selectedPlaylist.Name}'?",
                "Confirm Remove",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            using var db = new MusicContext();

            int removedCount = 0;

            foreach (Track selectedTrack in lstPlaylistTracks.SelectedItems)
            {
                var playlistTrack = await db.PlaylistTracks
                    .FirstOrDefaultAsync(pt => pt.PlaylistId == selectedPlaylist.PlaylistId && pt.TrackId == selectedTrack.TrackId);

                if (playlistTrack != null)
                {
                    db.PlaylistTracks.Remove(playlistTrack);
                    removedCount++;
                }
            }

            await db.SaveChangesAsync();

            await LoadPlaylistTracksAsync(selectedPlaylist.PlaylistId);

            MessageBox.Show($"{removedCount} track(s) removed from playlist.", "Remove from Playlist", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static int NextId(IEnumerable<int> ids)
        {
            return ids.Any() ? ids.Max() + 1 : 1;
        }

        private static string AskText(string title, string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, title, defaultValue);
        }

    }
}