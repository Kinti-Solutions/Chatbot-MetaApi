namespace Chatbot.Models
{
    public class Asesor
    {
        public int Id { get; set; }

        public string Usuario { get; set; } = null!;

        public string Celular { get; set; } = null!;

        public bool Activo { get; set; }

        public string NombreUsuario { get; set; } = null!;
    }
}
