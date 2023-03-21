using System;
using System.Collections.Generic;

namespace grimbil_ef.Models;

public partial class Picture
{
    public int Pictureid { get; set; }

    public string Picture1 { get; set; } = null!;

    public int? Postid { get; set; }

    public virtual Post? Post { get; set; }
}
