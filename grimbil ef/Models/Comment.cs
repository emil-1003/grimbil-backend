using System;
using System.Collections.Generic;

namespace grimbil_ef.Models;

public partial class Comment
{
    public int Commentid { get; set; }

    public string Comment1 { get; set; } = null!;

    public int Userid { get; set; }

    public int Postid { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
