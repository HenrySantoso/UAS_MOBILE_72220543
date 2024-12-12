using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
        public async Task<(string token, string errorMessage)> LoginAsync(Login credentials)
        {
            try
            {
                // Send POST request to the API with the login credentials
                var response = await _httpClient.PostAsJsonAsync(LoginEndpoint, credentials);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response body (which includes the Token)
                    var responseBody = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (responseBody != null && responseBody.ContainsKey("token"))
                    {
                        var token = responseBody["token"];

                        // Save the token securely using SecureStorage
                        await SecureStorage.SetAsync("auth_token", token);

                        return (token, null); // Return the token if successful
                    }
                    else
                    {
                        return (null, "Unexpected error: Token is missing.");
                    }
                }
                else
                {
                    // Handle unsuccessful login attempts, return error message
                    return (null, "Login failed: Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network issues)
                Console.WriteLine($"Login Error: {ex.Message}");
                return (null, "An error occurred while trying to log in. Please try again.");
            }
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
