using EncriptationDesc;
using Microsoft.Extensions.Options;
using ServiciosExternos.Models;

namespace ServiciosExternos.Controllers
{
    public class EncriptDecript
    {

        private readonly GlobalConfigurations _mySettings;

        public EncriptDecript(IOptions<GlobalConfigurations> mySettings)
        {
            _mySettings = mySettings.Value;
        }

        public string decrypData(EncriptedData _encriptedData)
        {
            var path = _mySettings.EncryptDecryptPath;
            Asimetrico asimetrico = new Asimetrico
            {
                PrivatKey = _mySettings.EncryptDecryptPath
            };

            string llaveDesencriptada = "LOCAL_SECRECT_PROVIDA_SEED";

            Simetrica simetrica = new Simetrica
            {
                Keystring = llaveDesencriptada,
            };

            string newJsonString = simetrica.Decrypt(_encriptedData.data);
            return newJsonString;
        }

        public EncriptedData encryptData(String data)
        {
            //BaseResponse respuestaError = new BaseResponse();
            //respuestaError.SF_ExisteError = true;
            //respuestaError.SF_Error = "Ha ocurrido un error, por favor intente más tarde. Código de error: 0x01";
            try
            {
                string responseBody = System.Text.Json.JsonSerializer.Serialize(data);
                var path = _mySettings.EncryptDecryptPath;

                Simetrica simetrica = new Simetrica
                {
                    Keystring = "LOCAL_SECRECT_PROVIDA_SEED",
                };
                var dataCiphered = simetrica.Encrypt(responseBody);

                Asimetrico asimetrico = new Asimetrico
                {
                    PublicKey = path,
                };

                var newJson = new EncriptedData
                {
                    data = dataCiphered,
                    key = ""
                };

                return newJson;
            } 
            catch (Exception e)
            {
                throw e;
            }

            
        }

    }
}
