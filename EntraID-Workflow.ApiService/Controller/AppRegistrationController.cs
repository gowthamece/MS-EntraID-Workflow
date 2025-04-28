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

    public class AppRegistrationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppRegistrationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppRegistration>>> GetAppRegistrations()
        {
            return await _context.AppRegistrations.Include(ar => ar.AppType).Include(ar => ar.Status).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppRegistration>> GetAppRegistration(int id)
        {
            var appRegistration = await _context.AppRegistrations.Include(ar => ar.AppType).Include(ar => ar.Status).FirstOrDefaultAsync(ar => ar.Id == id);

            if (appRegistration == null)
            {
                return NotFound();
            }

            return appRegistration;
        }

        [HttpPost]
        public async Task<ActionResult<AppRegistration>> CreateAppRegistration(AppRegistration appRegistration)
        {
            try
            {
                appRegistration.AppType = null;
                appRegistration.Status = null;

                _context.AppRegistrations.Add(appRegistration);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAppRegistration), new { id = appRegistration.Id }, appRegistration);
            }
            catch(Exception ex)
            {
                return CreatedAtAction(nameof(GetAppRegistration), new { id = appRegistration.Id }, appRegistration);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppRegistration(int id, AppRegistration appRegistration)
        {
            if (id != appRegistration.Id)
            {
                return BadRequest();
            }

            _context.Entry(appRegistration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppRegistrationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppRegistration(int id)
        {
            var appRegistration = await _context.AppRegistrations.FindAsync(id);
            if (appRegistration == null)
            {
                return NotFound();
            }

            _context.AppRegistrations.Remove(appRegistration);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppRegistrationExists(int id)
        {
            return _context.AppRegistrations.Any(e => e.Id == id);
        }
    }
}