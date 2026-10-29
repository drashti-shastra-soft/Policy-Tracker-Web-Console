using System.Collections.Generic;

namespace PolicyTrackerWebClient.Domain
{
    public class CategoryLister
    {
        public List<Category> List { get; set; } = new List<Category>();

        public Category SearchCriteria { get; set; } = new Category();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
