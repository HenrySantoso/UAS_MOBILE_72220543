using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Javademy.Data;
using Javademy.Models;

namespace Javademy.Pages
{
    public partial class CategoryEditPage : ContentPage
    {
        private int _currentCategoryId;

        public CategoryEditPage()
        {
            InitializeComponent();
        }

        public CategoryEditPage(int categoryId)
        {
            InitializeComponent();
            _currentCategoryId = categoryId; // Set the category ID
            LoadCategoryDetails(_currentCategoryId); // Load category details
        }

        private async void LoadCategoryDetails(int categoryId)
        {
            try
            {
                var category = await CategoriesManager.GetCategoryByIdAsync(categoryId);
                if (category != null)
                {
                    // Set the fields with category details
                    CategoryIdLabel.Text = $"Category ID: {category.CategoryId}";
                    CategoryNameEntry.Text = category.Name;
                    CategoryDescriptionEditor.Text = category.Description;
                }
                else
                {
                    await DisplayAlert("Error", "Category not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load category details: {ex.Message}", "OK");
            }
        }

        private async void OnUpdateButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var updatedCategory = new Category
                {
                    CategoryId = _currentCategoryId,
                    Name = CategoryNameEntry.Text,
                    Description = CategoryDescriptionEditor.Text
                };

                await CategoriesManager.UpdateCategory(updatedCategory);

                await DisplayAlert("Success", "Category updated successfully!", "OK");
                await Navigation.PopAsync(); // Go back to CategoryReadPage
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to update category: {ex.Message}", "OK");
            }
        }

        private async void OnResetButtonClicked(object sender, EventArgs e)
        {
            // Reset the fields to the selected category details
            LoadCategoryDetails(_currentCategoryId);
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navigate back to the previous page
            await Navigation.PopAsync();
        }
    }
}
