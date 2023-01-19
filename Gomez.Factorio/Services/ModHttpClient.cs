using Gomez.Factorio.Models;
using Gomez.Factorio.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Gomez.Factorio.Services
{
    public class ModHttpClient : IModHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ModdingOption _moddingOption;
        private readonly ILogger<ModHttpClient> _logger;

        public ModHttpClient(
            HttpClient httpClient,
            IOptions<ModdingOption> moddingOption,
            ILogger<ModHttpClient> logger)
        {
            _moddingOption = moddingOption.Value;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", _moddingOption.ApiKey);
            _logger = logger;
        }

        public async Task<bool> PostInitAsync(ModInfo info, string zipFilePath)
        {
            var requestUrl = $"{_moddingOption.ModPortalUrl}/api/v2/mods/releases/init_upload";
            var result = await _httpClient.PostAsync(requestUrl, new FormUrlEncodedContent(
                new Dictionary<string,string>(1) { { "mod", info.Name } }));
            if (result.IsSuccessStatusCode)
            {
                var initResponse = await result.Content.ReadFromJsonAsync<InitModPortalResponse>();
                if (await PostUploadAsync(zipFilePath, initResponse!))
                {
                    return true;
                }

                return false;
            }

            await LogErrorAsync(result, nameof(PostInitAsync));
            return false;
        }

        private async Task LogErrorAsync(HttpResponseMessage result, string type)
        {
            var errorResponse = await result.Content.ReadAsStringAsync();
            _logger.LogError(
                "{StatusCode}: Something went wrong with {Type}: {errorResponse}",
                result.StatusCode,
                type,
                errorResponse);
        }

        private async Task<bool> PostUploadAsync(string zipFilePath, InitModPortalResponse initResponse)
        {
            var fileStream = File.OpenRead(zipFilePath);
            var content = new MultipartFormDataContent
            {
                { new StreamContent(fileStream), "\"file\"", "\"file.zip\"" },
            };

            var result = await _httpClient.PostAsync(initResponse.UploadUrl, content);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }

            await LogErrorAsync(result, nameof(PostUploadAsync));
            return false;
        }
    }
}
