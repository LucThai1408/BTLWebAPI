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
    public class SuppliersController : ControllerBase
    {
        private readonly BTLContext _context;

        public SuppliersController(BTLContext context)
        {
            _context = context;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers(int page = 1, int pageSize = 3)
        {
            return await _context.Suppliers.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }


        [HttpGet("GetAllSuppliers")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }



        [HttpGet("counts")]
        public async Task<int> TotalSuppliers(string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return await _context.Suppliers.Where(x => x.SupplierName.Contains(name.Trim().ToLower())).CountAsync();
            }
            return await _context.Suppliers.CountAsync();
        }


        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return supplier;
        }

        [HttpGet("search")]
        public IActionResult SearchByName(string name, int? categoryId, int page = 1, int pageSize = 3)
        {

            if (string.IsNullOrEmpty(name))
            {
                // Trả về toàn bộ danh sách nếu không có tên
                return Ok(_context.Suppliers.Skip((page - 1) * pageSize).Take(pageSize).ToList());
            }


            // Lọc danh sách sản phẩm theo tên
            var suppliers = _context.Suppliers
                .Where(x => x.SupplierName.ToLower().Contains(name.Trim().ToLower()))
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(suppliers);
        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return Ok("Not found");
            }
            var supplierName = _context.Suppliers.FirstOrDefault(x => x.SupplierName.Contains(supplier.SupplierName) && x.SupplierId != supplier.SupplierId);
            if (supplierName != null)
            {
                return BadRequest(new { message = "SupplierName already exists" });
            }
            _context.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
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

        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
        {
            var supplierName = _context.Suppliers.FirstOrDefault(x => x.SupplierName.Contains(supplier.SupplierName));

            if (supplierName != null)
            {
                return BadRequest(new { message = "SupplierName already exists" });
            }
            if (!ModelState.IsValid)
            {
                return Ok(ModelState);
            }
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSuppliers", new { id = supplier.SupplierId }, supplier);
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            var products = await _context.Products.Where(x => x.SupplierId == supplier.SupplierId).CountAsync();
            if (products > 0)
            {
                return BadRequest(new { message = "Cannot delete this supplier because it contains products" });
            }
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
