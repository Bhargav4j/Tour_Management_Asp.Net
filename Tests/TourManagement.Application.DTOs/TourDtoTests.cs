using System;
using Xunit;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.DTOs.Tests
{
    /// <summary>
    /// Tests for TourDto
    /// </summary>
    public class TourDtoTests
    {
        [Fact]
        public void TourDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new TourDto();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.TourName);
            Assert.Equal(string.Empty, dto.Place);
            Assert.Equal(0, dto.Days);
            Assert.Equal(0, dto.Price);
            Assert.Equal(string.Empty, dto.Locations);
            Assert.Equal(string.Empty, dto.TourInfo);
            Assert.Null(dto.ImageFileName);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void TourDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new TourDto();
            var createdDate = DateTime.Now;

            // Act
            dto.Id = 100;
            dto.TourName = "Paris Tour";
            dto.Place = "Paris";
            dto.Days = 7;
            dto.Price = 1500.00m;
            dto.Locations = "Eiffel Tower, Louvre";
            dto.TourInfo = "Amazing tour";
            dto.ImageFileName = "paris.jpg";
            dto.CreatedDate = createdDate;
            dto.IsActive = true;

            // Assert
            Assert.Equal(100, dto.Id);
            Assert.Equal("Paris Tour", dto.TourName);
            Assert.Equal("Paris", dto.Place);
            Assert.Equal(7, dto.Days);
            Assert.Equal(1500.00m, dto.Price);
            Assert.Equal("Eiffel Tower, Louvre", dto.Locations);
            Assert.Equal("Amazing tour", dto.TourInfo);
            Assert.Equal("paris.jpg", dto.ImageFileName);
            Assert.Equal(createdDate, dto.CreatedDate);
            Assert.True(dto.IsActive);
        }

        [Fact]
        public void TourDto_ImageFileName_CanBeNull()
        {
            // Arrange
            var dto = new TourDto();

            // Act
            dto.ImageFileName = null;

            // Assert
            Assert.Null(dto.ImageFileName);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TourDto_IsActive_ShouldSetCorrectly(bool isActive)
        {
            // Arrange
            var dto = new TourDto();

            // Act
            dto.IsActive = isActive;

            // Assert
            Assert.Equal(isActive, dto.IsActive);
        }
    }

    /// <summary>
    /// Tests for TourCreateDto
    /// </summary>
    public class TourCreateDtoTests
    {
        [Fact]
        public void TourCreateDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new TourCreateDto();

            // Assert
            Assert.Equal(string.Empty, dto.TourName);
            Assert.Equal(string.Empty, dto.Place);
            Assert.Equal(0, dto.Days);
            Assert.Equal(0, dto.Price);
            Assert.Equal(string.Empty, dto.Locations);
            Assert.Equal(string.Empty, dto.TourInfo);
            Assert.Null(dto.ImageFileName);
        }

        [Fact]
        public void TourCreateDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new TourCreateDto();

            // Act
            dto.TourName = "Tokyo Tour";
            dto.Place = "Tokyo";
            dto.Days = 10;
            dto.Price = 2500.00m;
            dto.Locations = "Shibuya, Shinjuku";
            dto.TourInfo = "Explore Tokyo";
            dto.ImageFileName = "tokyo.jpg";

            // Assert
            Assert.Equal("Tokyo Tour", dto.TourName);
            Assert.Equal("Tokyo", dto.Place);
            Assert.Equal(10, dto.Days);
            Assert.Equal(2500.00m, dto.Price);
            Assert.Equal("Shibuya, Shinjuku", dto.Locations);
            Assert.Equal("Explore Tokyo", dto.TourInfo);
            Assert.Equal("tokyo.jpg", dto.ImageFileName);
        }

        [Fact]
        public void TourCreateDto_ImageFileName_CanBeNull()
        {
            // Arrange
            var dto = new TourCreateDto();

            // Act
            dto.ImageFileName = null;

            // Assert
            Assert.Null(dto.ImageFileName);
        }

        [Fact]
        public void TourCreateDto_WithObjectInitializer_ShouldSetAllProperties()
        {
            // Arrange & Act
            var dto = new TourCreateDto
            {
                TourName = "Rome Tour",
                Place = "Rome",
                Days = 5,
                Price = 1200.00m,
                Locations = "Colosseum, Vatican",
                TourInfo = "Historical tour",
                ImageFileName = "rome.jpg"
            };

            // Assert
            Assert.Equal("Rome Tour", dto.TourName);
            Assert.Equal("Rome", dto.Place);
            Assert.Equal(5, dto.Days);
            Assert.Equal(1200.00m, dto.Price);
            Assert.Equal("Colosseum, Vatican", dto.Locations);
            Assert.Equal("Historical tour", dto.TourInfo);
            Assert.Equal("rome.jpg", dto.ImageFileName);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(7)]
        [InlineData(30)]
        public void TourCreateDto_Days_ShouldAcceptVariousValues(int days)
        {
            // Arrange
            var dto = new TourCreateDto();

            // Act
            dto.Days = days;

            // Assert
            Assert.Equal(days, dto.Days);
        }

        [Theory]
        [InlineData(100.00)]
        [InlineData(0)]
        [InlineData(9999.99)]
        public void TourCreateDto_Price_ShouldAcceptVariousValues(decimal price)
        {
            // Arrange
            var dto = new TourCreateDto();

            // Act
            dto.Price = price;

            // Assert
            Assert.Equal(price, dto.Price);
        }
    }

    /// <summary>
    /// Tests for TourUpdateDto
    /// </summary>
    public class TourUpdateDtoTests
    {
        [Fact]
        public void TourUpdateDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new TourUpdateDto();

            // Assert
            Assert.Equal(string.Empty, dto.TourName);
            Assert.Equal(string.Empty, dto.Place);
            Assert.Equal(0, dto.Days);
            Assert.Equal(0, dto.Price);
            Assert.Equal(string.Empty, dto.Locations);
            Assert.Equal(string.Empty, dto.TourInfo);
            Assert.Null(dto.ImageFileName);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void TourUpdateDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new TourUpdateDto();

            // Act
            dto.TourName = "Updated Tour";
            dto.Place = "Updated Place";
            dto.Days = 14;
            dto.Price = 3000.00m;
            dto.Locations = "Location 1, Location 2";
            dto.TourInfo = "Updated info";
            dto.ImageFileName = "updated.jpg";
            dto.IsActive = true;

            // Assert
            Assert.Equal("Updated Tour", dto.TourName);
            Assert.Equal("Updated Place", dto.Place);
            Assert.Equal(14, dto.Days);
            Assert.Equal(3000.00m, dto.Price);
            Assert.Equal("Location 1, Location 2", dto.Locations);
            Assert.Equal("Updated info", dto.TourInfo);
            Assert.Equal("updated.jpg", dto.ImageFileName);
            Assert.True(dto.IsActive);
        }

        [Fact]
        public void TourUpdateDto_ImageFileName_CanBeNull()
        {
            // Arrange
            var dto = new TourUpdateDto();

            // Act
            dto.ImageFileName = null;

            // Assert
            Assert.Null(dto.ImageFileName);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TourUpdateDto_IsActive_ShouldSetCorrectly(bool isActive)
        {
            // Arrange
            var dto = new TourUpdateDto();

            // Act
            dto.IsActive = isActive;

            // Assert
            Assert.Equal(isActive, dto.IsActive);
        }

        [Fact]
        public void TourUpdateDto_WithObjectInitializer_ShouldSetAllProperties()
        {
            // Arrange & Act
            var dto = new TourUpdateDto
            {
                TourName = "Berlin Tour",
                Place = "Berlin",
                Days = 6,
                Price = 1800.00m,
                Locations = "Brandenburg Gate, Berlin Wall",
                TourInfo = "Modern history tour",
                ImageFileName = "berlin.jpg",
                IsActive = true
            };

            // Assert
            Assert.Equal("Berlin Tour", dto.TourName);
            Assert.Equal("Berlin", dto.Place);
            Assert.Equal(6, dto.Days);
            Assert.Equal(1800.00m, dto.Price);
            Assert.Equal("Brandenburg Gate, Berlin Wall", dto.Locations);
            Assert.Equal("Modern history tour", dto.TourInfo);
            Assert.Equal("berlin.jpg", dto.ImageFileName);
            Assert.True(dto.IsActive);
        }

        [Fact]
        public void TourUpdateDto_InactiveTour_ShouldHaveIsActiveFalse()
        {
            // Arrange
            var dto = new TourUpdateDto();

            // Act
            dto.IsActive = false;

            // Assert
            Assert.False(dto.IsActive);
        }
    }
}
