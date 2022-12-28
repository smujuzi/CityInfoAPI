
using CityInfo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly CitiesDataStore _citiesDataStore;
        public CitiesController(CitiesDataStore citiesDataStore) 
        {
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(_citiesDataStore.Cities);
                
        }
        /// <summary>
        /// Get  a city by id
        /// </summary>
        /// <param name="id"> The id of the city to get</param>
        /// <returns>An ActionResult</returns>
        /// <response code="200">Returns the requested city</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CityDto> GetCity(int id)
        {
            //find city
            var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);

            if(cityToReturn == null) 
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }
    }
}
