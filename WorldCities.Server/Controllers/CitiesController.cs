﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;

namespace WorldCities.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context) {
            _context = context;
            Serilog.Log.Information("CitiesController initialized.");
        }

        // GET: api/Cities
        // GET: api/Cities/?pageIndex=0&pageSize=10
        // GET: api/Cities/?pageIndex=0&pageSize=10&sortColumn=name&
        // sortOrder=asc

        [HttpGet()]
        public async Task<ActionResult<ApiResult<CityDTO>>> GetCities(
                 [FromQuery] int pageIndex = 0 ,
                 [FromQuery] int pageSize = 10 ,
                 [FromQuery] string? sortColumn = null ,
                 [FromQuery] string? sortOrder = null ,
                 [FromQuery] string? filterColumn = "" ,
                 [FromQuery] string? filterQuery = "") {
            return await ApiResult<CityDTO>.CreateAsync(
            _context.Cities.AsNoTracking()
                 .Select(c => new CityDTO() {
                     Id = c.Id ,
                     Name = c.Name ,
                     Lat = c.Lat ,
                     Lon = c.Lon ,
                     CountryId = c.Country!.Id ,
                     CountryName = c.Country!.Name
                 }) ,
            pageIndex ,
            pageSize ,
            sortColumn ,
            sortOrder ,
            filterColumn ,
            filterQuery
        );
        }
        // ...e
        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id) {
            var city = await _context.Cities.FindAsync(id);

            if (city == null) {
                return NotFound();
            }

            return city;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id , City city) {
            if (id != city.Id) {
                return BadRequest();
            }

            _context.Entry(city).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!CityExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city) {
            if (IsDupeCity(city))
                NoContent();

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity" , new {
                id = city.Id
            } , city);
        }
        [HttpPost]
        [Route("IsDupeCity")]
        public bool IsDupeCity(City city) {
            return _context.Cities.AsNoTracking().Any(
            e => e.Name == city.Name
            && e.Lat == city.Lat
            && e.Lon == city.Lon
            && e.CountryId == city.CountryId
            && e.Id != city.Id
            );
        }

        // DELETE: api/Cities/5

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id) {
            var city = await _context.Cities.FindAsync(id);
            if (city == null) {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id) {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
