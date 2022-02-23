using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json");
                
            try
            {
                // use the following IP address when run docker without K8S
                //var response = await _httpClient.PostAsync($"http://172.17.0.1:8081/api/c/platforms/", httpContent);

                // when run app in container, it is automatically change to Production environment (appsettings.Production.json)
                var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);

                if(response.IsSuccessStatusCode)
                {
                    Console.WriteLine("--> Sync POST to CommandService was OK!");
                }
                else
                {
                    Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} {e.StackTrace}");
            }
        }
    }
}