using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                        _ = ClienteFiware.CriarDispositivo(model);
                        DAO.Insert(model);
                    }
                    else
                    {
                        _ = ClienteFiware.EditarDispositivo(model);
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
                _ = ClienteFiware.ExcluirDispositivo(DAO.Consulta(id));
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

    }
}
