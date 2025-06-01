using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PBL.DAO;
using PBL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PBL.Controllers
{
    public class DispositivoController : PadraoController<DispositivoViewModel>
    {
        public DispositivoController()
        {
            DAO = new DispositivoDAO();
            GeraProximoId = true;
        }
        public virtual IActionResult Properties(int id)
        {
            try
            {
                DispositivoViewModel model = DAO.Consulta(id);
                ViewBag.Operacao = "P";
                return View("Status", model);
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public override IActionResult Save(DispositivoViewModel model, string Operacao)
        {
            try
            {
                ValidaDados(model, Operacao);
                if (ModelState.IsValid == false)
                {
                    ViewBag.Operacao = Operacao;
                    PreencheDadosParaView(Operacao, model);
                    return View(NomeViewForm, model);
                }
                else
                {
                    if (Operacao == "I")
                    {
                        ClienteFiware.CriarDispositivo(model).GetAwaiter().GetResult();
                        DAO.Insert(model);
                    }
                    else
                    {
                        ClienteFiware.EditarDispositivo(model).GetAwaiter().GetResult();
                        DAO.Update(model);
                    }
                    return RedirectToAction(NomeViewIndex);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public override IActionResult Delete(int id)
        {
            try
            {
                var model = DAO.Consulta(id);
                ClienteFiware.ExcluirDispositivo(model).GetAwaiter().GetResult();
                DAO.Delete(id);
                return RedirectToAction(NomeViewIndex);
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

        public IActionResult ReceberDadosOrion(int id, int lastN)
        {
            try
            {
                DispositivoViewModel model = DAO.Consulta(id);
                string json = ClienteFiware.ObterHistorico(model, lastN).Result;
                dynamic obj = JsonConvert.DeserializeObject(json);

                List<object> lista = new List<object>();
                foreach (var value in obj.contextResponses[0].contextElement.attributes[0].values)
                {
                    lista.Add(new
                    {
                        data = (string)value.recvTime,
                        voltagem = ConverterAnalogReadParaVolt((int)value.attrValue),
                        temperatura = CalcularTemperatura(ConverterAnalogReadParaVolt((int)value.attrValue))
                    });
                }

                return Json(lista);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        // [MATHEUS] ToDo: Consulta direto no fiware; Lista os dispositivos na nuvem; Obtém última leitura de cada um
        // Incluir valor e data na viewmodel; Filtrar por nome da entidade, valor medido e intervalo de tempo; 
        // Mandar para a view de consulta.
        // Única possibilidade que eu imagino para esta exigência:
        // Duas telas de consulta que permitam filtros para exibição de dados. Os dados deverão ser consultados nas 
        // APIs: https://github.com/fabiocabrini/fiware. A tela deve possuir ao menos 3 filtros, como por exemplo 
        // Intervalo de data, descrição, etc.

        public IActionResult ConsultaFiware()
        {
            try
            {
                return View("ConsultaFiware");
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.Message));
            }
        }

        public async Task<IActionResult> ObterDadosConsultaAsync(string nome, int? valorMenor, int? valorMaior, DateTime? dataInicial, DateTime? dataFinal)
        {
            try
            {
                List<DispositivoViewModel> dispositivos = await ListarDispositivosFiwareAsync();
                List<DispositivoViewModel> dispositvosFiltrados = new List<DispositivoViewModel>();

                var dispositivosFiltrados = dispositivos.Where(dispositivo =>
                    (string.IsNullOrEmpty(nome) || (dispositivo.Apelido != null && dispositivo.Apelido.Contains(nome, StringComparison.OrdinalIgnoreCase))) &&
                    (!valorMenor.HasValue || (dispositivo.ValorUltimaMedicao.HasValue && dispositivo.ValorUltimaMedicao >= valorMenor)) &&
                    (!valorMaior.HasValue || (dispositivo.ValorUltimaMedicao.HasValue && dispositivo.ValorUltimaMedicao <= valorMaior)) &&
                    (!dataInicial.HasValue || (dispositivo.DataUltimaMedicao.HasValue && dispositivo.DataUltimaMedicao >= dataInicial)) &&
                    (!dataFinal.HasValue || (dispositivo.DataUltimaMedicao.HasValue && dispositivo.DataUltimaMedicao <= dataFinal))
                ).ToList();

                return PartialView("pvGridDispositivos", dispositivosFiltrados);
            }
            catch (Exception erro)
            {
                return Json(new { erro = true, msg = erro.Message });
            }
        }

        private async Task<List<DispositivoViewModel>> ListarDispositivosFiwareAsync()
        {
            List<DispositivoViewModel> dispositivos = new List<DispositivoViewModel>();
            string json = await ClienteFiware.GetDispositivos();
            dynamic obj = JsonConvert.DeserializeObject(json);

            foreach (var device in obj.devices)
            {
                DispositivoViewModel model = new DispositivoViewModel
                {
                    Apelido = device.entity_type,
                    DeviceId = device.device_id,
                    EntityName = device.entity_type
                };

                string jsonMedicao = await ClienteFiware.GetResultadoMedição(model);
                dynamic medicao = JsonConvert.DeserializeObject(jsonMedicao);
                if (medicao.error == null)
                {
                    try
                    {
                        model.ValorUltimaMedicao = CalcularTemperatura(ConverterAnalogReadParaVolt((int)medicao.value));
                        model.DataUltimaMedicao = DateTime.Parse((string)medicao.metadata.TimeInstant.value, null, System.Globalization.DateTimeStyles.RoundtripKind);
                    }
                    catch (Exception)
                    {
                        model.ValorUltimaMedicao = null;
                        model.DataUltimaMedicao = null;
                    }
                }
                else
                {
                    model.ValorUltimaMedicao = null;
                    model.DataUltimaMedicao = null;
                }

                dispositivos.Add(model);
            }

            return dispositivos;
        }

        private double ConverterAnalogReadParaVolt(int analogRead)
        {
            // Considera-se que se mede a tensão dividida entre três resistências iguais (portanto igual a 1/3 da original)
            return (analogRead * 3.3 / 4095) * 3;
        }

        private double CalcularTemperatura(double voltagem)
        {
            // usar aqui a função da regressão linear descoberta experimentalmente
            return ((voltagem + 3.891375) / 0.203519);
        }

    }
}
