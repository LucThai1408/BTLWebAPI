using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebAPI.Models;

namespace WebAPI.Areas.Admin.Controllers
{
    [Area("admin")]
    public class SuppliersController : Controller
    {
        string uri = "https://localhost:44369/api/";
        HttpClient client = new HttpClient();
        public async Task<IActionResult> Index(string name, int? page = 1, int pageSize = 3)
        {
            client.BaseAddress = new Uri(uri);
            List<Supplier> suppliers = new List<Supplier>();
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 3;
            if (!string.IsNullOrEmpty(name))
            {
                suppliers = JsonConvert.DeserializeObject<List<Supplier>>(await client.GetStringAsync($"suppliers/search?name={name}&page={page}"));
            }
            else
            {
                suppliers = JsonConvert.DeserializeObject<List<Supplier>>(await client.GetStringAsync($"suppliers?page={page}"));
            }
            // Lấy tổng số sản phẩm để tính toán số trang
            var totalSuppliers = !string.IsNullOrEmpty(name)
            ? await client.GetStringAsync($"suppliers/counts?name={name}")
            : await client.GetStringAsync("suppliers/counts");
            int totalSupplierCount = JsonConvert.DeserializeObject<int>(totalSuppliers);

            // Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalSupplierCount / pageSize);

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
            return View(suppliers);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            // Kiểm tra model trước khi gọi API
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            client.BaseAddress = new Uri(uri);
            var response = await client.PostAsJsonAsync("suppliers", new
            {
                SupplierName = supplier.SupplierName,
                Phone = supplier.Phone,
                Address = supplier.Address
            });

            // Kiểm tra nếu phản hồi trả về trạng thái lỗi
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) // HTTP 409: Conflict
            {
                ViewBag.errName = "<span asp-validation-for='SupplierName' class='text-danger'>Supplier name already exists</span>";
                return View(supplier);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();

                // Thêm lỗi vào ModelState
                ModelState.AddModelError(string.Empty, errorResponse);

                return View(supplier);
            }
            TempData["success"] = "Create success!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            client.BaseAddress = new Uri(uri);
            var supplier = JsonConvert.DeserializeObject<Supplier>(await client.GetStringAsync($"suppliers/{id}"));
            return View(supplier);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,  Supplier supplier)
        {
            // Kiểm tra model trước khi gọi API
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }
            client.BaseAddress = new Uri(uri);
            var response = await client.PutAsJsonAsync($"suppliers/{id}", supplier);
            // Kiểm tra nếu phản hồi trả về trạng thái lỗi
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) // HTTP 409: Conflict
            {
                ViewBag.errName = "<span asp-validation-for='SupplierName' class='text-danger'>Supplier name already exists</span>";
                return View(supplier);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();

                // Thêm lỗi vào ModelState
                ModelState.AddModelError(string.Empty, errorResponse);

                return View(supplier);
            }
            TempData["success"] = "Update success!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            client.BaseAddress = new Uri(uri);
            var response = await client.DeleteAsync($"suppliers/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["errDelete"] = "Cannot delete this supplier because it contains products";
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
