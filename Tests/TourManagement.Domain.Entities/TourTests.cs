using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests
{
    /// <summary>
    /// Tests for Tour entity
    /// </summary>
    public class TourTests
    {
        [Fact]
        public void Tour_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var tour = new Tour();

            // Assert
            Assert.Equal(0, tour.Id);
            Assert.Equal(string.Empty, tour.TourName);
            Assert.Equal(string.Empty, tour.Place);
            Assert.Equal(0, tour.Days);
            Assert.Equal(0, tour.Price);
            Assert.Equal(string.Empty, tour.Locations);
            Assert.Equal(string.Empty, tour.TourInfo);
            Assert.Null(tour.ImageFileName);
            Assert.True(tour.IsActive);
            Assert.Equal("System", tour.CreatedBy);
            Assert.Null(tour.ModifiedBy);
            Assert.NotNull(tour.Bookings);
            Assert.Empty(tour.Bookings);
        }

        [Fact]
        public void Tour_Id_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedId = 123;

            // Act
            tour.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, tour.Id);
        }

        [Fact]
        public void Tour_TourName_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedName = "Amazing Tour";

            // Act
            tour.TourName = expectedName;

            // Assert
            Assert.Equal(expectedName, tour.TourName);
        }

        [Fact]
        public void Tour_Place_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedPlace = "Paris";

            // Act
            tour.Place = expectedPlace;

            // Assert
            Assert.Equal(expectedPlace, tour.Place);
        }

        [Fact]
        public void Tour_Days_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedDays = 7;

            // Act
            tour.Days = expectedDays;

            // Assert
            Assert.Equal(expectedDays, tour.Days);
        }

        [Theory]
        [InlineData(100.50)]
        [InlineData(0)]
        [InlineData(999.99)]
        public void Tour_Price_ShouldSetAndGetCorrectly(decimal expectedPrice)
        {
            // Arrange
            var tour = new Tour();

            // Act
            tour.Price = expectedPrice;

            // Assert
            Assert.Equal(expectedPrice, tour.Price);
        }

        [Fact]
        public void Tour_Locations_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedLocations = "Paris, London, Rome";

            // Act
            tour.Locations = expectedLocations;

            // Assert
            Assert.Equal(expectedLocations, tour.Locations);
        }

        [Fact]
        public void Tour_TourInfo_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedInfo = "Best tour ever";

            // Act
            tour.TourInfo = expectedInfo;

            // Assert
            Assert.Equal(expectedInfo, tour.TourInfo);
        }

        [Fact]
        public void Tour_ImageFileName_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedFileName = "image.jpg";

            // Act
            tour.ImageFileName = expectedFileName;

            // Assert
            Assert.Equal(expectedFileName, tour.ImageFileName);
        }

        [Fact]
        public void Tour_ImageFileName_CanBeNull()
        {
            // Arrange
            var tour = new Tour();

            // Act
            tour.ImageFileName = null;

            // Assert
            Assert.Null(tour.ImageFileName);
        }

        [Fact]
        public void Tour_CreatedDate_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedDate = DateTime.Now;

            // Act
            tour.CreatedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, tour.CreatedDate);
        }

        [Fact]
        public void Tour_ModifiedDate_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedDate = DateTime.Now;

            // Act
            tour.ModifiedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, tour.ModifiedDate);
        }

        [Fact]
        public void Tour_ModifiedDate_CanBeNull()
        {
            // Arrange
            var tour = new Tour();

            // Act
            tour.ModifiedDate = null;

            // Assert
            Assert.Null(tour.ModifiedDate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Tour_IsActive_ShouldSetAndGetCorrectly(bool expectedValue)
        {
            // Arrange
            var tour = new Tour();

            // Act
            tour.IsActive = expectedValue;

            // Assert
            Assert.Equal(expectedValue, tour.IsActive);
        }

        [Fact]
        public void Tour_CreatedBy_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedCreator = "Admin";

            // Act
            tour.CreatedBy = expectedCreator;

            // Assert
            Assert.Equal(expectedCreator, tour.CreatedBy);
        }

        [Fact]
        public void Tour_ModifiedBy_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var expectedModifier = "User123";

            // Act
            tour.ModifiedBy = expectedModifier;

            // Assert
            Assert.Equal(expectedModifier, tour.ModifiedBy);
        }

        [Fact]
        public void Tour_ModifiedBy_CanBeNull()
        {
            // Arrange
            var tour = new Tour();

            // Act
            tour.ModifiedBy = null;

            // Assert
            Assert.Null(tour.ModifiedBy);
        }

        [Fact]
        public void Tour_Bookings_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var tour = new Tour();

            // Assert
            Assert.NotNull(tour.Bookings);
            Assert.Empty(tour.Bookings);
        }

        [Fact]
        public void Tour_Bookings_ShouldAddBookingsCorrectly()
        {
            // Arrange
            var tour = new Tour();
            var booking1 = new Booking { Id = 1 };
            var booking2 = new Booking { Id = 2 };

            // Act
            tour.Bookings.Add(booking1);
            tour.Bookings.Add(booking2);

            // Assert
            Assert.Equal(2, tour.Bookings.Count);
            Assert.Contains(booking1, tour.Bookings);
            Assert.Contains(booking2, tour.Bookings);
        }

        [Fact]
        public void Tour_FullObjectInitialization_ShouldSetAllPropertiesCorrectly()
        {
            // Arrange
            var createdDate = DateTime.Now;
            var modifiedDate = DateTime.Now.AddDays(1);
            var bookings = new List<Booking> { new Booking { Id = 1 } };

            // Act
            var tour = new Tour
            {
                Id = 100,
                TourName = "European Tour",
                Place = "Europe",
                Days = 14,
                Price = 2500.00m,
                Locations = "Paris, Rome, Berlin",
                TourInfo = "A comprehensive European tour",
                ImageFileName = "europe.jpg",
                CreatedDate = createdDate,
                ModifiedDate = modifiedDate,
                IsActive = true,
                CreatedBy = "AdminUser",
                ModifiedBy = "EditorUser",
                Bookings = bookings
            };

            // Assert
            Assert.Equal(100, tour.Id);
            Assert.Equal("European Tour", tour.TourName);
            Assert.Equal("Europe", tour.Place);
            Assert.Equal(14, tour.Days);
            Assert.Equal(2500.00m, tour.Price);
            Assert.Equal("Paris, Rome, Berlin", tour.Locations);
            Assert.Equal("A comprehensive European tour", tour.TourInfo);
            Assert.Equal("europe.jpg", tour.ImageFileName);
            Assert.Equal(createdDate, tour.CreatedDate);
            Assert.Equal(modifiedDate, tour.ModifiedDate);
            Assert.True(tour.IsActive);
            Assert.Equal("AdminUser", tour.CreatedBy);
            Assert.Equal("EditorUser", tour.ModifiedBy);
            Assert.Single(tour.Bookings);
        }
    }
}
