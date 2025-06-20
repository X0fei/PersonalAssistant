using System;
using System.Collections.Generic;

namespace PersonalAssistant.Models;

public partial class Emotion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Feeling> Feelings { get; set; } = new List<Feeling>();
}
