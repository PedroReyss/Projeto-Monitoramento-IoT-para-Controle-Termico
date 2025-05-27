using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PBL.DAO;
using PBL.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PBL.Controllers
{
    public class DispositivoController : PadraoController<DispositivoViewModel>
    {
        public DispositivoController()
        {
            DAO = new DispositivoDAO();
            GeraProximoId = true;
        }
        public virtual IActionResult Properties(DispositivoViewModel model)
        {
            try
            {
                ViewBag.Operacao = "P";
                return View("Status", model);
            }
            catch (Exception erro)
            {
            return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        protected override void ValidaDados(DispositivoViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            if (string.IsNullOrEmpty(model.Apelido))
                ModelState.AddModelError("Apelido", "Digite um apelido");
            if (string.IsNullOrEmpty(model.DeviceId))
                ModelState.AddModelError("DeviceId", "Digite um ID para o Fiware");
            if (string.IsNullOrEmpty(model.EntityName))
                ModelState.AddModelError("EntityName", "Digite um nome para o Fiware");
        }

        private void CriarDispositivoFiware(DispositivoViewModel model) {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://3.90.209.194:4041/iot/devices");
            request.Headers.Add("fiware-service", "smart");
            request.Headers.Add("fiware-servicepath", "/");
            var content = new StringContent("{\"devices\": [{" +
                                            $"      \"device_id\": \"{model.DeviceId}\",         " +
                                            "      \"entity_name\": \"urn:ngsi-ld:DataLogger:001\",   " +
                                            "      \"entity_type\": \"DataLogger\",          " +
                                            "      \"protocol\": \"PDI-IoTA-UltraLight\",  " +
                                            "      \"transport\": \"MQTT\",            " +
                                            "" +
                                            "      " +
                                            "      \"commands\": [" +
                                            "        { \"name\": \"on\", \"type\": \"command\" },  " +
                                            "        { \"name\": \"off\", \"type\": \"command\" }  " +
                                            "      ]," +
                                            "" +
                                            "      " +
                                            "      \"attributes\": [" +
                                            "        { \"object_id\": \"s\", \"name\": \"state\", \"type\": \"Text\" }, " +
                                            "        { \"object_id\": \"v\", \"name\": \"voltage\", \"type\": \"Float\"}," +
                                            "        { \"object_id\": \"t\", \"name\": \"temperature\", \"type\": \"Float\"}" +
                                            "      ]" +
                                            "    }" +
                                            "  ]" +
                                            "}", null, "application/json");
            request.Content = content;
            ClienteFiware.Get(request);
        }

    }
}
