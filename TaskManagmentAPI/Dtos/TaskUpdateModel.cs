namespace TaskManagmentAPI.Dtos
{
    public class TaskUpdateModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Assignee { get; set; }
        public DateTime DueDate { get; set; }
        public int Status { get; set; }
    }
}
