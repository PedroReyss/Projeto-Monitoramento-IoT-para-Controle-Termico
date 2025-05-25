using Microsoft.AspNetCore.Mvc.Rendering;
using PBL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PBL.DAO
{
    public class FuncionarioDAO : PadraoDAO<FuncionarioViewModel>
    {
        protected override SqlParameter[] CriaParametros(FuncionarioViewModel model)
        {
            object imgByte = model.FotoEmByte;
            if (imgByte == null)
                imgByte = DBNull.Value;
            SqlParameter[] parametros =
            {
                new SqlParameter("id", model.Id),
                new SqlParameter("nome", model.Nome),
                new SqlParameter("idade", model.Idade),
                new SqlParameter("cargo", model.Cargo),
                new SqlParameter("foto", imgByte)
            };
            return parametros;
        }

        protected override FuncionarioViewModel MontaModel(DataRow registro)
        {
            FuncionarioViewModel f = new FuncionarioViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                Nome = registro["nome"].ToString(),
                Idade = Convert.ToInt32(registro["idade"]),
                Cargo = registro["cargo"].ToString(),
            };
            if (registro["foto"] != DBNull.Value)
                f.FotoEmByte = registro["foto"] as byte[];

            return f;
        }

        protected override void SetTabela()
        {
            Tabela = "Funcionarios";
        }
    }
}
