using System;
using System.Collections.Generic;

namespace PersonalAssistant.Models;

public partial class Status
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Type { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
