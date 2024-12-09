using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BTLWebAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace BTLWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /*[Authorize(Roles = "admin")]*/
    public class CategoriesController : ControllerBase
    {
        private readonly BTLContext _context;

        public CategoriesController(BTLContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int page = 1, int pageSize = 3)
        {
            return await _context.Categories.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }


        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }



        [HttpGet("counts")]
        public async Task<int> TotalCategory(string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return await _context.Categories.Where(x=>x.CategoryName.Contains(name.Trim().ToLower())).CountAsync();
            }
            return await _context.Categories.CountAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpGet("search")]
        public IActionResult SearchByName(string name, int? categoryId, int page = 1, int pageSize = 3)
        {

            if (string.IsNullOrEmpty(name))
            {
                // Trả về toàn bộ danh sách nếu không có tên
                return Ok(_context.Products.Skip((page - 1) * pageSize).Take(pageSize).ToList());
            }


            // Lọc danh sách sản phẩm theo tên
            var categories = _context.Categories
                .Where(x => x.CategoryName.ToLower().Contains(name.Trim().ToLower()))
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();

            /*if (categoryId.HasValue)
            {
                return Ok(_context.Products.Where(x => x.CategoryId == categoryId));
            }*/
            return Ok(categories);
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return Ok("Not found");
            }
            var categoryname = _context.Categories.FirstOrDefault(x => x.CategoryName.Contains(category.CategoryName) && x.CategoryId != category.CategoryId);
            if (categoryname != null )
            {
                return BadRequest(new { message = "CategoryName already exists" });
            }
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update success!");
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            var categoryname = _context.Categories.FirstOrDefault(x => x.CategoryName.Contains(category.CategoryName));

            if (categoryname !=null)
            {
                return BadRequest(new { message = "CategoryName already exists" });
            }
            if (!ModelState.IsValid)
            {
                return Ok(ModelState);
            }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var products = await _context.Products.Where(x => x.CategoryId == category.CategoryId).CountAsync();
            if (products > 0) {
                return BadRequest(new { message = "Cannot delete this category because it contains products" });
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

    }
}
