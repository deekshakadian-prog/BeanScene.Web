using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeanScene.Web.Data;
using BeanScene.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeanScene.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class RestaurantTablesController : Controller
    {
        private readonly BeanSceneContext _context;

        public RestaurantTablesController(BeanSceneContext context)
        {
            _context = context;
        }

        // GET: RestaurantTables
        // Shows all tables; any table that appears in Reservations (not Cancelled)
        // will be marked as IsBooked = true
        public async Task<IActionResult> Index()
        {
            // 1. Load all tables (no ordering yet – we'll do it in memory)
            var tables = await _context.RestaurantTables
                .Include(t => t.Area)
                .ToListAsync();

            // helper to extract the numeric part of "B1", "B10", "M3", etc.
            int GetTableNumber(RestaurantTable t)
            {
                if (string.IsNullOrEmpty(t.TableName))
                    return int.MaxValue;

                var digits = new string(
                    t.TableName.SkipWhile(c => !char.IsDigit(c)).ToArray()
                );

                return int.TryParse(digits, out var n) ? n : int.MaxValue;
            }

            // sort by prefix letter (B/M/O) then by the number B1,B2,...,B10
            tables = tables
                .OrderBy(t => string.IsNullOrEmpty(t.TableName) ? 'Z' : t.TableName[0])
                .ThenBy(GetTableNumber)
                .ToList();

            // 2. Find which table *names* are booked (no time constraint)
            var reservedNames = await _context.Reservations
                .Where(r =>
                    r.Status != "Cancelled" &&
                    !string.IsNullOrEmpty(r.TableNumber))
                .Select(r => r.TableNumber!)   // e.g. "B1"
                .Distinct()
                .ToListAsync();

            var bookedSet = new HashSet<string>(reservedNames,
                                                StringComparer.OrdinalIgnoreCase);

            // 3. Flag booked tables for the view
            foreach (var t in tables)
            {
                t.IsBooked = bookedSet.Contains(t.TableName);
            }

            return View(tables);
        }

        // ... keep your existing Details/Create/Edit/Delete actions unchanged ...
    }
}
