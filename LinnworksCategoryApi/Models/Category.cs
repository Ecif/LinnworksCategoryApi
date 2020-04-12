using System;

namespace LinnworksCategoryApi.Models
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductCount { get; set; }
    }
}