using System;
using System.Collections.Generic;

namespace TaskManagmentAPI.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? AssigneeId { get; set; }

    public DateTime DueDate { get; set; }

    public int? StatusId { get; set; }

    public virtual Assignee? Assignee { get; set; }

    public virtual Status? Status { get; set; }
}
