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
        public static async Task<string> Get(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
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


        public static async Task<string> Post(string url, string jsonBody)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Erro na requisição (Post). Code: {response.StatusCode}");
                }
            }
        }

        public static async Task<string> Put(string url, string jsonBody)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Erro na requisição (Put). Code: {response.StatusCode}");
                }
            }
        }

        public static async Task<string> DeleteAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Erro na requisição (Delete). Code: {response.StatusCode}");
                }
            }
        }


    }
}