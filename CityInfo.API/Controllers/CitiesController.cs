
using CityInfo.API.Models;
using CityInfo.API.Repositories;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly CitiesDataStore _citiesDataStore;
        private readonly ICityRepository _citiesDB;
        private readonly ILogger<CitiesController> _logger;
        public CitiesController(CitiesDataStore citiesDataStore, ICityRepository citiesDB, ILogger<CitiesController> logger) 
        {
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
            _citiesDB = citiesDB ?? throw new ArgumentNullException(nameof(citiesDB));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(_citiesDataStore.Cities);
                
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CityDto>> GetCity(string id)
        {
            //find city
            //var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);
            _logger.LogInformation("STUART LOG: I'm going in");
            var cityToReturn = await _citiesDB.GetAsync(id);


            if (cityToReturn == null) 
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }

        [HttpDelete("delete/{cityId}")]
        public async Task<ActionResult> DeleteCity(string cityId)
        {

            try
            {
                _logger.LogInformation("STUART LOG: Going to delete");
                var response = await _citiesDB.DeleteAsync(cityId);
                return Ok(response);
            }
            catch (Exception ex) 
            {
                _logger.LogInformation("STUART LOG: Failed deletion");
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<CityDto>> CreateCity(CityDto city)
        {
            try
            {
                var cityToCreate = await _citiesDB.CreateAsync(city);

                return Ok(cityToCreate);
            }
            catch
            {
                return BadRequest();
            }
            

        }

        [HttpPut("{cityId}")]
        public async Task<ActionResult> UpdateCity(string cityId, CityDto newCity)
        {
            var city = GetCity(cityId);
            if (city == null)
            {
                return NotFound();
            }
            else if(cityId != newCity.Id) 
            {
                return BadRequest();
            }

            try
            {
                await _citiesDB.UpdateAsync(newCity);
            }
            catch
            {
                return BadRequest();
            }

            return NoContent();
        }


    }
}
