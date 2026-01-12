using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace Labb2_NEU25G.Models;

public partial class PlaylistTrack
{
    public int PlaylistId { get; set; }

    public int TrackId { get; set; }

    public virtual Playlist Playlist { get; set; } = null!;

    public virtual Track Track { get; set; } = null!;
}