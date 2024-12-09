using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using WebAPI.Models;

namespace WebAPI.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ProductsController : Controller
    {
        string uri = "https://localhost:44369/api/";
        HttpClient client = new HttpClient();
        public async Task<IActionResult> Index(string name, int? page = 1, int pageSize = 3)
        {
            client.BaseAddress = new Uri(uri);
            List<Product> products = new List<Product>();
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 3;
            if (!string.IsNullOrEmpty(name))
            {
                products = JsonConvert.DeserializeObject<List<Product>>(await client.GetStringAsync($"products/search?name={name}&page={page}"));
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<Product>>(await client.GetStringAsync($"products?page={page}"));
            }
            // Lấy tổng số sản phẩm để tính toán số trang
            var totalProducts = !string.IsNullOrEmpty(name)
            ? await client.GetStringAsync($"products/counts?name={name}")
            : await client.GetStringAsync("products/counts");
            int totalProductCount = JsonConvert.DeserializeObject<int>(totalProducts);

            // Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

            // Nếu page quá lớn, đặt lại thành trang cuối cùng
            if (page > totalPages)
            {
                page = totalPages;
            }
            // Truyền dữ liệu vào ViewData
            ViewBag.url = "https://localhost:44369";
            ViewData["SearchName"] = name;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            client.BaseAddress = new Uri(uri);

            // Gọi API để lấy danh sách Categories
            var categoriesResponse = await client.GetStringAsync("categories");
            var categories = JsonConvert.DeserializeObject<List<Category>>(categoriesResponse);

            // Gọi API để lấy danh sách Suppliers
            var suppliersResponse = await client.GetStringAsync("suppliers");
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(suppliersResponse);

            // Truyền dữ liệu vào ViewData để sử dụng trong dropdown list
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "SupplierName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile fileupload)
        {
            client.BaseAddress = new Uri(uri);
            // Kiểm tra model trước khi gọi API
            if (!ModelState.IsValid)
            {
                // Gọi API để lấy danh sách Categories
                var categoriesResponse = await client.GetStringAsync("categories");
                var categories = JsonConvert.DeserializeObject<List<Category>>(categoriesResponse);

                // Gọi API để lấy danh sách Suppliers
                var suppliersResponse = await client.GetStringAsync("suppliers");
                var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(suppliersResponse);

                // Truyền dữ liệu vào ViewData để sử dụng trong dropdown list
                ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
                ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "SupplierName");
                return View(product);
            }

            // Đảm bảo xử lý upload file
            if (fileupload != null && fileupload.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileupload.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileupload.CopyToAsync(stream);
                }
                product.Image = "/images/" + fileupload.FileName;
            }

            // Gửi dữ liệu vào API
            client.BaseAddress = new Uri(uri);
            var response = await client.PostAsJsonAsync("products", new
            {
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                Price = product.Price,
                SalePrice = product.SalePrice,
                Image = product.Image,
                SupplierId = product.SupplierId,
                Description = product.Description,
                Stock = product.Stock
            });

            // Kiểm tra nếu phản hồi trả về trạng thái lỗi
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ViewBag.errName = "<span asp-validation-for='ProductName' class='text-danger'>Product name already exists</span>";

                // Gọi API để lấy danh sách Categories
                var categoriesResponse = await client.GetStringAsync("categories");
                var categories = JsonConvert.DeserializeObject<List<Category>>(categoriesResponse);

                // Gọi API để lấy danh sách Suppliers
                var suppliersResponse = await client.GetStringAsync("suppliers");
                var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(suppliersResponse);

                // Truyền dữ liệu vào ViewData để sử dụng trong dropdown list
                ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
                ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "SupplierName");
                return View(product);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorResponse);

                // Gọi API để lấy danh sách Categories
                var categoriesResponse = await client.GetStringAsync("categories");
                var categories = JsonConvert.DeserializeObject<List<Category>>(categoriesResponse);

                // Gọi API để lấy danh sách Suppliers
                var suppliersResponse = await client.GetStringAsync("suppliers");
                var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(suppliersResponse);

                // Truyền dữ liệu vào ViewData để sử dụng trong dropdown list
                ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
                ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "SupplierName");
                return View(product);
            }

            TempData["success"] = "Create success!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            client.BaseAddress = new Uri(uri);
            var response = await client.DeleteAsync($"products/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();

                // Thêm lỗi vào ModelState
                ModelState.AddModelError(string.Empty, errorResponse);

                return RedirectToAction("Index");
            }
            TempData["success"] = "Delete success!";
            return RedirectToAction("Index");
        }
    }
}
