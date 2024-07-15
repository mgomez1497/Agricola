namespace AgricolaWeb.Models
{
    public class Lotes
    {
        public int Id { get; set; }

        public int? IdFinca { get; set; }

        public string Nombre { get; set; } = null!;

        public int Arboles { get; set; }

        public string Etapa { get; set; } = null!;
    }
}
