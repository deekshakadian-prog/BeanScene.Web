namespace BeanScene.Web.ViewModels
{ 
  public class ReservationListItemViewModel
{
    public int Id { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string SittingName { get; set; } = string.Empty; // Breakfast / Lunch / Dinner / Event
    public DateTime ReservationStart { get; set; }
    public int DurationMinutes { get; set; }
    public int NumberOfGuests { get; set; }

    public string Area { get; set; } = string.Empty;  // Main / Outside / Balcony
    public string Tables { get; set; } = string.Empty; // e.g. "M3, M4"

    public string Status { get; set; } = string.Empty; // Pending / Confirmed / Seated / Completed / Cancelled
    public string Source { get; set; } = string.Empty; // Online / Mobile / Phone / Email / In-person
    public string Notes { get; set; } = string.Empty;
}

public class ReservationDashboardViewModel
{
    public DateTime Date { get; set; }

    public int TotalReservations { get; set; }
    public int PendingReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int SeatedReservations { get; set; }
    public int CompletedReservations { get; set; }
    public int CancelledReservations { get; set; }

    public int TotalCapacity { get; set; }        // Sum of capacity for sittings today
    public int GuestsBooked { get; set; }         // Sum of NumberOfGuests for active reservations
    public int RemainingCapacity => TotalCapacity - GuestsBooked;

    public List<ReservationListItemViewModel> Reservations { get; set; } = new();
}
}
