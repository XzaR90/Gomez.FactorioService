using Gomez.Factorio.Models;
using Gomez.Factorio.Options;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Gomez.Factorio.Services
{
    public class ModHttpClient : IModHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ModdingOption _moddingOption;

        public ModHttpClient(HttpClient httpClient, IOptions<ModdingOption> moddingOption)
        {
            _moddingOption = moddingOption.Value;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization 
                = new AuthenticationHeaderValue("Bearer", _moddingOption.ApiKey);
        }

        public async Task PostInitAsync(ModInfo info, string zipFilePath)
        {
            var requestUrl = $"{_moddingOption.ModPortalUrl}/api/v2/mods/releases/init_upload";
            var result = await _httpClient.PostAsJsonAsync(requestUrl, new { mod = info.Name });
            result.EnsureSuccessStatusCode();

            var initResponse = await result.Content.ReadFromJsonAsync<InitModPortalResponse>();
            await PostUploadAsync(zipFilePath, initResponse!);
        }

        private async Task PostUploadAsync(string zipFilePath, InitModPortalResponse initResponse)
        {
            var fileStream = File.OpenRead(zipFilePath);
            var content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "\"file\"", "\"file.zip\"" },
            };

            var result = await _httpClient.PostAsync(initResponse.UploadUrl, content);
            result.EnsureSuccessStatusCode();
        }
    }
}
