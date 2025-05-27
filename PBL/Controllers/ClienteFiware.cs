using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PBL.Controllers
{
    public static class ClienteFiware
    {
        public static async Task<string> Get(HttpRequestMessage request)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Erro na requisição (Get). Code: {response.StatusCode}");
                }
            }
        }
    }
}