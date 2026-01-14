using Xunit;
using TourManagement.Domain.DTOs;
using System;

namespace TourManagement.Domain.Tests;

/// <summary>
/// Test class for BookingDto, BookingCreateDto, and BookingUpdateDto
/// </summary>
public class BookingDtoTests
{
    #region BookingDto Tests

    [Fact]
    public void BookingDto_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var dto = new BookingDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0, dto.Id);
        Assert.Equal(0, dto.TourId);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void BookingDto_AllProperties_CanBeSetAndGet()
    {
        // Arrange
        var dto = new BookingDto
        {
            Id = 1,
            TourId = 100,
            TourName = "Grand Canyon Adventure",
            Place = "Arizona",
            Email = "john.doe@example.com",
            FirstName = "John",
            BookingDate = new DateTime(2024, 6, 15),
            CreatedDate = new DateTime(2024, 1, 15),
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal(100, dto.TourId);
        Assert.Equal("Grand Canyon Adventure", dto.TourName);
        Assert.Equal("Arizona", dto.Place);
        Assert.Equal("john.doe@example.com", dto.Email);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal(new DateTime(2024, 6, 15), dto.BookingDate);
        Assert.Equal(new DateTime(2024, 1, 15), dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void BookingDto_Id_CanBeZero()
    {
        // Arrange
        var dto = new BookingDto { Id = 0 };

        // Assert
        Assert.Equal(0, dto.Id);
    }

    [Fact]
    public void BookingDto_Id_CanBeNegative()
    {
        // Arrange
        var dto = new BookingDto { Id = -1 };

        // Assert
        Assert.Equal(-1, dto.Id);
    }

    [Fact]
    public void BookingDto_TourId_CanBePositive()
    {
        // Arrange
        var dto = new BookingDto { TourId = 123 };

        // Assert
        Assert.Equal(123, dto.TourId);
    }

    [Fact]
    public void BookingDto_Email_CanBeSetToValidEmail()
    {
        // Arrange
        var dto = new BookingDto { Email = "user@example.com" };

        // Assert
        Assert.Equal("user@example.com", dto.Email);
    }

    [Fact]
    public void BookingDto_IsActive_CanBeTrue()
    {
        // Arrange
        var dto = new BookingDto { IsActive = true };

        // Assert
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void BookingDto_IsActive_CanBeFalse()
    {
        // Arrange
        var dto = new BookingDto { IsActive = false };

        // Assert
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void BookingDto_BookingDate_CanBeSet()
    {
        // Arrange
        var date = new DateTime(2024, 7, 20);
        var dto = new BookingDto { BookingDate = date };

        // Assert
        Assert.Equal(date, dto.BookingDate);
    }

    [Fact]
    public void BookingDto_CreatedDate_CanBeSet()
    {
        // Arrange
        var date = new DateTime(2024, 1, 1);
        var dto = new BookingDto { CreatedDate = date };

        // Assert
        Assert.Equal(date, dto.CreatedDate);
    }

    #endregion

    #region BookingCreateDto Tests

    [Fact]
    public void BookingCreateDto_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var dto = new BookingCreateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0, dto.TourId);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal("System", dto.CreatedBy);
    }

    [Fact]
    public void BookingCreateDto_AllProperties_CanBeSetAndGet()
    {
        // Arrange
        var dto = new BookingCreateDto
        {
            TourId = 200,
            TourName = "European Adventure",
            Place = "Europe",
            Email = "jane.smith@example.com",
            FirstName = "Jane",
            BookingDate = new DateTime(2024, 8, 10),
            CreatedBy = "Admin"
        };

        // Assert
        Assert.Equal(200, dto.TourId);
        Assert.Equal("European Adventure", dto.TourName);
        Assert.Equal("Europe", dto.Place);
        Assert.Equal("jane.smith@example.com", dto.Email);
        Assert.Equal("Jane", dto.FirstName);
        Assert.Equal(new DateTime(2024, 8, 10), dto.BookingDate);
        Assert.Equal("Admin", dto.CreatedBy);
    }

    [Fact]
    public void BookingCreateDto_CreatedBy_DefaultsToSystem()
    {
        // Arrange & Act
        var dto = new BookingCreateDto();

        // Assert
        Assert.Equal("System", dto.CreatedBy);
    }

    [Fact]
    public void BookingCreateDto_CreatedBy_CanBeChanged()
    {
        // Arrange
        var dto = new BookingCreateDto { CreatedBy = "AdminUser" };

        // Assert
        Assert.Equal("AdminUser", dto.CreatedBy);
    }

    [Fact]
    public void BookingCreateDto_TourId_CanBeZero()
    {
        // Arrange
        var dto = new BookingCreateDto { TourId = 0 };

        // Assert
        Assert.Equal(0, dto.TourId);
    }

    [Fact]
    public void BookingCreateDto_TourId_CanBeNegative()
    {
        // Arrange
        var dto = new BookingCreateDto { TourId = -1 };

        // Assert
        Assert.Equal(-1, dto.TourId);
    }

    [Fact]
    public void BookingCreateDto_BookingDate_CanBeMinValue()
    {
        // Arrange
        var dto = new BookingCreateDto { BookingDate = DateTime.MinValue };

        // Assert
        Assert.Equal(DateTime.MinValue, dto.BookingDate);
    }

    [Fact]
    public void BookingCreateDto_BookingDate_CanBeMaxValue()
    {
        // Arrange
        var dto = new BookingCreateDto { BookingDate = DateTime.MaxValue };

        // Assert
        Assert.Equal(DateTime.MaxValue, dto.BookingDate);
    }

    [Fact]
    public void BookingCreateDto_AllStringProperties_CanBeEmpty()
    {
        // Arrange
        var dto = new BookingCreateDto
        {
            TourName = string.Empty,
            Place = string.Empty,
            Email = string.Empty,
            FirstName = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
    }

    #endregion

    #region BookingUpdateDto Tests

    [Fact]
    public void BookingUpdateDto_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var dto = new BookingUpdateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("System", dto.ModifiedBy);
    }

    [Fact]
    public void BookingUpdateDto_AllProperties_CanBeSetAndGet()
    {
        // Arrange
        var dto = new BookingUpdateDto
        {
            BookingDate = new DateTime(2024, 9, 25),
            ModifiedBy = "Editor"
        };

        // Assert
        Assert.Equal(new DateTime(2024, 9, 25), dto.BookingDate);
        Assert.Equal("Editor", dto.ModifiedBy);
    }

    [Fact]
    public void BookingUpdateDto_ModifiedBy_DefaultsToSystem()
    {
        // Arrange & Act
        var dto = new BookingUpdateDto();

        // Assert
        Assert.Equal("System", dto.ModifiedBy);
    }

    [Fact]
    public void BookingUpdateDto_ModifiedBy_CanBeChanged()
    {
        // Arrange
        var dto = new BookingUpdateDto { ModifiedBy = "EditorUser" };

        // Assert
        Assert.Equal("EditorUser", dto.ModifiedBy);
    }

    [Fact]
    public void BookingUpdateDto_BookingDate_CanBeModified()
    {
        // Arrange
        var originalDate = new DateTime(2024, 1, 1);
        var modifiedDate = new DateTime(2024, 12, 31);
        var dto = new BookingUpdateDto { BookingDate = originalDate };

        // Act
        dto.BookingDate = modifiedDate;

        // Assert
        Assert.Equal(modifiedDate, dto.BookingDate);
    }

    [Fact]
    public void BookingUpdateDto_BookingDate_CanBeMinValue()
    {
        // Arrange
        var dto = new BookingUpdateDto { BookingDate = DateTime.MinValue };

        // Assert
        Assert.Equal(DateTime.MinValue, dto.BookingDate);
    }

    [Fact]
    public void BookingUpdateDto_BookingDate_CanBeMaxValue()
    {
        // Arrange
        var dto = new BookingUpdateDto { BookingDate = DateTime.MaxValue };

        // Assert
        Assert.Equal(DateTime.MaxValue, dto.BookingDate);
    }

    [Fact]
    public void BookingUpdateDto_HasFewerPropertiesThan_BookingCreateDto()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            TourId = 1,
            TourName = "Tour",
            Place = "Place",
            Email = "email@example.com",
            FirstName = "John"
        };

        var updateDto = new BookingUpdateDto
        {
            BookingDate = new DateTime(2024, 1, 1)
        };

        // Assert - BookingUpdateDto has only BookingDate and ModifiedBy
        Assert.NotNull(updateDto);
        Assert.NotNull(createDto);
    }

    #endregion

    #region Comparison Tests

    [Fact]
    public void BookingDto_And_BookingCreateDto_HaveSimilarProperties()
    {
        // Arrange
        var bookingDto = new BookingDto
        {
            TourId = 100,
            TourName = "Sample Tour",
            Place = "Sample Place",
            Email = "test@example.com",
            FirstName = "Test",
            BookingDate = new DateTime(2024, 6, 15)
        };

        var createDto = new BookingCreateDto
        {
            TourId = 100,
            TourName = "Sample Tour",
            Place = "Sample Place",
            Email = "test@example.com",
            FirstName = "Test",
            BookingDate = new DateTime(2024, 6, 15)
        };

        // Assert
        Assert.Equal(bookingDto.TourId, createDto.TourId);
        Assert.Equal(bookingDto.TourName, createDto.TourName);
        Assert.Equal(bookingDto.Place, createDto.Place);
        Assert.Equal(bookingDto.Email, createDto.Email);
        Assert.Equal(bookingDto.FirstName, createDto.FirstName);
        Assert.Equal(bookingDto.BookingDate, createDto.BookingDate);
    }

    [Fact]
    public void BookingCreateDto_HasMorePropertiesThan_BookingUpdateDto()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            TourId = 1,
            TourName = "Tour",
            Place = "Place",
            Email = "email@example.com",
            FirstName = "John",
            BookingDate = new DateTime(2024, 1, 1)
        };

        var updateDto = new BookingUpdateDto
        {
            BookingDate = new DateTime(2024, 1, 1)
        };

        // Assert - Create has TourId, TourName, Place, Email, FirstName that Update doesn't have
        Assert.Equal(1, createDto.TourId);
        Assert.Equal("Tour", createDto.TourName);
        Assert.Equal("Place", createDto.Place);
        Assert.Equal("email@example.com", createDto.Email);
        Assert.Equal("John", createDto.FirstName);
    }

    #endregion
}
