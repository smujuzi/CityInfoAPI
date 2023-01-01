using CityInfo.API.Entites;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Citites { get; set; } = null;

        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null;

        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
        {

        }
    }
}
