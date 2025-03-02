using System.Text.Json.Serialization;
namespace WorldCities.Server.Data {
    public class CountryDTO {
        #region Properties
        public int Id {
            get; set;
        }
        public string Name { get; set; } = null!;
        [JsonPropertyName("isO2")]
        public string isO2 { get; set; } = null!;
        [JsonPropertyName("isO3")]
        public string isO3 { get; set; } = null!;
        public int? TotCities { get; set; } = null!;
        #endregion
    }
}