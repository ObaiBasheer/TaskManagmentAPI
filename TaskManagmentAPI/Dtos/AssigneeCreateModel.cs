using System.ComponentModel.DataAnnotations;

namespace TaskManagmentAPI.Dtos
{
    public class AssigneeCreateModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]

        public string Email { get; set; } = null!;
    }
}
