using System;
using System.Collections.Generic;

namespace PersonalAssistant.Models;

public partial class Feeling
{
    public int Id { get; set; }

    public int Level { get; set; }

    public string? Description { get; set; }

    public DateOnly Date { get; set; }

    public virtual ICollection<Emotion> Emotions { get; set; } = new List<Emotion>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
