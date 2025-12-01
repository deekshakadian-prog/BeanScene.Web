using System;
using System.Linq;
using System.Threading.Tasks;
using BeanScene.Web.Data;
using BeanScene.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BeanScene.Web.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberReservationsController : Controller
    {
        private readonly BeanSceneContext _context;

        public MemberReservationsController(BeanSceneContext context)
        {
            _context = context;
        }

        // GET: /MemberReservations/Book
        public IActionResult Book()
        {
            ViewData["SittingId"] = new SelectList(
                _context.SittingSchedules
                    .OrderBy(s => s.StartDateTime),
                "SittingScheduleId",
                "Stype"            // Breakfast / Lunch / Dinner
            );

            return View();
        }

        // POST: /MemberReservations/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(
            [Bind("ReservationId,SittingId,FirstName,LastName,Email,Phone,StartTime,Duration,NumOfGuests,ReservationSource,Notes,TableNumber")]
            Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                ViewData["SittingId"] = new SelectList(
                    _context.SittingSchedules
                        .OrderBy(s => s.StartDateTime),
                    "SittingScheduleId",
                    "Stype",
                    reservation.SittingId
                );
                return View(reservation);
            }

            // Force email to logged-in member
            var loginEmail = User.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(loginEmail))
            {
                reservation.Email = loginEmail;
            }

            reservation.Status = "Pending";          // members always start as Pending
            reservation.CreatedAt = DateTime.Now;

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyReservations));
        }

        // GET: /MemberReservations/MyReservations
        public async Task<IActionResult> MyReservations()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
                return Challenge();

            var reservations = await _context.Reservations
                .Include(r => r.Sitting)
                .Where(r => r.Email == email)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();

            return View(reservations);
        }
    }
}
