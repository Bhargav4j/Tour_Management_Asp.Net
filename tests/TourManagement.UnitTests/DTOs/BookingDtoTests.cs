using TourManagement.Domain.DTOs;
using Xunit;

namespace TourManagement.UnitTests.DTOs;

public class BookingDtoTests
{
    [Fact]
    public void BookingDto_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var dto = new BookingDto();

        // Assert
        Assert.Equal(0, dto.Id);
        Assert.Equal(0, dto.UserId);
        Assert.Equal(0, dto.TourId);
        Assert.Equal(0, dto.NumberOfPeople);
        Assert.Equal(0, dto.TotalAmount);
        Assert.Equal("Pending", dto.Status);
        Assert.Equal(string.Empty, dto.UserName);
        Assert.Equal(string.Empty, dto.TourName);
    }

    [Fact]
    public void BookingDto_Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new BookingDto();
        var bookingDate = DateTime.UtcNow;
        var createdDate = DateTime.UtcNow;

        // Act
        dto.Id = 1;
        dto.UserId = 10;
        dto.TourId = 5;
        dto.BookingDate = bookingDate;
        dto.NumberOfPeople = 3;
        dto.TotalAmount = 4500.00m;
        dto.Status = "Confirmed";
        dto.CreatedDate = createdDate;
        dto.UserName = "John Doe";
        dto.TourName = "Paris Adventure";

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal(10, dto.UserId);
        Assert.Equal(5, dto.TourId);
        Assert.Equal(bookingDate, dto.BookingDate);
        Assert.Equal(3, dto.NumberOfPeople);
        Assert.Equal(4500.00m, dto.TotalAmount);
        Assert.Equal("Confirmed", dto.Status);
        Assert.Equal(createdDate, dto.CreatedDate);
        Assert.Equal("John Doe", dto.UserName);
        Assert.Equal("Paris Adventure", dto.TourName);
    }

    [Fact]
    public void BookingCreateDto_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var dto = new BookingCreateDto();

        // Assert
        Assert.Equal(0, dto.UserId);
        Assert.Equal(0, dto.TourId);
        Assert.Equal(0, dto.NumberOfPeople);
        Assert.Equal(0, dto.TotalAmount);
    }

    [Fact]
    public void BookingCreateDto_Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new BookingCreateDto();
        var bookingDate = DateTime.UtcNow;

        // Act
        dto.UserId = 20;
        dto.TourId = 8;
        dto.BookingDate = bookingDate;
        dto.NumberOfPeople = 2;
        dto.TotalAmount = 3000.00m;

        // Assert
        Assert.Equal(20, dto.UserId);
        Assert.Equal(8, dto.TourId);
        Assert.Equal(bookingDate, dto.BookingDate);
        Assert.Equal(2, dto.NumberOfPeople);
        Assert.Equal(3000.00m, dto.TotalAmount);
    }

    [Fact]
    public void BookingUpdateDto_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var dto = new BookingUpdateDto();

        // Assert
        Assert.Equal(0, dto.NumberOfPeople);
        Assert.Equal(0, dto.TotalAmount);
        Assert.Equal("Pending", dto.Status);
    }

    [Fact]
    public void BookingUpdateDto_Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new BookingUpdateDto();
        var bookingDate = DateTime.UtcNow;

        // Act
        dto.BookingDate = bookingDate;
        dto.NumberOfPeople = 5;
        dto.TotalAmount = 7500.00m;
        dto.Status = "Cancelled";

        // Assert
        Assert.Equal(bookingDate, dto.BookingDate);
        Assert.Equal(5, dto.NumberOfPeople);
        Assert.Equal(7500.00m, dto.TotalAmount);
        Assert.Equal("Cancelled", dto.Status);
    }

    [Fact]
    public void BookingDto_Status_DefaultShouldBePending()
    {
        // Arrange & Act
        var dto = new BookingDto();

        // Assert
        Assert.Equal("Pending", dto.Status);
    }

    [Fact]
    public void BookingUpdateDto_Status_DefaultShouldBePending()
    {
        // Arrange & Act
        var dto = new BookingUpdateDto();

        // Assert
        Assert.Equal("Pending", dto.Status);
    }
}
