using System;
using System.Linq;
using System.Threading.Tasks;
using BeanScene.Web.Data;
using BeanScene.Web.Models;
using BeanScene.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BeanScene.Web.Controllers

{
    [Authorize(Roles = "Admin")]
    public class Dashboards : Controller
    {
        private readonly BeanSceneContext _context;

        public Dashboards(BeanSceneContext context)
        {
            _context = context;
        }

        // GET: /Dashboards/Index?date=2025-11-28 (date is optional)
        public async Task<IActionResult> Index(DateTime? date)
        {
            // If no date given, use today
            var targetDate = (date ?? DateTime.Today).Date;

            // 1) Load real reservations for that date
            var reservationsForDay = await _context.Reservations
                .Include(r => r.Sitting)             // SittingSchedule nav (Stype, Scapacity, etc.)
                .Include(r => r.RestaurantTables)    // tables for this reservation
                    .ThenInclude(t => t.Area)        // area for each table
                .Where(r => r.StartTime.Date == targetDate)
                .ToListAsync();

            // 2) Map to ReservationListItemViewModel (for the list on the dashboard)
            var reservationItems = reservationsForDay
                .Select(r => new ReservationListItemViewModel
                {
                    Id = r.ReservationId,
                    GuestName = $"{r.FirstName} {r.LastName}",
                    Phone = r.Phone,
                    Email = r.Email,
                    SittingName = r.Sitting != null ? r.Sitting.Stype : "",
                    ReservationStart = r.StartTime,
                    DurationMinutes = r.Duration,
                    NumberOfGuests = r.NumOfGuests,
                    Area = string.Join(", ",
                        r.RestaurantTables
                            .Where(t => t.Area != null)
                            .Select(t => t.Area!.AreaName)
                            .Distinct()
                    ),
                    Tables = string.Join(", ",
                        r.RestaurantTables.Select(t => t.TableName)
                    ),
                    Status = r.Status,
                    Source = r.ReservationSource,
                    Notes = r.Notes
                })
                .ToList();

            // 3) Work out total capacity for the day (sum of Scapacity for sittings used that day)
            int totalCapacity = 0;

            var sittingsForDay = reservationsForDay
                .Where(r => r.Sitting != null)
                .Select(r => r.Sitting!)
                .Distinct();

            if (sittingsForDay.Any())
            {
                totalCapacity = sittingsForDay.Sum(s => s.Scapacity);
            }
            else
            {
                // Fallback if no sittings found – you can adjust this
                totalCapacity = 40;
            }

            // 4) Build dashboard summary
            var model = new ReservationDashboardViewModel
            {
                Date = targetDate,

                TotalReservations = reservationItems.Count,
                PendingReservations = reservationItems.Count(r => r.Status == "Pending"),
                ConfirmedReservations = reservationItems.Count(r => r.Status == "Confirmed"),
                SeatedReservations = reservationItems.Count(r => r.Status == "Seated"),
                CompletedReservations = reservationItems.Count(r => r.Status == "Completed"),
                CancelledReservations = reservationItems.Count(r => r.Status == "Cancelled"),

                TotalCapacity = totalCapacity,
                GuestsBooked = reservationItems.Sum(r => r.NumberOfGuests),

                Reservations = reservationItems
            };

            return View(model);
        }
    }
}

