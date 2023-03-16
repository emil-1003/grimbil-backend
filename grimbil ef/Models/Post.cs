using System;
using System.Collections.Generic;

namespace grimbil_ef.Models;

public partial class Post
{
    public int Postid { get; set; }

    public int Userid { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Rating> Ratings { get; } = new List<Rating>();

    public virtual User User { get; set; } = null!;
}
