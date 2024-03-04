using System.ComponentModel.DataAnnotations;

namespace DemoAPI {
    /// <summary>
    /// Ejemplo de modelo
    /// </summary>
    public class WeatherForecast {
        /// <summary>
        /// Fecha
        /// </summary>
        [Required]
        public DateOnly Date { get; set; }
        /// <summary>
        /// Temperatura en grados centígrados
        /// </summary>

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [MaxLength]
        public string? Summary { get; set; }
    }
}
