using System.ComponentModel.DataAnnotations;

namespace TaskManagmentAPI.Dtos
{
    public class TaskCreateModel
    {
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public int Assignee { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public int Status { get; set; }
    }
}
