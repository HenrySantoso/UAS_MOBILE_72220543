using Javademy.Data;
using Microsoft.Maui.Controls;
using Javademy.Models;

namespace Javademy.Pages
{
    public partial class CategoryCreatePage : ContentPage
    {
        public CategoryCreatePage()
        {
            InitializeComponent();
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            string categoryName = CategoryNameEntry.Text;
            string categoryDescription = CategoryDescriptionEditor.Text;

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                await DisplayAlert("Validation Error", "Category name is required.", "OK");
                return;
            }

            try
            {
                var newCategory = await CategoriesManager.AddCategory(categoryName, categoryDescription);
                if (newCategory != null)
                {
                    await DisplayAlert("Success", "Category created successfully!", "OK");
                    await Navigation.PopAsync(); // Navigate back after successful creation
                }
                else
                {
                    await DisplayAlert("Error", "Failed to create category. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to create category: {ex.Message}", "OK");
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            // Clear the input fields
            CategoryNameEntry.Text = string.Empty;
            CategoryDescriptionEditor.Text = string.Empty;
        }
    }
}
