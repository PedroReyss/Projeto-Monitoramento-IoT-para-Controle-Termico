using PBL.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace PBL.DAO
{
    public class DispositivoDAO : PadraoDAO<DispositivoViewModel>
    {
        protected override SqlParameter[] CriaParametros(DispositivoViewModel model)
        {
            SqlParameter[] parametros =
            {
                new SqlParameter("id", model.Id),
                new SqlParameter("apelido", model.Apelido),
                new SqlParameter("device_id", model.DeviceId),
                new SqlParameter("entity_name", model.EntityName)
            };
            return parametros;
        }

        protected override DispositivoViewModel MontaModel(DataRow registro)
        {
            DispositivoViewModel d = new DispositivoViewModel()
            {
                Id = Convert.ToInt32(registro["id"]),
                Apelido = registro["apelido"].ToString(),
                DeviceId = registro["device_id"].ToString(),
                EntityName = registro["entity_name"].ToString()
            };
            return d;
        }

        protected override void SetTabela()
        {
            Tabela = "Dispositivos";
        }
    }
}
