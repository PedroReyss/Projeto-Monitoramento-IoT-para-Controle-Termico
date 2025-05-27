using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PBL.Models;

namespace PBL.Controllers
{
    public static class ClienteFiware
    {
        private const string IP_FIWARE = "";
        public static async Task CriarDispositivoFiware(DispositivoViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://" + IP_FIWARE + ":4041/iot/devices");
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
            var content = new StringContent("{\"devices\": [{" +
                                            $"    \"device_id\": \"{model.DeviceId}\"," +
                                            $"    \"entity_name\": \"urn:ngsi-ld:{model.EntityName}:{model.DeviceId}\"," +
                                            $"    \"entity_type\": \"{model.EntityName}\"," +
                                            "    \"protocol\": \"PDI-IoTA-UltraLight\"," +
                                            "    \"transport\": \"MQTT\"," +
                                            "    \"commands\": [" +
                                            "        { \"name\": \"on\", \"type\": \"command\" }," +
                                            "        { \"name\": \"off\", \"type\": \"command\" }" +
                                            "      ]," +
                                            "    \"attributes\": [" +
                                            "        { \"object_id\": \"s\", \"name\": \"state\", \"type\": \"Text\" }, " +
                                            "        { \"object_id\": \"v\", \"name\": \"voltage\", \"type\": \"Float\"}," +
                                            "      ]" +
                                            "    }" +
                                            "  ]" +
                                            "}", null, "application/json");
            request.Content = content;
            await HelperControllers.EnviarRequisição(request);
        }

        public static async Task<string> GetResultadoMedição(DispositivoViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{IP_FIWARE}:1026/v2/entities/urn:ngsi-ld:{model.EntityName}:{model.DeviceId}/attrs/voltage");
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
            request.Headers.Add("accept", "application/json");

            return await HelperControllers.EnviarRequisição(request);
        }

        public static async Task ExcluirDispositivo(DispositivoViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"http://{IP_FIWARE}:4041/iot/devices/{model.DeviceId}");
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");

            await HelperControllers.EnviarRequisição(request);
        }

        public static async Task SubscribeOrion(DispositivoViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"http://{IP_FIWARE}:1026/v2/subscriptions");
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
            var content = new StringContent("{\"description\": \"Notify STH-Comet of all Motion Sensor count changes\"," +
                                            "\"subject\": {" +
                                            "    \"entities\": [{" +
                                            $"        \"id\": \"urn:ngsi-ld:{model.EntityName}:{model.DeviceId}\"," +
                                            $"        \"type\": \"{model.EntityName}\"" +
                                            "    }]," +
                                            "    \"condition\": { \"attrs\": [\"voltage\"] } " +
                                            "}," +
                                            "\"notification\": {" +
                                            "    \"http\": {" +
                                            $"    \"url\": \"http://{IP_FIWARE}:8666/notify\"" +
                                            "    }," +
                                            "    \"attrs\": [" +
                                            "      \"voltage\" " +
                                            "    ]," +
                                            "    \"attrsFormat\": \"legacy\" " +
                                            "}}", null, "application/json");
            request.Content = content;

            await HelperControllers.EnviarRequisição(request);
        }

        public static async Task<string> ObterHistorico(DispositivoViewModel model, int numeroDeRegistros)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{IP_FIWARE}:8666/STH/v1/contextEntities/type/{model.EntityName}/id/urn:ngsi-ld:{model.EntityName}:{model.DeviceId}/attributes/voltage?lastN={numeroDeRegistros}");
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");

            return await HelperControllers.EnviarRequisição(request);
        }
    }
}