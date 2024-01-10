using Microsoft.EntityFrameworkCore;
using TaskManagmentAPI.Dtos;
using TaskManagmentAPI.Models;

namespace TaskManagmentAPI.Services
{
    public  class Mapping
    {
        private readonly TaskManagementDbContext _context;

        public Mapping()
        {
            _context = new TaskManagementDbContext();
        }
        // Helper method to map TaskCreate to Task
        public static Models.Task MapTaskCreateToTask(TaskCreateModel taskCreate)
        {
            return new Models.Task
            {
                Title = taskCreate.Title!,
                Description = taskCreate.Description,
                AssigneeId = taskCreate.Assignee,
                DueDate = taskCreate.DueDate,
                StatusId = taskCreate.Status
            };
        }

        // Helper method to create TaskModel from Task
        public  TaskModel CreateTaskModel(Models.Task task)
        {
            return new TaskModel
            {
                Title = task.Title,
                Description = task.Description!,
                AssigneeName = _context.Assignees.FirstOrDefault(x => x.Id == task.AssigneeId)?.Name!,
                StatusName = _context.Statuses.FirstOrDefault(x => x.Id == task.StatusId)?.Name!
            };
        }
    }
}
