namespace Chatbot.Services
{
    public class ValidacionService
    {
        public ValidacionService() { }

        public bool IsValidCedulaRuc(string cedulaRuc)
        {
            if (string.IsNullOrWhiteSpace(cedulaRuc) || (cedulaRuc.Length != 10 && cedulaRuc.Length != 13))
            {
                return false;
            }

            // Validación de Cédula
            if (cedulaRuc.Length == 10)
            {
                return IsValidCedula(cedulaRuc);
            }

            // Validación de RUC
            if (cedulaRuc.Length == 13)
            {
                // Los primeros 10 dígitos deben ser una cédula válida y los últimos 3 deben ser "001"
                return IsValidCedula(cedulaRuc.Substring(0, 10)) && cedulaRuc.Substring(10, 3) == "001";
            }

            return false;
        }

        private bool IsValidCedula(string cedula)
        {
            // La cédula debe tener exactamente 10 caracteres
            if (cedula.Length != 10)
            {
                return false;
            }

            // Los dos primeros dígitos representan la provincia (01-24)
            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24)
            {
                return false;
            }

            // El tercer dígito debe ser menor a 6 (Ecuador usa el dígito 6 para extranjeros y 9 para entidades jurídicas)
            int tercerDigito = int.Parse(cedula.Substring(2, 1));
            if (tercerDigito >= 6)
            {
                return false;
            }

            // Algoritmo de validación basado en el dígito verificador
            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;

            for (int i = 0; i < 9; i++)
            {
                int digito = int.Parse(cedula.Substring(i, 1));
                int multiplicacion = digito * coeficientes[i];
                if (multiplicacion >= 10)
                {
                    multiplicacion -= 9;
                }
                suma += multiplicacion;
            }

            int ultimoDigito = int.Parse(cedula.Substring(9, 1));
            int decenaSuperior = (int)Math.Ceiling(suma / 10.0) * 10;
            int digitoVerificador = decenaSuperior - suma;

            // Si el dígito verificador es 10, se cambia a 0
            if (digitoVerificador == 10)
            {
                digitoVerificador = 0;
            }

            // El décimo dígito debe ser igual al dígito verificador
            return digitoVerificador == ultimoDigito;
        }
    }
}
