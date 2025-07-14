using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Web;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace TuNombreDeEspacio
{
    /// <summary>
    /// Cliente para interactuar con Infoblox WAPI: zonas y registros TXT.
    /// </summary>
    public class InfobloxClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        /// <summary>
        /// Inicializa una nueva instancia del cliente Infoblox.
        /// </summary>
        /// <param name="config">Configuración de la aplicación.</param>
        /// <param name="httpFactory">Fábrica de HttpClient.</param>
        public InfobloxClient(IConfiguration config, IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("Infoblox");
            _baseUrl = config["INFOBLOX_WAPI_URL"]!;
            var username = config["INFOBLOX_USERNAME"]!;
            var password = config["INFOBLOX_PASSWORD"]!;
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
        }

        /// <summary>
        /// Obtiene todas las zonas DNS.
        /// </summary>
        public async Task<List<dynamic>> GetZonesAsync()
        {
            var res = await _http.GetAsync($"{_baseUrl}/zone_auth");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<dynamic>>(json)!;
        }

        /// <summary>
        /// Busca la zona más específica para un FQDN.
        /// </summary>
        public async Task<string?> FindZoneAsync(string fqdn)
        {
            var parts = fqdn.TrimEnd('.').Split('.');
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var candidate = string.Join('.', parts.Skip(i));
                var res = await _http.GetAsync($"{_baseUrl}/zone_auth?fqdn={HttpUtility.UrlEncode(candidate)}");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var list = JsonSerializer.Deserialize<List<dynamic>>(json);
                    if (list?.Count > 0)
                        return list[0].fqdn.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene registros TXT por nombre.
        /// </summary>
        public async Task<List<dynamic>> GetTxtRecordsAsync(string name)
        {
            var res = await _http.GetAsync($"{_baseUrl}/record:txt?name={HttpUtility.UrlEncode(name)}");
            if (!res.IsSuccessStatusCode)
                return new List<dynamic>();

            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<dynamic>>(json)!;
        }

        /// <summary>
        /// Elimina todos los registros TXT por nombre.
        /// </summary>
        public async Task DeleteTxtRecordsAsync(string name)
        {
            var records = await GetTxtRecordsAsync(name);
            foreach (var rec in records)
            {
                // Use JsonElement API to get _ref property
                var href = ((JsonElement)rec).GetProperty("_ref").GetString();
                if (!string.IsNullOrEmpty(href))
                {
                    await _http.DeleteAsync($"{_baseUrl}/{href}");
                }
            }
        }

        /// <summary>
        /// Crea o actualiza registros TXT en una zona.
        /// </summary>
        public async Task UpsertTxtRecordAsync(string zone, string name, List<string> values, int ttl)
        {
            await DeleteTxtRecordsAsync(name);

            foreach (var val in values)
            {
                var data = new Dictionary<string, object> {
                    { "name", name },
                    { "text", val },
                    { "ttl", ttl }
                    // { "view", "default" } // Descomenta si tu Infoblox usa views
                };

                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var res = await _http.PostAsync($"{_baseUrl}/record:txt", content);
                res.EnsureSuccessStatusCode();
            }
        }
    }
}
