﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;

namespace WorldCities.Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context) {
            _context = context;
            Serilog.Log.Information("CountriesController initialized.");
        }

        // GET: api/Countries
        //[HttpGet("pageIndex={pageIndex}&pageSize={pageSize}&sortColumn={sortColumn}&sortOrder={sortOrder}&filterColumn={filterColumn}&filterQuery={filterQuery}")]
        [HttpGet()]
        public async Task<ActionResult<ApiResult<CountryDTO>>> GetCountries(
                 [FromQuery] int pageIndex = 0 ,
                 [FromQuery] int pageSize = 10 ,
                 [FromQuery] string? sortColumn = null ,
                 [FromQuery] string? sortOrder = null ,
                 [FromQuery] string? filterColumn = "" ,
                 [FromQuery] string? filterQuery = "") {
            return await ApiResult<CountryDTO>.CreateAsync(
            _context.Countries.AsNoTracking()
                .Select(c => new CountryDTO() {
                    Id = c.Id ,
                    Name = c.Name ,
                    isO2 = c.isO2 ,
                    isO3 = c.isO3 ,
                    TotCities = c.Cities!.Count
                }) ,
                    pageIndex ,
                    pageSize ,
                    sortColumn ,
                    sortOrder ,
                    filterColumn ,
                    filterQuery
                );
        }
        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id) {
            var country = await _context.Countries.FindAsync(id);

            if (country == null) {
                return NotFound();
            }

            return country;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id , Country country) {
            if (id != country.Id) {
                return BadRequest();
            }

            _context.Entry(country).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!CountryExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country) {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry" , new {
                id = country.Id
            } , country);
        }

        // DELETE: api/Countries/5
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id) {
            var country = await _context.Countries.FindAsync(id);
            if (country == null) {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id) {
            return _context.Countries.Any(e => e.Id == id);
        }
        [HttpPost]
        [Route("IsDupeField")]
        public bool IsDupeField(
                     int countryId ,
                     string fieldName ,
                     string fieldValue) {
            switch (fieldName) {
                case "name":
                    return _context.Countries.Any(
                    c => c.Name == fieldValue && c.Id != countryId);
                case "isO2":
                    return _context.Countries.Any(
                    c => c.isO2 == fieldValue && c.Id != countryId);
                case "isO3":
                    return _context.Countries.Any(
                    c => c.isO3 == fieldValue && c.Id != countryId);
                default:
                    return false;
            }
        }
    }
}
