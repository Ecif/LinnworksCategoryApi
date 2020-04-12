namespace LinnworksCategoryApi.Config
{
    public class CategoryApiConfig
    {
        public string BaseUrl { get; set; }

        public int Timeout { get; set; }

        public string GetCategories { get; set; }

        public string CreateCategory { get; set; }

        public string UpdateCategory { get; set; }
        
        public string DeleteCategoryById { get; set; }
    }
}