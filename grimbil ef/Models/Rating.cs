using System;
using System.Collections.Generic;

namespace grimbil_ef.Models;

public partial class Rating
{
    public int Ratingid { get; set; }

    public int Rating1 { get; set; }

    public int Userid { get; set; }

    public int Postid { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
