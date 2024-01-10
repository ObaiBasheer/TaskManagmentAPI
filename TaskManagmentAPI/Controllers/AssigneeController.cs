using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagmentAPI.Dtos;
using TaskManagmentAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TaskManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssigneeController : ControllerBase
    {
        private readonly TaskManagementDbContext _context;

        public AssigneeController(TaskManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignee>>> GetAllUsers()
        {
            var assignees = await _context.Assignees.ToListAsync();
            return Ok(assignees);
        }

        [HttpPost]
        public async Task<ActionResult<Assignee>> CreateAssigneeAsync(AssigneeCreateModel assignee)
        {
            if (assignee == null || string.IsNullOrWhiteSpace(assignee.Name))
            {
                return BadRequest("Assignee data is invalid.");
            }

            // Additional validation, e.g., email format validation
            if (!IsValidEmail(assignee.Email))
            {
                return BadRequest("Invalid email format.");
            }

            if (AssigneeExists(assignee.Name))
            {
                return Conflict("Assignee with the same name already exists.");
            }

            var newAssignee = new Assignee
            {
                Name = assignee.Name.Trim(),
                Email = assignee.Email?.Trim()!,
            };

            _context.Assignees.Add(newAssignee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllUsers), new { assigneeId = newAssignee.Id }, newAssignee);
        }

        private bool AssigneeExists(string assigneeName)
        {
            return _context.Assignees.Any(e => e.Name == assigneeName);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string emailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";

            return Regex.IsMatch(email, emailPattern);
        }
    }
}
