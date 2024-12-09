using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml.Linq;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ShopController : Controller
    {
        string uri = "https://localhost:44369/api/";
        HttpClient client = new HttpClient();
        public async Task<IActionResult> Index(string name, int? categoryId , int? page = 1 , int pageSize = 3)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 3;
            client.BaseAddress = new Uri(uri);
            List<Product> products = new List<Product>();
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
            : await client.GetStringAsync($"products/counts");
            int totalProductCount = JsonConvert.DeserializeObject<int>(totalProducts);

            // Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);
            // Lấy danh mục sản phẩm
            var categories = JsonConvert.DeserializeObject<List<Category>>(await client.GetStringAsync("categories/GetAllCategories"));

            // Nếu page quá lớn, đặt lại thành trang cuối cùng
            if (page > totalPages)
            {
                page = totalPages;
            }
            // Truyền dữ liệu vào ViewData
            ViewBag.url = "https://localhost:44369";
            ViewBag.Categories = categories;
            ViewData["SearchName"] = name;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;
            return View(products);
        }

		public async Task<IActionResult> ProductByCategory(string name, int? categoryId, int page = 1, int pageSize = 3)
		{
			client.BaseAddress = new Uri(uri);

			// Lấy danh sách sản phẩm theo danh mục
			var productsResponse = await client.GetStringAsync($"products/ProductByCategory?categoryId={categoryId}&page={page}&pageSize={pageSize}");
			var products = !string.IsNullOrEmpty(productsResponse)
				? JsonConvert.DeserializeObject<List<Product>>(productsResponse)
				: new List<Product>();

			// Lấy danh sách danh mục
			var categoriesResponse = await client.GetStringAsync("categories/GetAllCategories");
			var categories = !string.IsNullOrEmpty(categoriesResponse)
				? JsonConvert.DeserializeObject<List<Category>>(categoriesResponse)
				: new List<Category>();

			// Lấy tổng số sản phẩm theo danh mục
			var totalProductsResponse = await client.GetStringAsync($"products/ProductCountByCategory?categoryId={categoryId}");
			int totalProductCount = !string.IsNullOrEmpty(totalProductsResponse)
				? JsonConvert.DeserializeObject<int>(totalProductsResponse)
				: 0;

			// Tính toán tổng số trang
			int totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

            // Truyền dữ liệu vào ViewBag và ViewData
            ViewBag.url = "https://localhost:44369";
            ViewBag.Categories = categories;
			ViewBag.CurrentCategory = categoryId;
			ViewData["SearchName"] = name;
			ViewData["CurrentPage"] = page;
			ViewData["TotalPages"] = totalPages;
			ViewData["PageSize"] = pageSize;

			// Render lại view Index, sử dụng danh sách sản phẩm
			return View("Index", products);
		}


		public async Task<IActionResult> Details(int id)
        {
            ViewBag.url = "https://localhost:44369";
            client.BaseAddress = new Uri(uri);
            var product = JsonConvert.DeserializeObject<Product>(await client.GetStringAsync($"products/{id}"));
            return View(product);
        }
    }
}
