namespace Chatbot.Models
{
    public class PoliticaProteccionDato
    {
        public int Id { get; set; }

        public string Idpersona { get; set; } = null!;

        public DateTime? Fechaaceptacion { get; set; }

        public DateTime Fechaultimacons { get; set; }

        public bool Autorizado { get; set; }
    }
}
