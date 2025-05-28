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
        private static readonly string IP_FIWARE = Environment.GetEnvironmentVariable("FIWARE_IP") ?? "127.0.0.1";

        public static async Task CriarDispositivoFiware(DispositivoViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://" + IP_FIWARE + ":4041/iot/devices");
            AddHeaders(request);
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
            AddHeaders(request);
            request.Headers.Add("accept", "application/json");

            return await HelperControllers.EnviarRequisição(request);
        }

        public static async Task ExcluirDispositivo(DispositivoViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"http://{IP_FIWARE}:4041/iot/devices/{model.DeviceId}");
            AddHeaders(request);

            await HelperControllers.EnviarRequisição(request);
        }

        public static async Task SubscribeOrion(DispositivoViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"http://{IP_FIWARE}:1026/v2/subscriptions");
            AddHeaders(request);
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
            AddHeaders(request);

            return await HelperControllers.EnviarRequisição(request);
        }

        public static async Task EditarDispositivo(DispositivoViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"http://{IP_FIWARE}:4041/iot/devices/{model.DeviceId}");
            AddHeaders(request);
            var edicao = new
            {
                entity_name = $"urn:ngsi-ld:{model.EntityName}:{model.DeviceId}"
            };
            var json = JsonConvert.SerializeObject(edicao);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            request.Content = content;
            await HelperControllers.EnviarRequisição(request);
        }

        private static void AddHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
        }
    }
}