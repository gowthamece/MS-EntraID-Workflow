using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntraID.Workflow.ApiService.DBContext;
using EntraID.Workflow.ApiService.Models;

namespace EntraID.Workflow.ApiService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkflowsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Workflows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Workflow.ApiService.Models.Workflow>>> GetWorkflows()
        {
            return await _context.Workflows.ToListAsync();
        }

        // GET: api/Workflows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Workflow.ApiService.Models.Workflow>> GetWorkflow(int id)
        {
            var workflow = await _context.Workflows.FindAsync(id);

            if (workflow == null)
            {
                return NotFound();
            }

            return workflow;
        }

        // PUT: api/Workflows/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkflow(int id, Workflow.ApiService.Models.Workflow workflow)
        {
            if (id != workflow.Id)
            {
                return BadRequest();
            }

            _context.Entry(workflow).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkflowExists(id))
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

        // POST: api/Workflows
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Workflow.ApiService.Models.Workflow>> PostWorkflow(Workflow.ApiService.Models.Workflow workflow)
        {
            _context.Workflows.Add(workflow);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkflow", new { id = workflow.Id }, workflow);
        }

        // DELETE: api/Workflows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkflow(int id)
        {
            var workflow = await _context.Workflows.FindAsync(id);
            if (workflow == null)
            {
                return NotFound();
            }

            _context.Workflows.Remove(workflow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkflowExists(int id)
        {
            return _context.Workflows.Any(e => e.Id == id);
        }
    }
}
