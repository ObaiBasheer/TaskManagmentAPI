using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagmentAPI.Dtos;
using TaskManagmentAPI.Models;
using TaskManagmentAPI.Services;

namespace TaskManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;
        private readonly IEmailService _emailService;

        public TasksController(TaskManagementDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(RequestJsonData), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RequestJsonData>> GetTasks()
        {

            var tasks = await _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.Status)
                .Select(t => new TaskModel
                {
                    Title = t.Title,
                    Description = t.Description!,
                    AssigneeName = t.Assignee != null ? t.Assignee.Name : "Unassigned",
                    StatusName = t.Status != null ? t.Status.Name : "Undefined"
                })
                .ToListAsync();

            return new RequestJsonData(tasks, "All Data", 200, "items");
        }

        [HttpGet("GetById")]
        [ProducesResponseType(typeof(RequestJsonData), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RequestJsonData>> GetTask([FromQuery] int taskId)
        {
            var taskList = new List<TaskModel>();

            if (taskId <= 0)
            {
                return BadRequest("Please provide a valid taskId");
            }

            var task = await _context.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return NotFound();
            }

            var taskModel = new TaskModel
            {
                Title = task.Title,
                AssigneeName = task.Assignee?.Name!,
                Description = task.Description!,
                StatusName = task.Status?.Name!
            };
            taskList.Add(taskModel);
            return new RequestJsonData(taskList, "Task",200, "Successful");
        }

        [HttpPost]
        [ProducesResponseType(typeof(RequestJsonData), 201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RequestJsonData>> CreateTask(TaskCreateModel taskCreate)
        {
            var mapping = new Mapping();
            var taskList = new List<TaskModel>();

            if (taskCreate == null)
            {
                return BadRequest("Please provide valid data for task creation");
            }

            try
            {
                if (taskCreate.Assignee != 0)
                {
                    var email = _context.Assignees.FirstOrDefault(x => x.Id == taskCreate.Assignee)?.Email;
                    if (email != null)
                    {
                        await SendAssignmentEmailAsync(email, taskCreate.Title!);
                    }
                }

                var newTask = Mapping.MapTaskCreateToTask(taskCreate);
                _context.Tasks.Add(newTask);
                await _context.SaveChangesAsync();

                var taskModel = mapping.CreateTaskModel(newTask);
                taskList.Add(taskModel);

                return new RequestJsonData(taskList, "Task Created Successfully", 200, "Success");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to create task. Please try again. : " + ex.Message);
            }
        }

        [HttpPut()]
        [ProducesResponseType(typeof(RequestJsonData),204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RequestJsonData>> UpdateTask([FromQuery] int taskId, TaskUpdateModel task)
        {
            if (taskId != task.Id)
            {
                return BadRequest("TaskId in the URL does not match the TaskId in the request body");
            }

            var existingTask = await _context.Tasks.FindAsync(taskId);

            if (existingTask == null)
            {
                return  NotFound();
            }

            existingTask.Title = task.Title!;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.StatusId = task.Status;
            existingTask.AssigneeId = task.Assignee;

            try
            {
                await _context.SaveChangesAsync();

                if (await SendAssignmentEmailAsync(existingTask.Assignee!.Email, task.Title!)) return Ok("Task Is Assigned and Email is send ");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(taskId))
                {
                    return NotFound();

                }
                else
                {
                    throw;
                }
            }

            return new RequestJsonData(null!, $"Task with Id {taskId} Updated Successfully", 200, "Successful Update");
        }

        [HttpDelete("Delete")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTask([FromQuery] int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok("Task Is Deleted");
        }

        [HttpPost("Assign")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AssignTask([FromQuery] int taskId, [FromQuery] int assigneeId)
        {
            if (taskId <= 0 || assigneeId <= 0)
            {
                return BadRequest("Please provide valid taskId and assigneeId");
            }

            var task = await _context.Tasks.FindAsync(taskId);
            var assignee = await _context.Assignees.FindAsync(assigneeId);

            if (task == null || assignee == null)
            {
                return NotFound("Task or Assignee not found");
            }

            task.AssigneeId = assigneeId;
            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                if (await SendAssignmentEmailAsync(assignee.Email, task.Title)) return Ok("Task Is Assigned and Email is send ");

                return BadRequest("something wrong happened");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(taskId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private async System.Threading.Tasks.Task<bool> SendAssignmentEmailAsync(string assigneeEmail, string taskTitle)
        {
            return await _emailService.SendEmailAsync(assigneeEmail, "Task Assignment", $"You have been assigned to the task: {taskTitle}");
        }

        [HttpGet("GenerateReport")]
        [ProducesResponseType(200, Type = typeof(FileContentResult))]
        [ProducesResponseType(500)]
        public IActionResult GenerateReport()
        {
            var tasks = _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.Status)
                .Select(t => new
                {
                    Title = t.Title,
                    Description = t.Description,
                    AssigneeName = t.Assignee != null ? t.Assignee.Name : "Unassigned",
                    StatusName = t.Status != null ? t.Status.Name : "Undefined"
                })
                .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("TaskReport");

                var dataRange = worksheet.Cells["A1:D" + (tasks.Count + 1)];

                worksheet.Cells["A1"].Value = "Title";
                worksheet.Cells["B1"].Value = "Description";
                worksheet.Cells["C1"].Value = "Assignee";
                worksheet.Cells["D1"].Value = "Status";

                var row = 2;
                foreach (var task in tasks)
                {
                    worksheet.Cells[row, 1].Value = task.Title;
                    worksheet.Cells[row, 2].Value = task.Description;
                    worksheet.Cells[row, 3].Value = task.AssigneeName;
                    worksheet.Cells[row, 4].Value = task.StatusName;

                    row++;
                }

                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 40;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 25;

                var table = worksheet.Tables.Add(dataRange, "TaskTable");
                table.ShowHeader = true;
                table.TableStyle = TableStyles.Medium9;

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TaskReport.xlsx");
            }
        }

        private bool TaskExists(int taskId)
        {
            return _context.Tasks.Any(e => e.Id == taskId);
        }
    }
}

