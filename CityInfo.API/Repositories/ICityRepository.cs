using CityInfo.API.Models;

namespace CityInfo.API.Repositories
{
    public interface ICityRepository
    {

        Task<bool> CreateAsync(CityDto city);


        Task<CityDto?> GetAsync(string id);

        //public async Task<IEnumerable<CityDto>> GetAllAsync();
        Task<bool> UpdateAsync(CityDto city);

        Task<bool> DeleteAsync(string id);

    }
}