namespace thanhthao.Models
{
    // Temporary in-memory data store (no database yet).
    // Data is reset when the application restarts.
    public static class DataStore
    {
        public static List<Category> Categories { get; } = Enumerable.Range(1, 6)
            .Select(i => new Category
            {
                CategoryId = i,
                CategoryName = $"Category {i}",
                Description = $"Description {i}"
            })
            .ToList();

        public static List<Product> Products { get; } = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Product 1", ProductPrice = 100, CategoryId = 1 },
            new Product { ProductId = 2, ProductName = "Product 2", ProductPrice = 200, CategoryId = 1 },
            new Product { ProductId = 3, ProductName = "Product 3", ProductPrice = 300, CategoryId = 2 },
            new Product { ProductId = 4, ProductName = "Product 4", ProductPrice = 400, CategoryId = 3 },
            new Product { ProductId = 5, ProductName = "Product 5", ProductPrice = 500, CategoryId = 4 },
            new Product { ProductId = 6, ProductName = "Product 6", ProductPrice = 600, CategoryId = 5 }
        };

        public static int NextCategoryId() => Categories.Count == 0 ? 1 : Categories.Max(c => c.CategoryId) + 1;

        public static int NextProductId() => Products.Count == 0 ? 1 : Products.Max(p => p.ProductId) + 1;
    }
}
