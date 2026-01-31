using System;
using Xunit;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.DTOs.Tests
{
    /// <summary>
    /// Tests for BookingDto
    /// </summary>
    public class BookingDtoTests
    {
        [Fact]
        public void BookingDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new BookingDto();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal(0, dto.TourId);
            Assert.Equal(0, dto.UserId);
            Assert.Null(dto.TourName);
            Assert.Null(dto.UserEmail);
            Assert.Equal(0, dto.NumberOfPeople);
            Assert.Equal(0, dto.TotalAmount);
            Assert.Equal("Pending", dto.Status);
            Assert.Null(dto.Notes);
        }

        [Fact]
        public void BookingDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new BookingDto();
            var bookingDate = DateTime.Now;
            var createdDate = DateTime.Now;

            // Act
            dto.Id = 100;
            dto.TourId = 10;
            dto.UserId = 20;
            dto.TourName = "Paris Tour";
            dto.UserEmail = "user@test.com";
            dto.BookingDate = bookingDate;
            dto.NumberOfPeople = 4;
            dto.TotalAmount = 2000.00m;
            dto.Status = "Confirmed";
            dto.Notes = "Test notes";
            dto.CreatedDate = createdDate;

            // Assert
            Assert.Equal(100, dto.Id);
            Assert.Equal(10, dto.TourId);
            Assert.Equal(20, dto.UserId);
            Assert.Equal("Paris Tour", dto.TourName);
            Assert.Equal("user@test.com", dto.UserEmail);
            Assert.Equal(bookingDate, dto.BookingDate);
            Assert.Equal(4, dto.NumberOfPeople);
            Assert.Equal(2000.00m, dto.TotalAmount);
            Assert.Equal("Confirmed", dto.Status);
            Assert.Equal("Test notes", dto.Notes);
            Assert.Equal(createdDate, dto.CreatedDate);
        }

        [Fact]
        public void BookingDto_NullableProperties_CanBeNull()
        {
            // Arrange
            var dto = new BookingDto();

            // Act
            dto.TourName = null;
            dto.UserEmail = null;
            dto.Notes = null;

            // Assert
            Assert.Null(dto.TourName);
            Assert.Null(dto.UserEmail);
            Assert.Null(dto.Notes);
        }

        [Theory]
        [InlineData("Pending")]
        [InlineData("Confirmed")]
        [InlineData("Cancelled")]
        public void BookingDto_Status_ShouldAcceptVariousValues(string status)
        {
            // Arrange
            var dto = new BookingDto();

            // Act
            dto.Status = status;

            // Assert
            Assert.Equal(status, dto.Status);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void BookingDto_NumberOfPeople_ShouldAcceptVariousValues(int numberOfPeople)
        {
            // Arrange
            var dto = new BookingDto();

            // Act
            dto.NumberOfPeople = numberOfPeople;

            // Assert
            Assert.Equal(numberOfPeople, dto.NumberOfPeople);
        }

        [Theory]
        [InlineData(100.00)]
        [InlineData(0)]
        [InlineData(9999.99)]
        public void BookingDto_TotalAmount_ShouldAcceptVariousValues(decimal totalAmount)
        {
            // Arrange
            var dto = new BookingDto();

            // Act
            dto.TotalAmount = totalAmount;

            // Assert
            Assert.Equal(totalAmount, dto.TotalAmount);
        }
    }

    /// <summary>
    /// Tests for BookingCreateDto
    /// </summary>
    public class BookingCreateDtoTests
    {
        [Fact]
        public void BookingCreateDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new BookingCreateDto();

            // Assert
            Assert.Equal(0, dto.TourId);
            Assert.Equal(0, dto.UserId);
            Assert.Equal(0, dto.NumberOfPeople);
            Assert.Equal(0, dto.TotalAmount);
            Assert.Null(dto.Notes);
        }

        [Fact]
        public void BookingCreateDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new BookingCreateDto();
            var bookingDate = DateTime.Now;

            // Act
            dto.TourId = 5;
            dto.UserId = 15;
            dto.BookingDate = bookingDate;
            dto.NumberOfPeople = 3;
            dto.TotalAmount = 1500.00m;
            dto.Notes = "Special requirements";

            // Assert
            Assert.Equal(5, dto.TourId);
            Assert.Equal(15, dto.UserId);
            Assert.Equal(bookingDate, dto.BookingDate);
            Assert.Equal(3, dto.NumberOfPeople);
            Assert.Equal(1500.00m, dto.TotalAmount);
            Assert.Equal("Special requirements", dto.Notes);
        }

        [Fact]
        public void BookingCreateDto_Notes_CanBeNull()
        {
            // Arrange
            var dto = new BookingCreateDto();

            // Act
            dto.Notes = null;

            // Assert
            Assert.Null(dto.Notes);
        }

        [Fact]
        public void BookingCreateDto_WithObjectInitializer_ShouldSetAllProperties()
        {
            // Arrange
            var bookingDate = DateTime.Now;

            // Act
            var dto = new BookingCreateDto
            {
                TourId = 1,
                UserId = 2,
                BookingDate = bookingDate,
                NumberOfPeople = 2,
                TotalAmount = 800.00m,
                Notes = "Honeymoon package"
            };

            // Assert
            Assert.Equal(1, dto.TourId);
            Assert.Equal(2, dto.UserId);
            Assert.Equal(bookingDate, dto.BookingDate);
            Assert.Equal(2, dto.NumberOfPeople);
            Assert.Equal(800.00m, dto.TotalAmount);
            Assert.Equal("Honeymoon package", dto.Notes);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(20)]
        public void BookingCreateDto_NumberOfPeople_ShouldAcceptVariousValues(int numberOfPeople)
        {
            // Arrange
            var dto = new BookingCreateDto();

            // Act
            dto.NumberOfPeople = numberOfPeople;

            // Assert
            Assert.Equal(numberOfPeople, dto.NumberOfPeople);
        }

        [Theory]
        [InlineData(50.00)]
        [InlineData(500.00)]
        [InlineData(5000.00)]
        public void BookingCreateDto_TotalAmount_ShouldAcceptVariousValues(decimal totalAmount)
        {
            // Arrange
            var dto = new BookingCreateDto();

            // Act
            dto.TotalAmount = totalAmount;

            // Assert
            Assert.Equal(totalAmount, dto.TotalAmount);
        }
    }

    /// <summary>
    /// Tests for BookingUpdateDto
    /// </summary>
    public class BookingUpdateDtoTests
    {
        [Fact]
        public void BookingUpdateDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new BookingUpdateDto();

            // Assert
            Assert.Equal(0, dto.NumberOfPeople);
            Assert.Equal(0, dto.TotalAmount);
            Assert.Equal("Pending", dto.Status);
            Assert.Null(dto.Notes);
        }

        [Fact]
        public void BookingUpdateDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new BookingUpdateDto();
            var bookingDate = DateTime.Now;

            // Act
            dto.BookingDate = bookingDate;
            dto.NumberOfPeople = 6;
            dto.TotalAmount = 3000.00m;
            dto.Status = "Confirmed";
            dto.Notes = "Updated notes";

            // Assert
            Assert.Equal(bookingDate, dto.BookingDate);
            Assert.Equal(6, dto.NumberOfPeople);
            Assert.Equal(3000.00m, dto.TotalAmount);
            Assert.Equal("Confirmed", dto.Status);
            Assert.Equal("Updated notes", dto.Notes);
        }

        [Fact]
        public void BookingUpdateDto_Notes_CanBeNull()
        {
            // Arrange
            var dto = new BookingUpdateDto();

            // Act
            dto.Notes = null;

            // Assert
            Assert.Null(dto.Notes);
        }

        [Theory]
        [InlineData("Pending")]
        [InlineData("Confirmed")]
        [InlineData("Cancelled")]
        [InlineData("Completed")]
        public void BookingUpdateDto_Status_ShouldAcceptVariousValues(string status)
        {
            // Arrange
            var dto = new BookingUpdateDto();

            // Act
            dto.Status = status;

            // Assert
            Assert.Equal(status, dto.Status);
        }

        [Fact]
        public void BookingUpdateDto_WithObjectInitializer_ShouldSetAllProperties()
        {
            // Arrange
            var bookingDate = DateTime.Now;

            // Act
            var dto = new BookingUpdateDto
            {
                BookingDate = bookingDate,
                NumberOfPeople = 8,
                TotalAmount = 4000.00m,
                Status = "Completed",
                Notes = "All good"
            };

            // Assert
            Assert.Equal(bookingDate, dto.BookingDate);
            Assert.Equal(8, dto.NumberOfPeople);
            Assert.Equal(4000.00m, dto.TotalAmount);
            Assert.Equal("Completed", dto.Status);
            Assert.Equal("All good", dto.Notes);
        }

        [Fact]
        public void BookingUpdateDto_DefaultStatus_ShouldBePending()
        {
            // Arrange & Act
            var dto = new BookingUpdateDto();

            // Assert
            Assert.Equal("Pending", dto.Status);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(50)]
        public void BookingUpdateDto_NumberOfPeople_ShouldAcceptVariousValues(int numberOfPeople)
        {
            // Arrange
            var dto = new BookingUpdateDto();

            // Act
            dto.NumberOfPeople = numberOfPeople;

            // Assert
            Assert.Equal(numberOfPeople, dto.NumberOfPeople);
        }
    }
}
