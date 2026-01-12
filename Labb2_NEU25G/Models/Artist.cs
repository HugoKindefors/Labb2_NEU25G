using Labb2_NEU25G.Models;
using System;
using System.Collections.Generic;

namespace Labb2_NEU25G.Models;

public partial class Artist
{
    public int ArtistId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
}