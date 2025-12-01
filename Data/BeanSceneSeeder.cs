using BeanScene.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BeanScene.Web.Data
{
    public static class BeanSceneSeeder
    {
        public static async Task EnsureSeededAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<BeanSceneContext>();

            // Make sure DB is created/migrated
            await context.Database.MigrateAsync();

            // If tables already exist, don't seed again
            if (context.RestaurantTables.Any())
                return;

            // Create Areas
            var balcony = new Area { AreaName = "Balcony" };
            var main = new Area { AreaName = "Main" };
            var outside = new Area { AreaName = "Outside" };

            context.Areas.AddRange(balcony, main, outside);
            await context.SaveChangesAsync();

            // Add B1..B10, M1..M10, O1..O10
            void AddTables(Area area, char prefix)
            {
                for (int i = 1; i <= 10; i++)
                {
                    context.RestaurantTables.Add(new RestaurantTable
                    {
                        AreaId = area.AreaId,
                        TableName = $"{prefix}{i}", // e.g. "B1"
                        Seats = 4
                    });
                }
            }

            AddTables(balcony, 'B');
            AddTables(main, 'M');
            AddTables(outside, 'O');

            await context.SaveChangesAsync();
        }
    }
}

