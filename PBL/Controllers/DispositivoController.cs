using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PBL.DAO;
using PBL.Models;
using System;
using System.Collections.Generic;

namespace PBL.Controllers
{
    public class DispositivoController : PadraoController<DispositivoViewModel>
    {
        public DispositivoController()
        {
            DAO = new DispositivoDAO();
            GeraProximoId = true;
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
