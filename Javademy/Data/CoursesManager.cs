using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Javademy.Models;

namespace Javademy.Data
{
    public static class CoursesManager
    {
        static readonly string BaseAddress = "https://actbackendseervices.azurewebsites.net"; // Base address for the API
        static readonly string Url = $"{BaseAddress}/api/courses"; // Endpoint for courses
        private static string authorizationKey;

        static HttpClient client;

        private static async Task<HttpClient> GetClient()
        {
            if (client != null)
                return client;

            client = new HttpClient();

            //// Get authorization key if needed
            //if (string.IsNullOrEmpty(authorizationKey))
            //{
            //    var response = await client.GetStringAsync($"{BaseAddress}/api/v1/login"); // Update with actual login endpoint
            //    authorizationKey = JsonSerializer.Deserialize<string>(response);
            //}

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.ConnectionClose = true; // Close connection after response

            return client;
        }


        // Method to get all courses
        public static async Task<List<Course>> GetAllCourses()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return new List<Course>();

            var client = await GetClient();
            return await client.GetFromJsonAsync<List<Course>>(Url);
        }

        // Method to add a course
        public static async Task<Course> AddCourse(string name, string imageName, int? duration, string description, int? categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Course name is required.");

            var course = new Course
            {
                Name = name,
                ImageName = imageName,
                Duration = duration,
                Description = description,
                CategoryId = categoryId
            };

            var msg = new HttpRequestMessage(HttpMethod.Post, Url);
            msg.Content = JsonContent.Create(course);

            var client = await GetClient();
            var response = await client.SendAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                var returnedJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Course>(returnedJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error saving course: {response.StatusCode} - {errorContent}");
            }
        }

        // Method to update a course
        public static async Task UpdateCourse(Course course)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return;

            HttpRequestMessage msg = new(HttpMethod.Put, $"{Url}/{course.CourseId}");
            msg.Content = JsonContent.Create<Course>(course);
            var client = await GetClient();
            var response = await client.SendAsync(msg);
            response.EnsureSuccessStatusCode();
        }

        // Method to delete a course
        public static async Task DeleteCourse(int courseId)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return;

            HttpRequestMessage msg = new(HttpMethod.Delete, $"{Url}/{courseId}");
            var client = await GetClient();
            var response = await client.SendAsync(msg);
            response.EnsureSuccessStatusCode();
        }

        // Method to get a course by its ID
        public static async Task<Course> GetCourseById(int courseId)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return null;

            var client = await GetClient();
            var response = await client.GetAsync($"{Url}/{courseId}");

            if (response.IsSuccessStatusCode)
            {
                var returnedJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Course>(returnedJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error retrieving course: {response.StatusCode} - {errorContent}");
            }
        }

        public static async Task<List<Course>> GetCourseByName(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                throw new ArgumentException("Search text is required.");

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return new List<Course>();

            var client = await GetClient();
            var response = await client.GetAsync($"{Url}?name={Uri.EscapeDataString(searchText)}");

            if (response.IsSuccessStatusCode)
            {
                var returnedJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Course>>(returnedJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error retrieving courses: {response.StatusCode} - {errorContent}");
            }
        }
    }
}
