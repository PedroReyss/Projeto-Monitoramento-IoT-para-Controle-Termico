using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PBL.Enum;

namespace PBL.Controllers
{
    public static class HelperControllers
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> EnviarRequisição(HttpRequestMessage request)
        {
            var response = await httpClient.SendAsync(request);
            var conteudo = await response.Content.ReadAsStringAsync();

            return conteudo;
        }

        public static Boolean UserEstaLogado(ISession session)
        {
            string logado = session.GetString("Logado");
            if (logado == null)
                return false;
            else
                return true;
        }

        public static Boolean UserEhAdmin(ISession session)
        {
            return session.GetInt32("Tipo") == (int)TipoEnum.Administrador;
        }

        public static Boolean AdminEstaLogado(ISession session)
        {
            return UserEstaLogado(session) && UserEhAdmin(session);
        }

        
    }
}