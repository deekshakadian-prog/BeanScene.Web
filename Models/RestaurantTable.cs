using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BeanScene.Web.Models
{
    public partial class RestaurantTable
    {
        public int RestaurantTableId { get; set; }

        [Required]
        [Display(Name = "Area")]
        public int AreaId { get; set; }

        [Required]
        [StringLength(50)]
        public string TableName { get; set; } = string.Empty;   // B1, M2, O7

        [Range(1, 100)]
        public int? Seats { get; set; }

        [ValidateNever]
        public virtual Area? Area { get; set; }

        [ValidateNever]
        public virtual ICollection<Reservation> ReservationTables { get; set; }
            = new List<Reservation>();

        // ✅ For colouring the dots – NOT stored in the DB
        [NotMapped]
        public bool IsBooked { get; set; }
    }
}
