using Xunit;
using TourManagement.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace TourManagement.Web.ViewModels.Tests;

public class TourViewModelTests
{
    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var viewModel = new TourViewModel();

        // Act
        viewModel.Id = 1;
        viewModel.TourName = "Paris Tour";
        viewModel.Place = "Paris";
        viewModel.Days = 5;
        viewModel.Price = 1000;
        viewModel.Locations = "Eiffel Tower";
        viewModel.TourInfo = "Best of Paris";
        viewModel.PictureFileName = "paris.jpg";

        // Assert
        Assert.Equal(1, viewModel.Id);
        Assert.Equal("Paris Tour", viewModel.TourName);
        Assert.Equal("Paris", viewModel.Place);
        Assert.Equal(5, viewModel.Days);
        Assert.Equal(1000, viewModel.Price);
        Assert.Equal("Eiffel Tower", viewModel.Locations);
        Assert.Equal("Best of Paris", viewModel.TourInfo);
        Assert.Equal("paris.jpg", viewModel.PictureFileName);
    }

    [Fact]
    public void Validation_FailsWhenTourNameIsEmpty()
    {
        // Arrange
        var viewModel = new TourViewModel
        {
            TourName = "",
            Place = "Paris",
            Days = 5,
            Price = 1000,
            Locations = "Locations",
            TourInfo = "Info"
        };
        var context = new ValidationContext(viewModel);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(viewModel, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
    }

    [Fact]
    public void Validation_FailsWhenDaysOutOfRange()
    {
        // Arrange
        var viewModel = new TourViewModel
        {
            TourName = "Tour",
            Place = "Paris",
            Days = 0,
            Price = 1000,
            Locations = "Locations",
            TourInfo = "Info"
        };
        var context = new ValidationContext(viewModel);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(viewModel, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Days"));
    }

    [Fact]
    public void Validation_PassesWhenAllFieldsValid()
    {
        // Arrange
        var viewModel = new TourViewModel
        {
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000,
            Locations = "Eiffel Tower",
            TourInfo = "Best of Paris"
        };
        var context = new ValidationContext(viewModel);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(viewModel, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}
