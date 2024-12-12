using Javademy.Models;
using Newtonsoft.Json;
using System.Text;

namespace Javademy.Pages
{
    public partial class LoginPage : ContentPage
    {
        int count = 0;
        public static string AuthToken { get; private set; }

        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnTogglePasswordVisibilityClicked(object sender, EventArgs e)
        {
            // Toggle the password visibility
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;

            // Change button text based on password visibility
            TogglePasswordVisibilityButton.Text = PasswordEntry.IsPassword ? "Show Password" : "Hide Password";
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            var loginData = new
            {
                userName = username,
                password = password
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://actbackendseervices.azurewebsites.net/api/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    AuthToken = JsonConvert.DeserializeObject<Login>(responseContent).token;

                    // Save the token (e.g., in Preferences)
                    Preferences.Set("auth_token", AuthToken);

                    // Navigate to the next page
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    // Handle login failure
                    await DisplayAlert("Login Failed", "Invalid username or password", "OK");
                }
            }
        }
    }
}
