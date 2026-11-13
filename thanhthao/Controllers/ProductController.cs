using thanhthao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace thanhthao.Controllers
{
    public class ProductController : Controller
    {
        private const int PageSize = 3;

        // GET: Product?searchString=abc&pageNumber=2
        // Uses ProductViewModel to show CategoryName instead of CategoryId
        public IActionResult Index(string? searchString, int pageNumber = 1)
        {
            IEnumerable<Product> products = DataStore.Products;

            // Search by name
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                products = products.Where(p =>
                    p.ProductName != null &&
                    p.ProductName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            // Pagination
            int totalItems = products.Count();
            int totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)PageSize));
            pageNumber = Math.Clamp(pageNumber, 1, totalPages);

            ViewBag.SearchString = searchString;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;

            var model = products
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    CategoryName = DataStore.Categories
                        .FirstOrDefault(c => c.CategoryId == p.CategoryId)?.CategoryName
                })
                .ToList();

            return View(model);
        }

        // GET: Product/Details/5
        public IActionResult Details(int id)
        {
            var product = FindWithCategory(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            CheckCategoryExists(product.CategoryId);
            if (!ModelState.IsValid)
            {
                LoadCategories(product.CategoryId);
                return View(product);
            }

            product.ProductId = DataStore.NextProductId();
            DataStore.Products.Add(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Edit/5
        public IActionResult Edit(int id)
        {
            var product = DataStore.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();

            LoadCategories(product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();

            CheckCategoryExists(product.CategoryId);
            if (!ModelState.IsValid)
            {
                LoadCategories(product.CategoryId);
                return View(product);
            }

            var existing = DataStore.Products.FirstOrDefault(p => p.ProductId == id);
            if (existing == null) return NotFound();

            existing.ProductName = product.ProductName;
            existing.ProductPrice = product.ProductPrice;
            existing.CategoryId = product.CategoryId;
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Delete/5
        public IActionResult Delete(int id)
        {
            var product = FindWithCategory(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = DataStore.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null) DataStore.Products.Remove(product);
            return RedirectToAction(nameof(Index));
        }

        // Fill the Category navigation property
        private static Product? FindWithCategory(int id)
        {
            var product = DataStore.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                product.Category = DataStore.Categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
            }
            return product;
        }

        // Category list for the dropdown
        private void LoadCategories(int? selectedId = null)
        {
            ViewBag.Categories = new SelectList(DataStore.Categories, "CategoryId", "CategoryName", selectedId);
        }

        // The foreign key must point to an existing Category
        private void CheckCategoryExists(int categoryId)
        {
            if (!DataStore.Categories.Any(c => c.CategoryId == categoryId))
            {
                ModelState.AddModelError(nameof(Product.CategoryId), "Please select a category");
            }
        }
    }
}
