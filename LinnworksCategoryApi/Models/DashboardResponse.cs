using System.Collections.Generic;

namespace LinnworksCategoryApi.Models
{
    public class DashboardResponse
    {
        public IEnumerable<Category> Results { get; set; }
    }
}