using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebAPI.Models;

namespace WebAPI.Areas.Admin.Controllers
{
    [Area("admin")]
    public class CategoriesController : Controller
    {
        string uri = "https://localhost:44369/api/";
        HttpClient client = new HttpClient();
        public async Task<IActionResult> Index(string name , int? page = 1 , int pageSize = 3 )
        {
            client.BaseAddress = new Uri( uri );
            List<Category> categories = new List<Category>();
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 3;
            if(!string.IsNullOrEmpty(name))
            {
                categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync($"categories/search?name={name}&page={page}"));
            }
            else
            {
                categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync($"categories?page={page}"));
            }
            // Lấy tổng số sản phẩm để tính toán số trang
            var totalCategories = !string.IsNullOrEmpty(name)
            ? await client.GetStringAsync($"categories/counts?name={name}")
            : await client.GetStringAsync("categories/counts");
            int totalCategoryCount = JsonConvert.DeserializeObject<int>(totalCategories);

            // Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalCategoryCount / pageSize);

            // Nếu page quá lớn, đặt lại thành trang cuối cùng
            if (page > totalPages)
            {
                page = totalPages;
            }
            // Truyền dữ liệu vào ViewData
            ViewData["SearchName"] = name;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;
            return View(categories);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            // Kiểm tra model trước khi gọi API
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            client.BaseAddress = new Uri(uri);
            var response = await client.PostAsJsonAsync("categories", new
            {
                CategoryName = category.CategoryName,
                Status = category.Status,
                CreatedAt = category.CreatedAt
            });

            // Kiểm tra nếu phản hồi trả về trạng thái lỗi
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) // HTTP 409: Conflict
            {
                ViewBag.errName = "<span asp-validation-for='CategoryName' class='text-danger'>Category name already exists</span>";
                return View(category);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();

                // Thêm lỗi vào ModelState
                ModelState.AddModelError(string.Empty, errorResponse);

                return View(category);
            }
            TempData["success"] = "Create success!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            client.BaseAddress = new Uri(uri);
            var category = JsonConvert.DeserializeObject<Category>(await client.GetStringAsync($"categories/{id}"));
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            // Kiểm tra model trước khi gọi API
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            client.BaseAddress = new Uri(uri);
            var response = await client.PutAsJsonAsync($"categories/{id}", category);
            // Kiểm tra nếu phản hồi trả về trạng thái lỗi
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) // HTTP 409: Conflict
            {
                ViewBag.errName = "<span asp-validation-for='CategoryName' class='text-danger'>Category name already exists</span>";
                return View(category);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();

                // Thêm lỗi vào ModelState
                ModelState.AddModelError(string.Empty, errorResponse);

                return View(category);
            }
            TempData["success"] = "Update success!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            client.BaseAddress = new Uri(uri);
            var response = await client.DeleteAsync($"categories/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["errDelete"] = "Cannot delete this category because it contains products";
                return RedirectToAction("Index");
            }
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
