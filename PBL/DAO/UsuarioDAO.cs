using PBL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace PBL.DAO
{
    public class UsuarioDAO : PadraoDAO<UsuarioViewModel>
    {
        protected override SqlParameter[] CriaParametros(UsuarioViewModel model)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("id", model.Id),
                new SqlParameter("id_pessoa", model.IdPessoa),
                new SqlParameter("tipo", model.Tipo),
                new SqlParameter("username", model.Username),
                new SqlParameter("senha", model.Senha),
            };
            return parametros;
        }

        protected override UsuarioViewModel MontaModel(DataRow registro)
        {
            UsuarioViewModel u = new UsuarioViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                IdPessoa = Convert.ToInt32(registro["id_pessoa"]),
                Tipo = Convert.ToInt32(registro["tipo"]),
                Username = registro["username"].ToString(),
                Senha = registro["senha"].ToString()
            };

            if (registro.Table.Columns.Contains("nome"))
                u.NomeFuncionario = registro["nome"].ToString();

            return u;
        }

        protected override void SetTabela()
        {
            Tabela = "Usuarios";
        }

        public override List<UsuarioViewModel> Listagem()
        {
            var tabela = HelperDAO.ExecutaProcSelect("spListagem_Usuario", new SqlParameter[0]);
            List<UsuarioViewModel> lista = new List<UsuarioViewModel>();
            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModel(registro));

            return lista;
        }
    }
}
