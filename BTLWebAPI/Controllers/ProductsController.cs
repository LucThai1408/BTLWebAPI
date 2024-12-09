using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BTLWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Printing;

namespace BTLWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /*[Authorize(Roles = "admin")]*/
    public class ProductsController : ControllerBase
    {
        private readonly BTLContext _context;

        public ProductsController(BTLContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int page = 1 , int pageSize = 3)
        {
            return await _context.Products.OrderByDescending(x=> x.CreatedAt).Skip((page - 1)*pageSize).Take(pageSize).ToListAsync();
        }

        [HttpGet("counts")]
        public async Task<int> TotalProducts(string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return await _context.Products.Where(x=> x.ProductName.ToLower().Contains(name.Trim().ToLower())).CountAsync();
            }
            return await _context.Products.CountAsync();
        }
		// GET: api/Products/5
		[HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return Ok();
            }

            return product;
        }

        //Get: api/Products/search?name=name
        [HttpGet("search")]
        public IActionResult SearchByName(string name,int? categoryId, int page = 1, int pageSize = 3) 
        {

            if (string.IsNullOrEmpty(name))
            {
                // Trả về toàn bộ danh sách nếu không có tên
                return Ok(_context.Products.Skip((page - 1)*pageSize).Take(pageSize).ToList());
            }


            // Lọc danh sách sản phẩm theo tên
            var products = _context.Products
                .Where(x => x.ProductName.ToLower().Contains(name.Trim().ToLower()))
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(products);
        }

        //Get
        [HttpGet("ProductByCategory")]
        public IActionResult ProductByCategory(int? categoryId , int page = 1 , int pageSize = 3) {
            if (categoryId == null ) { 
                var allProduct = _context.Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();    
            }

            var products = _context.Products.Where(x => x.CategoryId == categoryId).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            /*if (products == null || !products.Any())
            {
                return NotFound($"No products found for Category ID: {categoryId}");
            }
*/
            return Ok(products);
        }

		[HttpGet("ProductCountByCategory")]
		public async Task<int> ProductCountByCategory(int? categoryId)
		{
			return await _context.Products.Where(x => x.CategoryId == categoryId ).CountAsync();

		}
		// PUT: api/Products/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromForm] ProductImage data)
        {
            if (id != data.ProductId)
            {
                return Ok("ProductId does not match");
            }
            var product = new Product { ProductId = data.ProductId, ProductName = data.ProductName, CategoryId = data.CategoryId, Price = data.Price, SalePrice = data.SalePrice, Image = data.PictureOld, SupplierId = data.SupplierId,Description = data.Description, Stock = data.Stock, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };

            //xử lý upload ảnh
            if (data.Image != null && data.Image.Length > 0)
            {   
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.Image.FileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await data.Image.CopyToAsync(stream);
                }
                product.Image = "/images/" + data.Image.FileName;
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update Success!");
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostProduct([FromForm] ProductImage data)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'BTLContext.Products'  is null.");
            }
            var find = await _context.Products.FindAsync(data.ProductId);
            if (find != null)
                return Ok("ProductId already exists");
            var product = new Product { ProductId = data.ProductId, ProductName = data.ProductName,CategoryId = data.CategoryId, Price = data.Price,SalePrice = data.SalePrice, Image = null  , SupplierId = data.SupplierId  , Description = data.Description,Stock = data.Stock ,CreatedAt = DateTime.Now , UpdatedAt = DateTime.Now};
            //xử lý upload ảnh
            if (data.Image != null && data.Image.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", data.Image.FileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await data.Image.CopyToAsync(stream);

                }
                product.Image = "/images/" + data.Image.FileName;
            }

            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
