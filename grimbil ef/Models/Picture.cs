using System;
using System.Collections.Generic;

namespace grimbil_ef.Models;

public partial class Picture
{
    public int Pictureid { get; set; }

    public byte[]? Picture1 { get; set; }

    public int? Postid { get; set; }

    public virtual Post? Post { get; set; }
}
