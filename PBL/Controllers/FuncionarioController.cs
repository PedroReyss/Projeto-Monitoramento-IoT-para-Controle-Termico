using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using PBL.DAO;
using PBL.Models;
using System.Collections.Generic;
using System.IO;

namespace PBL.Controllers
{
    public class FuncionarioController : PadraoController<FuncionarioViewModel>
    {
        public FuncionarioController()
        {
            DAO = new FuncionarioDAO();
            GeraProximoId = true;
        }
        protected override void ValidaDados(FuncionarioViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            if (string.IsNullOrEmpty(model.Nome))
                ModelState.AddModelError("Nome", "Preencha o nome");
            if (model.Idade <= 0)
                ModelState.AddModelError("Idade", "Preencha a idade corretamente");
            if (string.IsNullOrEmpty(model.Cargo))
                ModelState.AddModelError("Cargo", "Preencha o cargo");
            if (model.Foto == null && operacao == "I")
                ModelState.AddModelError("Foto", "Escolha uma Foto.");
            if (model.Foto != null && model.Foto.Length / 1024 / 1024 >= 2)
                ModelState.AddModelError("Foto", "Foto limitada a 2 mb.");
            if (ModelState.IsValid)
            {
                if (operacao == "A" && model.Foto == null)
                {
                    FuncionarioViewModel funcionario = DAO.Consulta(model.Id);
                    model.FotoEmByte = funcionario.FotoEmByte;
                }
                else
                {
                    model.FotoEmByte = ConvertImageToByte(model.Foto);
                }
            }
        }

        public byte[] ConvertImageToByte(IFormFile file)
        {
            if (file != null)
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    return ms.ToArray();
                }
            else
                return null;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ExigeAutenticacao && !HelperControllers.AdminEstaLogado(HttpContext.Session))
                context.Result = RedirectToAction("Login", "Usuario");
            else
            {
                ViewBag.Logado = true;
                ViewBag.Tipo = HttpContext.Session.GetInt32("Tipo");
                base.OnActionExecuting(context);
            }
        }
    }
}
