using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            var device = new
            {
                devices = new[] {
                    new {
                        device_id = model.DeviceId,
                        entity_name = $"urn:ngsi-ld:{model.EntityName}:{model.DeviceId}",
                        entity_type = model.EntityName,
                        protocol = "PDI-IoTA-UltraLight",
                        transport = "MQTT",
                        commands = new[] {
                            new { name = "on", type = "command" },
                            new { name = "off", type = "command" }
                        },
                        attributes = new[] {
                            new { object_id = "s", name = "state", type = "Text" },
                            new { object_id = "v", name = "voltage", type = "Float" }
                        }
                    }
                }
            };
            string json = JsonConvert.SerializeObject(device);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
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
            var subscription = new
            {
                description = "Notify STH-Comet of all Motion Sensor count changes",
                subject = new
                {
                    entities = new[]
                    {
                        new
                        {
                            id = $"urn:ngsi-ld:{model.EntityName}:{model.DeviceId}",
                            type = model.EntityName
                        }
                    },
                    condition = new
                    {
                        attrs = new[] { "voltage" }
                    }
                },
                notification = new
                {
                    http = new
                    {
                        url = $"http://{IP_FIWARE}:8666/notify"
                    },
                    attrs = new[] { "voltage" },
                    attrsFormat = "legacy"
                }
            };
            var json = JsonConvert.SerializeObject(subscription);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
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

        private static void AddHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
        }
    }
}