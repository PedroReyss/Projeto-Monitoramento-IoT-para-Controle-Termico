using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PBL.DAO;
using PBL.Models;
using System;
using System.Collections.Generic;
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

        private double ConverterAnalogReadParaVolt(int analogRead)
        {
            // Está fornecendo a tensão em volts do analogRead do ESP, não a tensão real do kit
            // precisa ajustar a proporção baseado no divisor de tensão 
            return analogRead * 3.3 / 4095;
        }

        private double CalcularTemperatura(double voltagem)
        {
            // usar aqui a função linear descoberta experimentalmente
            return (voltagem + 3.891375) / 0.203519;
        }

    }
}
