using System;
using System.Collections.Generic;

namespace PersonalAssistant.Models;

public partial class EisenhowerMatrix
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Strength { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
