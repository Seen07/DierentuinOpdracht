using DierentuinOpdracht.Services.Actions;
using Microsoft.AspNetCore.Mvc;

namespace DierentuinOpdracht.Controllers.Api
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly AnimalActionService _animalActionService;

        public AnimalsController(AnimalActionService animalActionService)
        {
            _animalActionService = animalActionService;
        }

        [HttpGet("{id:int}/sunrise")]
        public async Task<IActionResult> Sunrise(int id)
        {
            try
            {
                var result = await _animalActionService.SunriseAsync(id);
                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("{id:int}/sunset")]
        public async Task<IActionResult> Sunset(int id)
        {
            try
            {
                var result = await _animalActionService.SunsetAsync(id);
                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("{id:int}/feeding-time")]
        public async Task<IActionResult> FeedingTime(int id)
        {
            try
            {
                var result = await _animalActionService.FeedingTimeAsync(id);
                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("{id:int}/check-constraints")]
        public async Task<IActionResult> CheckConstraints(int id)
        {
            try
            {
                var result = await _animalActionService.CheckConstraintsAsync(id);
                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
