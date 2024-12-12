using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Maui.Storage;
using Javademy.Models;

namespace Javademy.Data
{
    public class LoginManager
    {
        private readonly HttpClient _httpClient;
        private readonly string LoginEndpoint = "https://actbackendseervices.azurewebsites.net/api/login"; // API endpoint

        public LoginManager()
        {
            _httpClient = new HttpClient();
        }

        // LoginAsync method to handle user login
        public async Task<string> LoginAsync(Login credentials)
        {
            try
            {
                // Send POST request to the API with the login credentials
                var response = await _httpClient.PostAsJsonAsync(LoginEndpoint, credentials);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response and get the token
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginRespond>();
                    var token = loginResponse?.Token;

                    if (token != null)
                    {
                        // Save the token securely using SecureStorage
                        await SecureStorage.SetAsync("auth_token", token);
                        return token; // Return the token if successful
                    }
                }
                else
                {
                    // Handle failed login attempts
                    Console.WriteLine($"Login failed: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network issues)
                Console.WriteLine($"Login Error: {ex.Message}");
                return null;
            }

            return null;
        }

        public async Task AddAuthHeaderAsync()
        {
            // Get the token from SecureStorage
            var token = await SecureStorage.GetAsync("auth_token");
            if (token != null)
            {
                // Add the token to the Authorization header
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                // Handle missing token
                Console.WriteLine("Token not found.");
            }
        }
    }
}
