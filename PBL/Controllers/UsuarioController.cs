using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PBL.DAO;
using PBL.Models;
using System;
using System.Collections.Generic;

namespace PBL.Controllers
{
    public class UsuarioController : PadraoController<UsuarioViewModel>
    {
        public UsuarioController()
        {
            DAO = new UsuarioDAO();
            GeraProximoId = true;
            ExigeAutenticacao = false;
        }

        public override IActionResult Index()
        {
            if (HelperControllers.UserEstaLogado(HttpContext.Session))
                return base.Index();
            else
                return View("Login");
        }

        public IActionResult Login()
        {
            return View("Login");
        }

        protected override void ValidaDados(UsuarioViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            if (model.IdPessoa == 0)
                ModelState.AddModelError("IdPessoa", "Selecione um funcionário");
            if (model.Tipo == 0)
                ModelState.AddModelError("Tipo", "Selecione um tipo para o usuário");
            if (string.IsNullOrEmpty(model.Username))
                ModelState.AddModelError("Username", "Digite um username");
            if (((UsuarioDAO)DAO).ConsultaPorUsername(model.Username) != null)
                ModelState.AddModelError("Username", "Username já está em uso");
            if (string.IsNullOrEmpty(model.Senha))
                ModelState.AddModelError("Senha", "Digite uma senha");
        }

        public override IActionResult Create()
        {
            try
            {
                if (HelperControllers.UserEstaLogado(HttpContext.Session))
                {
                    ViewBag.Operacao = "I";
                    UsuarioViewModel model = new UsuarioViewModel();
                    PreencheDadosParaView("I", model);
                    PreparaListaFuncionariosParaCombo();
                    return View(NomeViewForm, model);
                }
                else
                {
                    return View("Login");
                }

            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult Cadastro() {
            UsuarioViewModel model = new UsuarioViewModel();
            return View("Form", model);
        }
        
        public override IActionResult Edit(int id)
        {
            try
            {
                if (HelperControllers.UserEstaLogado(HttpContext.Session))
                {
                    ViewBag.Operacao = "A";
                    var model = DAO.Consulta(id);
                    PreparaListaFuncionariosParaCombo();
                    if (model == null)
                        return RedirectToAction(NomeViewIndex);
                    else
                    {
                        PreencheDadosParaView("A", model);
                        return View(NomeViewForm, model);
                    }
                }
                else
                {
                    return View("Login");
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public override IActionResult Save(UsuarioViewModel model, string Operacao)
        {
            try
            {
                ValidaDados(model, Operacao);
                if (ModelState.IsValid == false)
                {
                    ViewBag.Operacao = Operacao;
                    PreencheDadosParaView(Operacao, model);
                    PreparaListaFuncionariosParaCombo();
                    return View(NomeViewForm, model);
                }
                else
                {
                    if (Operacao == "I")
                        DAO.Insert(model);
                    else
                        DAO.Update(model);
                    return RedirectToAction(NomeViewIndex);
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult FazerLogin(string usuario, string senha)
        {
            try
            {
                UsuarioViewModel user = ((UsuarioDAO)DAO).ConsultaPorUsername(usuario);

                if (user != null && user.Senha == senha)
                {
                    HttpContext.Session.SetString("Logado", "true");
                    HttpContext.Session.SetString("User", user.Username);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Erro = "Usuário ou senha inválidos!";
                    return View("Login");
                }
            }
            catch (Exception erro)
            {
                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult LogOff()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        private void PreparaListaFuncionariosParaCombo()
        {
            FuncionarioDAO FuncionarioDao = new FuncionarioDAO();
            var Funcionarios = FuncionarioDao.Listagem();
            List<SelectListItem> listaFuncionarios = new List<SelectListItem>();

            listaFuncionarios.Add(new SelectListItem("Selecione um Funcionário...", "0"));
            foreach (var Funcionario in Funcionarios)
            {
                SelectListItem item = new SelectListItem(Funcionario.Nome, Funcionario.Id.ToString());
                listaFuncionarios.Add(item);
            }
            ViewBag.Funcionarios = listaFuncionarios;
        }
    }
}
