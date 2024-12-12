using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Javademy.Data;
using Javademy.Models;

namespace Javademy.Pages
{
    public partial class CourseCreatePage : ContentPage
    {
        public CourseCreatePage()
        {
            InitializeComponent();
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            // Gather input data
            string courseName = CourseNameEntry.Text?.Trim();
            string courseImageName = CourseImageNameEntry.Text?.Trim(); // Image Name entry
            string courseDescription = CourseDescriptionEditor.Text?.Trim();
            string courseDurationText = CourseDurationEntry.Text?.Trim();
            int? courseDuration = int.TryParse(courseDurationText, out int duration) ? duration : (int?)null;

            // Get category ID from the Entry
            string categoryIdText = CourseCategoryIdEntry.Text?.Trim();
            int? courseCategoryId = int.TryParse(categoryIdText, out int categoryId) ? categoryId : (int?)null;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(courseName) || string.IsNullOrWhiteSpace(courseImageName) ||
                string.IsNullOrWhiteSpace(courseDescription) || courseDuration == null || courseCategoryId == null)
            {
                await DisplayAlert("Error", "Please fill in all fields correctly.", "OK");
                return; // Exit early if validation fails
            }

            // Add the new course using CoursesManager
            try
            {
                var newCourse = await CoursesManager.AddCourse(courseName, courseImageName, courseDuration, courseDescription, courseCategoryId);
                await DisplayAlert("Success", "Course created successfully!", "OK");
                ResetForm(); // Reset the form after successful submission
            }
            catch (HttpRequestException httpEx) // Handle specific exception
            {
                await DisplayAlert("Error", $"HTTP Error: {httpEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to create course: {ex.Message}", "OK");
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            ResetForm(); // Call the reset method
        }

        private void ResetForm()
        {
            CourseNameEntry.Text = string.Empty;
            CourseImageNameEntry.Text = string.Empty; // Reset Image Name entry
            CourseDescriptionEditor.Text = string.Empty;
            CourseDurationEntry.Text = string.Empty;
            CourseCategoryIdEntry.Text = string.Empty; // Reset Category ID entry
        }
    }
}
