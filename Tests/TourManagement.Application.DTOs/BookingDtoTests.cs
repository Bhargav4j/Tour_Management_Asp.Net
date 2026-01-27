using Xunit;
using System;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.DTOs.Tests;

public class BookingDtoTests
{
    [Fact]
    public void BookingDto_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var dto = new BookingDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0, dto.Id);
        Assert.Equal(0, dto.UserId);
        Assert.Equal(0, dto.TourId);
        Assert.Equal(default(DateTime), dto.BookingDate);
        Assert.Equal(0, dto.NumberOfPeople);
        Assert.Equal(0m, dto.TotalAmount);
        Assert.Equal(string.Empty, dto.Status);
        Assert.Equal(default(DateTime), dto.CreatedDate);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void BookingDto_SetAllProperties_SetsCorrectly()
    {
        // Arrange
        var dto = new BookingDto
        {
            Id = 1,
            UserId = 10,
            TourId = 20,
            BookingDate = DateTime.Now,
            NumberOfPeople = 4,
            TotalAmount = 4000m,
            Status = "Confirmed",
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal(10, dto.UserId);
        Assert.Equal(20, dto.TourId);
        Assert.NotNull(dto.BookingDate);
        Assert.Equal(4, dto.NumberOfPeople);
        Assert.Equal(4000m, dto.TotalAmount);
        Assert.Equal("Confirmed", dto.Status);
        Assert.NotNull(dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void BookingDto_Status_CanBeEmpty()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.Status = string.Empty;

        // Assert
        Assert.Equal(string.Empty, dto.Status);
    }

    [Fact]
    public void BookingDto_SetNumberOfPeople_WithZero_SetsCorrectly()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.NumberOfPeople = 0;

        // Assert
        Assert.Equal(0, dto.NumberOfPeople);
    }

    [Fact]
    public void BookingDto_SetTotalAmount_WithZero_SetsCorrectly()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.TotalAmount = 0m;

        // Assert
        Assert.Equal(0m, dto.TotalAmount);
    }
}

public class BookingCreateDtoTests
{
    [Fact]
    public void BookingCreateDto_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var dto = new BookingCreateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0, dto.UserId);
        Assert.Equal(0, dto.TourId);
        Assert.Equal(default(DateTime), dto.BookingDate);
        Assert.Equal(0, dto.NumberOfPeople);
        Assert.Equal(0m, dto.TotalAmount);
        Assert.Equal("Pending", dto.Status);
    }

    [Fact]
    public void BookingCreateDto_SetAllProperties_SetsCorrectly()
    {
        // Arrange
        var dto = new BookingCreateDto
        {
            UserId = 5,
            TourId = 15,
            BookingDate = DateTime.Now,
            NumberOfPeople = 3,
            TotalAmount = 3000m,
            Status = "Confirmed"
        };

        // Assert
        Assert.Equal(5, dto.UserId);
        Assert.Equal(15, dto.TourId);
        Assert.NotNull(dto.BookingDate);
        Assert.Equal(3, dto.NumberOfPeople);
        Assert.Equal(3000m, dto.TotalAmount);
        Assert.Equal("Confirmed", dto.Status);
    }

    [Fact]
    public void BookingCreateDto_Status_DefaultsToPending()
    {
        // Arrange & Act
        var dto = new BookingCreateDto();

        // Assert
        Assert.Equal("Pending", dto.Status);
    }

    [Fact]
    public void BookingCreateDto_SetUserId_WithNegativeValue_SetsValue()
    {
        // Arrange
        var dto = new BookingCreateDto();

        // Act
        dto.UserId = -1;

        // Assert
        Assert.Equal(-1, dto.UserId);
    }

    [Fact]
    public void BookingCreateDto_SetTourId_WithNegativeValue_SetsValue()
    {
        // Arrange
        var dto = new BookingCreateDto();

        // Act
        dto.TourId = -1;

        // Assert
        Assert.Equal(-1, dto.TourId);
    }

    [Fact]
    public void BookingCreateDto_SetNumberOfPeople_WithNegativeValue_SetsValue()
    {
        // Arrange
        var dto = new BookingCreateDto();

        // Act
        dto.NumberOfPeople = -5;

        // Assert
        Assert.Equal(-5, dto.NumberOfPeople);
    }

    [Fact]
    public void BookingCreateDto_SetTotalAmount_WithNegativeValue_SetsValue()
    {
        // Arrange
        var dto = new BookingCreateDto();

        // Act
        dto.TotalAmount = -1000m;

        // Assert
        Assert.Equal(-1000m, dto.TotalAmount);
    }

    [Fact]
    public void BookingCreateDto_SetBookingDate_WithFutureDate_SetsCorrectly()
    {
        // Arrange
        var dto = new BookingCreateDto();
        var futureDate = DateTime.Now.AddDays(30);

        // Act
        dto.BookingDate = futureDate;

        // Assert
        Assert.Equal(futureDate, dto.BookingDate);
    }
}

public class BookingUpdateDtoTests
{
    [Fact]
    public void BookingUpdateDto_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var dto = new BookingUpdateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(default(DateTime), dto.BookingDate);
        Assert.Equal(0, dto.NumberOfPeople);
        Assert.Equal(0m, dto.TotalAmount);
        Assert.Equal(string.Empty, dto.Status);
    }

    [Fact]
    public void BookingUpdateDto_SetAllProperties_SetsCorrectly()
    {
        // Arrange
        var dto = new BookingUpdateDto
        {
            BookingDate = DateTime.Now,
            NumberOfPeople = 6,
            TotalAmount = 6000m,
            Status = "Cancelled"
        };

        // Assert
        Assert.NotNull(dto.BookingDate);
        Assert.Equal(6, dto.NumberOfPeople);
        Assert.Equal(6000m, dto.TotalAmount);
        Assert.Equal("Cancelled", dto.Status);
    }

    [Fact]
    public void BookingUpdateDto_Status_CanBeEmpty()
    {
        // Arrange
        var dto = new BookingUpdateDto();

        // Act
        dto.Status = string.Empty;

        // Assert
        Assert.Equal(string.Empty, dto.Status);
    }

    [Fact]
    public void BookingUpdateDto_SetNumberOfPeople_WithMaxValue_SetsCorrectly()
    {
        // Arrange
        var dto = new BookingUpdateDto();

        // Act
        dto.NumberOfPeople = int.MaxValue;

        // Assert
        Assert.Equal(int.MaxValue, dto.NumberOfPeople);
    }

    [Fact]
    public void BookingUpdateDto_SetTotalAmount_WithMaxValue_SetsCorrectly()
    {
        // Arrange
        var dto = new BookingUpdateDto();

        // Act
        dto.TotalAmount = decimal.MaxValue;

        // Assert
        Assert.Equal(decimal.MaxValue, dto.TotalAmount);
    }

    [Fact]
    public void BookingUpdateDto_SetBookingDate_WithPastDate_SetsCorrectly()
    {
        // Arrange
        var dto = new BookingUpdateDto();
        var pastDate = DateTime.Now.AddDays(-30);

        // Act
        dto.BookingDate = pastDate;

        // Assert
        Assert.Equal(pastDate, dto.BookingDate);
    }
}
