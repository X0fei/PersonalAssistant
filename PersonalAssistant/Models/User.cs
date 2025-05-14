using System;
using System.Collections.Generic;

namespace PersonalAssistant.Models;

public partial class User
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Bio { get; set; }

    public int? MainPfp { get; set; }

    public int Lvl { get; set; }

    public long Xp { get; set; }

    public virtual Pfp? MainPfpNavigation { get; set; }

    public virtual ICollection<Feeling> Feelings { get; set; } = new List<Feeling>();

    public virtual ICollection<List> Lists { get; set; } = new List<List>();

    public virtual ICollection<Pfp> Pfps { get; set; } = new List<Pfp>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
