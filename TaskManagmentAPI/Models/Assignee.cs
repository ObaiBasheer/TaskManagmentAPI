using System;
using System.Collections.Generic;

namespace TaskManagmentAPI.Models;

public partial class Assignee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
