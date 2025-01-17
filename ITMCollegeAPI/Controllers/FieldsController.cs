﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITMCollegeAPI.Models;

namespace ITMCollegeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldsController : ControllerBase
    {
        private readonly ITMCollegeContext _context;

        public FieldsController(ITMCollegeContext context)
        {
            _context = context;
        }

        // GET: api/Fields
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Field>>> GetFields()
        {
            return await _context.Fields
                .OrderByDescending(i => i.FieldId)
                .AsNoTracking()
                .Include(i=>i.Stream)
                .ToListAsync();
        }

        [HttpGet]
        [Route("GetFieldsByStreamId/{id}")]
        public async Task<ActionResult<IEnumerable<Field>>> GetFieldsByStreamId(int id)
        {
            return await _context.Fields
               
                .AsNoTracking()
                .Include(i => i.Stream).Where(i=>i.StreamId==id)
                .ToListAsync();
        }

        // GET: api/Fields/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Field>> GetField(int id)
        {
            var field = await _context.Fields.FindAsync(id);

            if (field == null)
            {
                return NotFound();
            }

            return field;
        }

        // PUT: api/Fields/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutField(int id, Field field)
        {
            if (id != field.FieldId)
            {
                return BadRequest();
            }

            _context.Entry(field).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FieldExists(id))
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

        // POST: api/Fields
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Field>> PostField(Field field)
        {
            _context.Fields.Add(field);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        // DELETE: api/Fields/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            var field = await _context.Fields.FindAsync(id);
            if (field == null)
            {
                return NotFound();
            }

            _context.Fields.Remove(field);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FieldExists(int id)
        {
            return _context.Fields.Any(e => e.FieldId == id);
        }
        [HttpGet("GetFieldByFieldId")]
        public Field GetFieldByFieldId(int id)
        {
            var field = _context.Fields.Find(id);

            if (field == null)
            {
                return null;
            }

            return field;
        }
    }
}
