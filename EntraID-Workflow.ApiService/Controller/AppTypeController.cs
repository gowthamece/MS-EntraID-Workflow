using EntraID.Workflow.ApiService.DBContext;
using EntraID.Workflow.ApiService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntraID.Workflow.ApiService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AuthZPolicy")]
    public class AppTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppTypeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppType>>> GetAppTypes()
        {
            return await _context.AppTypes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppType>> GetAppType(int id)
        {
            var appType = await _context.AppTypes.FindAsync(id);
            if (appType == null)
                return NotFound();
            return appType;
        }

        [HttpPost]
        public async Task<ActionResult<AppType>> CreateAppType(AppType appType)
        {
            _context.AppTypes.Add(appType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAppType), new { id = appType.Id }, appType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppType(int id, AppType appType)
        {
            if (id != appType.Id)
                return BadRequest();

            _context.Entry(appType).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.AppTypes.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppType(int id)
        {
            var appType = await _context.AppTypes.FindAsync(id);
            if (appType == null)
                return NotFound();

            _context.AppTypes.Remove(appType);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool AppTypeExists(int id)
        {
            return _context.AppTypes.Any(e => e.Id == id);
        }
    }
}