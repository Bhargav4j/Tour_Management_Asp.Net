using System;
using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests
{
    /// <summary>
    /// Tests for Booking entity
    /// </summary>
    public class BookingTests
    {
        [Fact]
        public void Booking_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var booking = new Booking();

            // Assert
            Assert.Equal(0, booking.Id);
            Assert.Equal(0, booking.TourId);
            Assert.Equal(0, booking.UserId);
            Assert.Equal(0, booking.NumberOfPeople);
            Assert.Equal(0, booking.TotalAmount);
            Assert.Equal("Pending", booking.Status);
            Assert.Null(booking.Notes);
            Assert.True(booking.IsActive);
            Assert.Equal("System", booking.CreatedBy);
            Assert.Null(booking.ModifiedBy);
            Assert.Null(booking.Tour);
            Assert.Null(booking.User);
        }

        [Fact]
        public void Booking_Id_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedId = 789;

            // Act
            booking.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, booking.Id);
        }

        [Fact]
        public void Booking_TourId_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedTourId = 100;

            // Act
            booking.TourId = expectedTourId;

            // Assert
            Assert.Equal(expectedTourId, booking.TourId);
        }

        [Fact]
        public void Booking_UserId_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedUserId = 200;

            // Act
            booking.UserId = expectedUserId;

            // Assert
            Assert.Equal(expectedUserId, booking.UserId);
        }

        [Fact]
        public void Booking_BookingDate_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedDate = DateTime.Now;

            // Act
            booking.BookingDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, booking.BookingDate);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void Booking_NumberOfPeople_ShouldSetAndGetCorrectly(int expectedNumber)
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.NumberOfPeople = expectedNumber;

            // Assert
            Assert.Equal(expectedNumber, booking.NumberOfPeople);
        }

        [Theory]
        [InlineData(100.00)]
        [InlineData(0)]
        [InlineData(5000.99)]
        public void Booking_TotalAmount_ShouldSetAndGetCorrectly(decimal expectedAmount)
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.TotalAmount = expectedAmount;

            // Assert
            Assert.Equal(expectedAmount, booking.TotalAmount);
        }

        [Theory]
        [InlineData("Pending")]
        [InlineData("Confirmed")]
        [InlineData("Cancelled")]
        [InlineData("Completed")]
        public void Booking_Status_ShouldSetAndGetCorrectly(string expectedStatus)
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.Status = expectedStatus;

            // Assert
            Assert.Equal(expectedStatus, booking.Status);
        }

        [Fact]
        public void Booking_Notes_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedNotes = "Special dietary requirements";

            // Act
            booking.Notes = expectedNotes;

            // Assert
            Assert.Equal(expectedNotes, booking.Notes);
        }

        [Fact]
        public void Booking_Notes_CanBeNull()
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.Notes = null;

            // Assert
            Assert.Null(booking.Notes);
        }

        [Fact]
        public void Booking_CreatedDate_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedDate = DateTime.Now;

            // Act
            booking.CreatedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, booking.CreatedDate);
        }

        [Fact]
        public void Booking_ModifiedDate_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedDate = DateTime.Now.AddDays(3);

            // Act
            booking.ModifiedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, booking.ModifiedDate);
        }

        [Fact]
        public void Booking_ModifiedDate_CanBeNull()
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.ModifiedDate = null;

            // Assert
            Assert.Null(booking.ModifiedDate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Booking_IsActive_ShouldSetAndGetCorrectly(bool expectedValue)
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.IsActive = expectedValue;

            // Assert
            Assert.Equal(expectedValue, booking.IsActive);
        }

        [Fact]
        public void Booking_CreatedBy_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedCreator = "AdminUser";

            // Act
            booking.CreatedBy = expectedCreator;

            // Assert
            Assert.Equal(expectedCreator, booking.CreatedBy);
        }

        [Fact]
        public void Booking_ModifiedBy_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedModifier = "Editor456";

            // Act
            booking.ModifiedBy = expectedModifier;

            // Assert
            Assert.Equal(expectedModifier, booking.ModifiedBy);
        }

        [Fact]
        public void Booking_ModifiedBy_CanBeNull()
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.ModifiedBy = null;

            // Assert
            Assert.Null(booking.ModifiedBy);
        }

        [Fact]
        public void Booking_Tour_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedTour = new Tour { Id = 50 };

            // Act
            booking.Tour = expectedTour;

            // Assert
            Assert.Equal(expectedTour, booking.Tour);
        }

        [Fact]
        public void Booking_Tour_CanBeNull()
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.Tour = null;

            // Assert
            Assert.Null(booking.Tour);
        }

        [Fact]
        public void Booking_User_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var booking = new Booking();
            var expectedUser = new User { Id = 75 };

            // Act
            booking.User = expectedUser;

            // Assert
            Assert.Equal(expectedUser, booking.User);
        }

        [Fact]
        public void Booking_User_CanBeNull()
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.User = null;

            // Assert
            Assert.Null(booking.User);
        }

        [Fact]
        public void Booking_FullObjectInitialization_ShouldSetAllPropertiesCorrectly()
        {
            // Arrange
            var bookingDate = DateTime.Now;
            var createdDate = DateTime.Now;
            var modifiedDate = DateTime.Now.AddDays(1);
            var tour = new Tour { Id = 1, TourName = "Test Tour" };
            var user = new User { Id = 2, Email = "test@example.com" };

            // Act
            var booking = new Booking
            {
                Id = 500,
                TourId = 1,
                UserId = 2,
                BookingDate = bookingDate,
                NumberOfPeople = 4,
                TotalAmount = 1500.00m,
                Status = "Confirmed",
                Notes = "Family vacation",
                CreatedDate = createdDate,
                ModifiedDate = modifiedDate,
                IsActive = true,
                CreatedBy = "WebUser",
                ModifiedBy = "AdminUpdate",
                Tour = tour,
                User = user
            };

            // Assert
            Assert.Equal(500, booking.Id);
            Assert.Equal(1, booking.TourId);
            Assert.Equal(2, booking.UserId);
            Assert.Equal(bookingDate, booking.BookingDate);
            Assert.Equal(4, booking.NumberOfPeople);
            Assert.Equal(1500.00m, booking.TotalAmount);
            Assert.Equal("Confirmed", booking.Status);
            Assert.Equal("Family vacation", booking.Notes);
            Assert.Equal(createdDate, booking.CreatedDate);
            Assert.Equal(modifiedDate, booking.ModifiedDate);
            Assert.True(booking.IsActive);
            Assert.Equal("WebUser", booking.CreatedBy);
            Assert.Equal("AdminUpdate", booking.ModifiedBy);
            Assert.Equal(tour, booking.Tour);
            Assert.Equal(user, booking.User);
        }

        [Fact]
        public void Booking_WithZeroNumberOfPeople_ShouldBeValid()
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.NumberOfPeople = 0;

            // Assert
            Assert.Equal(0, booking.NumberOfPeople);
        }

        [Fact]
        public void Booking_WithZeroTotalAmount_ShouldBeValid()
        {
            // Arrange
            var booking = new Booking();

            // Act
            booking.TotalAmount = 0;

            // Assert
            Assert.Equal(0, booking.TotalAmount);
        }
    }
}
