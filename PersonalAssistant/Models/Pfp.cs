using System;
using System.Collections.Generic;

namespace PersonalAssistant.Models;

public partial class Pfp
{
    public int Id { get; set; }

    public string Path { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<User> UsersNavigation { get; set; } = new List<User>();
}
