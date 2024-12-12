using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Javademy.Data;
using Javademy.Models;

namespace Javademy.Pages
{
    public partial class CourseReadPage : ContentPage
    {
        private List<Course> allCourses; // Store all courses

        public CourseReadPage()
        {
            InitializeComponent();
            LoadCourses();
        }

        private async void LoadCourses()
        {
            try
            {
                allCourses = await CoursesManager.GetAllCourses();
                CourseCollectionView.ItemsSource = allCourses; // Display all courses initially
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load courses: {ex.Message}", "OK");
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (button.CommandParameter is int courseId)
            {
                await Navigation.PushAsync(new CourseEditPage(courseId));
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (button.CommandParameter is int courseId)
            {
                bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this course?", "Yes", "No");
                if (confirmDelete)
                {
                    try
                    {
                        await CoursesManager.DeleteCourse(courseId);
                        await DisplayAlert("Success", "Course deleted successfully!", "OK");
                        LoadCourses(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to delete course: {ex.Message}", "OK");
                    }
                }
            }
        }

        private void OnRefreshButtonClicked(object sender, EventArgs e)
        {
            LoadCourses();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.ToLower() ?? string.Empty;

            // Filter courses based on search input
            var filteredCourses = string.IsNullOrWhiteSpace(searchText)
                ? allCourses // Show all if search text is empty
                : allCourses.Where(c => c.Name.ToLower().Contains(searchText)).ToList();

            // Update the CollectionView with filtered results
            CourseCollectionView.ItemsSource = filteredCourses;
        }
    }
}
