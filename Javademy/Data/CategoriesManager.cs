using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Javademy.Models;

namespace Javademy.Data
{
    public static class CategoriesManager
    {
        private static readonly string BaseAddress = "https://actbackendseervices.azurewebsites.net/api/categories";
        private static string authorizationKey;
        private static HttpClient client;

        // Method to get or create an HttpClient instance
        private static async Task<HttpClient> GetClient()
        {
            if (client != null)
                return client;

            client = new HttpClient();
            //await SetAuthorizationHeaderAsync();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.ConnectionClose = true;

            return client;
        }

        // Method to set the authorization header
        //private static async Task SetAuthorizationHeaderAsync()
        //{
        //    if (string.IsNullOrEmpty(authorizationKey))
        //    {
        //        client = new HttpClient(); // Initialize client before use
        //        var response = await client.GetStringAsync("https://actbackendseervices.azurewebsites.net/api/login"); //Ensure this is the correct login endpoint
        //        authorizationKey = JsonSerializer.Deserialize<string>(response);
        //        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorizationKey);
        //    }
        //}

        // Method to get all categories
        public static async Task<IEnumerable<Category>> GetAllCategories()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return new List<Category>();

            var client = await GetClient();
            string result = await client.GetStringAsync(BaseAddress);

            return JsonSerializer.Deserialize<List<Category>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        // Method to get a category by ID
        public static async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            var client = await GetClient();
            string apiUrl = $"{BaseAddress}/{categoryId}";

            try
            {
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var category = await response.Content.ReadFromJsonAsync<Category>();
                return category;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }

        // Method to add a new category
        public static async Task<Category> AddCategory(string name, string description)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return null;

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty", nameof(name));

            var category = new Category
            {
                Name = name,
                Description = description
            };

            var msg = new HttpRequestMessage(HttpMethod.Post, BaseAddress)
            {
                Content = JsonContent.Create(category)
            };

            var client = await GetClient();
            var response = await client.SendAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                var returnedJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Category>(returnedJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Error saving category: {response.StatusCode} - {errorContent}");
                throw new Exception($"Error saving category: {response.StatusCode} - {errorContent}");
            }
        }

        // Method to update an existing category
        public static async Task UpdateCategory(Category category)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return;

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            HttpRequestMessage msg = new(HttpMethod.Put, $"{BaseAddress}/{category.CategoryId}")
            {
                Content = JsonContent.Create(category)
            };

            var client = await GetClient();
            var response = await client.SendAsync(msg);
            response.EnsureSuccessStatusCode();
        }

        // Method to delete a category
        public static async Task DeleteCategory(int categoryId)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return;

            HttpRequestMessage msg = new(HttpMethod.Delete, $"{BaseAddress}/{categoryId}");
            var client = await GetClient();
            var response = await client.SendAsync(msg);
            response.EnsureSuccessStatusCode();
        }
    }
}
