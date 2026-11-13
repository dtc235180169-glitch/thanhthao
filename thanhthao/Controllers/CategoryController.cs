using thanhthao.Models;
using Microsoft.AspNetCore.Mvc;

namespace thanhthao.Controllers
{
    public class CategoryController : Controller
    {
        private const int PageSize = 3;

        // GET: Category?searchString=abc&pageNumber=2
        public IActionResult Index(string? searchString, int pageNumber = 1)
        {
            IEnumerable<Category> categories = DataStore.Categories;

            // Search by name
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                categories = categories.Where(c =>
                    c.CategoryName != null &&
                    c.CategoryName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            // Pagination
            int totalItems = categories.Count();
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)PageSize));
            pageNumber = Math.Clamp(pageNumber, 1, totalPages);

            ViewBag.SearchString = searchString;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;

            var model = categories
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return View(model);
        }

        // GET: Category/Details/5
        public IActionResult Details(int id)
        {
            var category = DataStore.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null) return NotFound();

            // Navigation property: products of this category
            category.Products = DataStore.Products.Where(p => p.CategoryId == id).ToList();
            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            category.CategoryId = DataStore.NextCategoryId();
            DataStore.Categories.Add(category);
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int id)
        {
            var category = DataStore.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.CategoryId) return NotFound();
            if (!ModelState.IsValid) return View(category);

            var existing = DataStore.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (existing == null) return NotFound();

            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int id)
        {
            var category = DataStore.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null) return NotFound();

            ViewBag.ProductCount = DataStore.Products.Count(p => p.CategoryId == id);
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = DataStore.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null) return NotFound();

            // A category that still has products cannot be deleted (foreign key integrity)
            int productCount = DataStore.Products.Count(p => p.CategoryId == id);
            if (productCount > 0)
            {
                ViewBag.ProductCount = productCount;
                ViewBag.Error = "This category cannot be deleted because it still has products.";
                return View(category);
            }

            DataStore.Categories.Remove(category);
            return RedirectToAction(nameof(Index));
        }
    }
}
