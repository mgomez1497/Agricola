namespace AgricolaWeb.Models
{
    public class Finca
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
        public decimal Hectareas { get; set; }
        public string? Descripcion { get; set; }
    }
}
