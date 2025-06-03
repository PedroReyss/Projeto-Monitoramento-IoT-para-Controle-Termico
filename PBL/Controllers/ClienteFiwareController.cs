using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ClienteFiwareController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public ClienteFiwareController()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("fiware-service", "smart");
        _httpClient.DefaultRequestHeaders.Add("fiware-servicepath", "/");
    }

    [HttpGet]
    public async Task<IActionResult> Get(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest(new { erro = "URL da API não informada." });
        }

        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new { erro = "Falha ao acessar a API." });
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var valuesElement = json.RootElement
                                    .GetProperty("contextResponses")[0]
                                    .GetProperty("contextElement")
                                    .GetProperty("attributes")[0]
                                    .GetProperty("values");

            var resultado = new List<object>();

            foreach (var item in valuesElement.EnumerateArray())
            {
                resultado.Add(new
                {
                    timestamp = item.GetProperty("timestamp").GetString(),
                    valor = item.GetProperty("valor").GetString()
                });
            }

            return Ok(new { dados = resultado });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno ao processar os dados.", detalhes = ex.Message });
        }
    }
}
