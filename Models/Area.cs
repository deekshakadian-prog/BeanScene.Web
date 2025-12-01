using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;   // 👈 add this
                                                      // so EF ignores the pointers
namespace BeanScene.Web.Models
{
    public partial class Area
    {
        public int AreaId { get; set; }

        public string AreaName { get; set; } = null!;

        public virtual ICollection<RestaurantTable> RestaurantTables { get; set; }
            = new List<RestaurantTable>();

        // 🔁 Doubly linked list pointers (NOT stored in database)
        [NotMapped]
        public Area? PreviousArea { get; set; }

        [NotMapped]
        public Area? NextArea { get; set; }
    }
}
