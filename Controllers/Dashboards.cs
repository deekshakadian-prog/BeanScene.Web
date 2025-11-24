using BeanScene.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BeanScene.Web.Controllers
{
    public class Dashboards : Controller
    {
        public IActionResult Index(DateTime? date)
        {
            var targetDate = (date ?? DateTime.Today).Date;

            // TODO: Replace this with real data from your DB (Reservations, Sittings, Tables, etc.)
            var demoReservations = new List<ReservationListItemViewModel>
            {
                new ReservationListItemViewModel
                {
                    Id = 1,
                    GuestName = "John Smith",
                    Phone = "0400 000 000",
                    Email = "john@example.com",
                    SittingName = "Dinner",
                    ReservationStart = targetDate.AddHours(18), // 6pm
                    DurationMinutes = 90,
                    NumberOfGuests = 3,
                    Area = "Main",
                    Tables = "M3",
                    Status = "Pending",
                    Source = "Phone",
                    Notes = "Table by the window"
                },
                new ReservationListItemViewModel
                {
                    Id = 2,
                    GuestName = "Sarah Lee",
                    Phone = "0400 111 111",
                    Email = "sarah@example.com",
                    SittingName = "Dinner",
                    ReservationStart = targetDate.AddHours(19), // 7pm
                    DurationMinutes = 120,
                    NumberOfGuests = 4,
                    Area = "Balcony",
                    Tables = "B1, B2",
                    Status = "Confirmed",
                    Source = "Online",
                    Notes = "Highchair required"
                }
            };

            var model = new ReservationDashboardViewModel
            {
                Date = targetDate,
                TotalReservations = demoReservations.Count,
                PendingReservations = demoReservations.Count(r => r.Status == "Pending"),
                ConfirmedReservations = demoReservations.Count(r => r.Status == "Confirmed"),
                SeatedReservations = demoReservations.Count(r => r.Status == "Seated"),
                CompletedReservations = demoReservations.Count(r => r.Status == "Completed"),
                CancelledReservations = demoReservations.Count(r => r.Status == "Cancelled"),
                TotalCapacity = 40, // e.g. dinner sitting capacity
                GuestsBooked = demoReservations.Sum(r => r.NumberOfGuests),
                Reservations = demoReservations
            };

            return View(model);
        }
    }
}
