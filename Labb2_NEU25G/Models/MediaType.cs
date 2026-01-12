using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace Labb2_NEU25G.Models;

public partial class MediaType
{
    public int MediaTypeId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}