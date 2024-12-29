using System.Collections;

namespace Book_Store.Models.DTOs
{
    public class DashboardDTO
    {
        public List<DashboardOrderDetailsDTO>? OrderDetails { get; set; }
        public List<DashboardBookDetailsDTO>? NewBooks { get; set; }
        public List<DashboardBookDetailsDTO>? UsedBooks { get; set; }    
    }
}
