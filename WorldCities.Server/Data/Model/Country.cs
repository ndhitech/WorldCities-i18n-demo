using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace WorldCities.Server.Data.Models {
    [Table("NdtechCountry")]
    [Index(nameof(Name))]
    [Index(nameof(isO2))]
    [Index(nameof(isO3))]
    public class Country {
        #region Properties
        /// <summary>
        /// The unique id and primary key for this Country
        /// </summary>
        [Key]
        [Required]
        public int Id {
            get; set;
        }
        /// <summary>
        /// Country name (in UTF8 format)
        /// /// </summary>
        public required string Name {
            get; set;
        }
        /// <summary>
        /// Country code (in ISO 3166-1 ALPHA-2 format)
        /// </summary>
        [JsonPropertyName("isO2")]
        public required string isO2 {
            get; set;
        }
        /// <summary>
        /// Country code (in ISO 3166-1 ALPHA-3 format)
        /// </summary>
        [JsonPropertyName("isO3")]
        public required string isO3 {
            get; set;
        }
        #endregion
        #region Navigation Properties
        /// <summary>
        /// A collection of all the cities related to this country.
        /// </summary>
        public ICollection<City>? Cities {
            get; set;
        }
        #endregion
    }
}