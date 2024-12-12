using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Javademy.Data;
using Javademy.Models;

namespace Javademy.Pages
{
    public partial class CategoryReadPage : ContentPage
    {
        private List<Category> allCategories; // Store all categories

        public CategoryReadPage()
        {
            InitializeComponent();
            LoadCategories();
        }

        private async void LoadCategories()
        {
            try
            {
                allCategories = (await CategoriesManager.GetAllCategories()).ToList();
                CategoryCollectionView.ItemsSource = allCategories; // Display all categories initially
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load categories: {ex.Message}", "OK");
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter is int categoryId)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this category?", "Yes", "No");
                if (confirmDelete)
                {
                    try
                    {
                        await CategoriesManager.DeleteCategory(categoryId);
                        await DisplayAlert("Success", "Category deleted successfully!", "OK");
                        LoadCategories(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to delete category: {ex.Message}", "OK");
                    }
                }
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            // Get the category ID from the button's CommandParameter
            var button = (Button)sender;
            int categoryId = (int)button.CommandParameter;

            // Navigate to CategoryEditPage and pass the category ID
            await Navigation.PushAsync(new CategoryEditPage(categoryId));
        }

        private void OnRefreshButtonClicked(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.ToLower() ?? string.Empty;

            // Filter categories based on search input
            var filteredCategories = string.IsNullOrWhiteSpace(searchText)
                ? allCategories // Show all if search text is empty
                : allCategories.Where(c => c.Name.ToLower().Contains(searchText)).ToList();

            // Update the CollectionView with filtered results
            CategoryCollectionView.ItemsSource = filteredCategories;
        }
    }
}
