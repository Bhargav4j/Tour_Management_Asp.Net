using Xunit;
using TourManagement.Application.DTOs;

namespace TourManagement.Tests.Application.DTOs;

public class BookingDtoTests
{
    [Fact]
    public void BookingDto_Constructor_InitializesWithDefaultValues()
    {
        var dto = new BookingDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(0, dto.TourId);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.UserEmail);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void BookingDto_SetProperties_ShouldStoreValues()
    {
        var dto = new BookingDto
        {
            Id = 1,
            TourId = 10,
            TourName = "Paris Adventure",
            Place = "Paris",
            UserEmail = "customer@example.com",
            FirstName = "Charlie",
            BookingDate = new DateTime(2024, 7, 1),
            IsActive = true
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal(10, dto.TourId);
        Assert.Equal("Paris Adventure", dto.TourName);
        Assert.Equal("Paris", dto.Place);
        Assert.Equal("customer@example.com", dto.UserEmail);
        Assert.Equal("Charlie", dto.FirstName);
        Assert.Equal(new DateTime(2024, 7, 1), dto.BookingDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void BookingCreateDto_Constructor_InitializesWithDefaultValues()
    {
        var dto = new BookingCreateDto();

        Assert.Equal(0, dto.TourId);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.UserEmail);
        Assert.Equal(string.Empty, dto.FirstName);
    }

    [Fact]
    public void BookingCreateDto_SetProperties_ShouldStoreValues()
    {
        var dto = new BookingCreateDto
        {
            TourId = 20,
            TourName = "Tokyo Tour",
            Place = "Tokyo",
            UserEmail = "traveler@example.com",
            FirstName = "Diana"
        };

        Assert.Equal(20, dto.TourId);
        Assert.Equal("Tokyo Tour", dto.TourName);
        Assert.Equal("Tokyo", dto.Place);
        Assert.Equal("traveler@example.com", dto.UserEmail);
        Assert.Equal("Diana", dto.FirstName);
    }

    [Fact]
    public void BookingDto_BookingDate_CanBeSet()
    {
        var dto = new BookingDto();
        var bookingDate = DateTime.UtcNow;

        dto.BookingDate = bookingDate;

        Assert.Equal(bookingDate, dto.BookingDate);
    }

    [Fact]
    public void BookingDto_IsActive_DefaultsToFalse()
    {
        var dto = new BookingDto();

        Assert.False(dto.IsActive);
    }

    [Fact]
    public void BookingDto_IsActive_CanBeSetToTrue()
    {
        var dto = new BookingDto { IsActive = true };

        Assert.True(dto.IsActive);
    }
}
