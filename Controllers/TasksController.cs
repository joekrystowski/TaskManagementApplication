using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagementApplication.Data;
using TaskManagementApplication.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace TaskManagementApplication.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskDbContext _context;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ILogger<TasksController> logger, TaskDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Tasks
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder = "date_asc", string currentSort = "date_asc")
        {
            // Set default sorting if parameters are null or empty
            sortOrder = String.IsNullOrEmpty(sortOrder) ? "date_asc" : sortOrder;
            currentSort = String.IsNullOrEmpty(currentSort) ? "date_asc" : currentSort;

            // If sortOrder is null or empty, use the current sort order
            sortOrder = String.IsNullOrEmpty(sortOrder) ? currentSort : sortOrder;

            // Toggling sort order
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewBag.DescriptionSortParm = sortOrder == "description_asc" ? "description_desc" : "description_asc";
            ViewBag.DateSortParm = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.PrioritySortParm = sortOrder == "priority_asc" ? "priority_desc" : "priority_asc";
            ViewBag.StatusSortParm = sortOrder == "status_asc" ? "status_desc" : "status_asc";

            var tasks = from t in _context.Task
                        select t;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);        

            switch (sortOrder)
            {
                case "name_desc":
                    tasks = tasks.OrderByDescending(t => t.Name);
                    break;
                case "name_asc":
                    tasks = tasks.OrderBy(t => t.Name);
                    break;
                case "description_desc":
                    tasks = tasks.OrderByDescending(t => t.Description);
                    break;
                case "description_asc":
                    tasks = tasks.OrderBy(t => t.Description);
                    break;
                case "date_desc":
                    tasks = tasks.OrderByDescending(t => t.DueDate);
                    break;
                case "date_asc":
                    tasks = tasks.OrderBy(t => t.DueDate);
                    break;
                case "priority_desc":
                    tasks = tasks.OrderByDescending(t => t.Priority);
                    break;
                case "priority_asc":
                    tasks = tasks.OrderBy(t => t.Priority);
                    break;
                case "status_desc":
                    tasks = tasks.OrderByDescending(t => t.Status);
                    break;
                case "status_asc":
                    tasks = tasks.OrderBy(t => t.Status);
                    break;
                default:
                    tasks = tasks.OrderBy(t => t.Name); // Default sorting order
                    break;
            }

            tasks = (IQueryable<Models.Task>)tasks.Where(t => t.UserId == userId);

            return View(await tasks.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        [Authorize]
        public IActionResult Create() 
        {
            _logger.LogInformation("Started Creating task.");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Status,DueDate,Priority")] Models.Task task)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    task.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _context.Add(task);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the error
                    _logger.LogError(ex, "Error creating task.");
                }
            }
            else
            {
                _logger.LogInformation("Model state is not valid");
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                _logger.LogWarning("Validation failed: " + string.Join("; ", errors.Select(e => e.ErrorMessage)));
            }
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Status,DueDate,Priority")] Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            task.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Task.FindAsync(id);
            if (task != null)
            {
                _context.Task.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(int id )
        {
            var task = await _context.Task.FindAsync(id);

            if (task == null) 
            {
                return NotFound();
            }
       
            task.Status = Constants.Status.Completed;

           
            try
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        private bool TaskExists(int id)
        {
            return _context.Task.Any(e => e.Id == id);
        }
    }
}
