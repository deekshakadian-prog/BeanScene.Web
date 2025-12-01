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
    // 👮 Only Admin + Staff can access this controller
    [Authorize(Roles = "Admin,Staff")]
    public class AdminReservationsController : Controller
    {
        private readonly BeanSceneContext _context;

        public AdminReservationsController(BeanSceneContext context)
        {
            _context = context;
        }

        // =========================
        // LIST & DETAILS
        // =========================

        // GET: AdminReservations
        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Sitting)            // SittingSchedule nav
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();

            return View(reservations);
        }

        // GET: AdminReservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.Sitting)
                .FirstOrDefaultAsync(m => m.ReservationId == id);

            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        // =========================
        // CREATE  (Admin + Staff)
        // =========================

        // GET: AdminReservations/Create
        public IActionResult Create()
        {
            // Show Breakfast / Lunch / Dinner in dropdown
            ViewData["SittingId"] = new SelectList(
                _context.SittingSchedules
                    .OrderBy(s => s.StartDateTime),
                "SittingScheduleId",   // value
                "Stype"                // text shown in dropdown
            );

            return View();
        }

        // POST: AdminReservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ReservationId,SittingId,FirstName,LastName,Email,Phone,StartTime,Duration,NumOfGuests,ReservationSource,Notes,Status,CreatedAt,TableNumber")]
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

            // Default to Pending if Status is empty
            if (string.IsNullOrWhiteSpace(reservation.Status))
                reservation.Status = "Pending";

            reservation.CreatedAt = DateTime.Now;

            _context.Add(reservation);
            await _context.SaveChangesAsync();

            // Admin/Staff always go back to the admin list
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT  (Admin + Staff)
        // =========================

        // GET: AdminReservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            ViewData["SittingId"] = new SelectList(
                _context.SittingSchedules
                    .OrderBy(s => s.StartDateTime),
                "SittingScheduleId",
                "Stype",
                reservation.SittingId
            );

            return View(reservation);
        }

        // POST: AdminReservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ReservationId,SittingId,FirstName,LastName,Email,Phone,StartTime,Duration,NumOfGuests,ReservationSource,Notes,Status,CreatedAt,TableNumber")]
            Reservation reservation)
        {
            if (id != reservation.ReservationId)
                return NotFound();

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

            try
            {
                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservation.ReservationId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE  (Admin + Staff)
        // =========================

        // GET: AdminReservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.Sitting)
                .FirstOrDefaultAsync(m => m.ReservationId == id);

            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        // POST: AdminReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
