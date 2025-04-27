using System;
using System.Collections.Generic;

namespace PersonalAssistant.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Priority { get; set; }

    public int Status { get; set; }

    public int? PriorityTable { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual Priority? PriorityNavigation { get; set; }

    public virtual PriorityTable? PriorityTableNavigation { get; set; }

    public virtual Status StatusNavigation { get; set; } = null!;

    public virtual ICollection<List> Lists { get; set; } = new List<List>();

    public virtual ICollection<Task> SubTasks { get; set; } = new List<Task>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
