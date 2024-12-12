using Microsoft.Maui.Controls;
using Javademy.Data;
using Javademy.Models;
using System;

namespace Javademy.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly LoginManager _loginManager;

        public LoginPage()
        {
            InitializeComponent();
            _loginManager = new LoginManager(); // Initialize LoginManager
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorLabel.Text = "Please enter both username and password.";
                ErrorLabel.IsVisible = true;
                return;
            }

            try
            {
                var credentials = new Login { UserName = username, Password = password };
                string token = await _loginManager.LoginAsync(credentials);

                if (token != null)
                {
                    // Successful login, navigate to MainPage
                    ErrorLabel.IsVisible = false;
                    await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    // Invalid credentials
                    ErrorLabel.Text = "Invalid username or password.";
                    ErrorLabel.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
            }
        }
    }
}
