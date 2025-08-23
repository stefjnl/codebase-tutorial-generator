using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetTutorialGenerator.Infrastructure.GitHub.Models;

namespace DotNetTutorialGenerator.Infrastructure.GitHub
{
    public class GitHubApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.github.com";

        public GitHubApiClient(HttpClient httpClient, string? token = null)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DotNetTutorialGenerator/1.0");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<GitHubRepository?> GetRepositoryAsync(string owner, string repo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/repos/{owner}/{repo}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GitHubRepository>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<GitHubContent[]?> GetRepositoryContentsAsync(string owner, string repo, string path = "")
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/repos/{owner}/{repo}/contents/{path}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GitHubContent[]>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> GetFileContentAsync(string downloadUrl)
        {
            try
            {
                var response = await _httpClient.GetAsync(downloadUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
