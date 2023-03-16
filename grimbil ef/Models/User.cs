using System;
using System.Collections.Generic;

namespace grimbil_ef.Models;

public partial class User
{
    public int Userid { get; set; }

    public string Useremail { get; set; } = null!;

    public string Userpassword { get; set; } = null!;

    public int Usertype { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Post> Posts { get; } = new List<Post>();

    public virtual ICollection<Rating> Ratings { get; } = new List<Rating>();
}
