using Xunit;
using TourManagement.Application.DTOs;
using System;

namespace TourManagement.Application.DTOs.Tests;

public class BookingDtoTests
{
    [Fact]
    public void BookingDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new BookingDto();
        var date = DateTime.Now;

        // Act
        dto.Id = 1;
        dto.UserId = 10;
        dto.TourId = 20;
        dto.UserName = "John Doe";
        dto.TourName = "Paris Tour";
        dto.BookingDate = date;
        dto.NumberOfPersons = 3;
        dto.TotalAmount = 3000;
        dto.Status = "Confirmed";
        dto.CreatedDate = date;
        dto.IsActive = true;

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal(10, dto.UserId);
        Assert.Equal(20, dto.TourId);
        Assert.Equal("John Doe", dto.UserName);
        Assert.Equal("Paris Tour", dto.TourName);
        Assert.Equal(date, dto.BookingDate);
        Assert.Equal(3, dto.NumberOfPersons);
        Assert.Equal(3000, dto.TotalAmount);
        Assert.Equal("Confirmed", dto.Status);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void BookingCreateDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new BookingCreateDto();
        var date = DateTime.Now;

        // Act
        dto.UserId = 10;
        dto.TourId = 20;
        dto.BookingDate = date;
        dto.NumberOfPersons = 2;
        dto.TotalAmount = 2000;

        // Assert
        Assert.Equal(10, dto.UserId);
        Assert.Equal(20, dto.TourId);
        Assert.Equal(date, dto.BookingDate);
        Assert.Equal(2, dto.NumberOfPersons);
        Assert.Equal(2000, dto.TotalAmount);
    }

    [Fact]
    public void BookingUpdateDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new BookingUpdateDto();
        var date = DateTime.Now;

        // Act
        dto.BookingDate = date;
        dto.NumberOfPersons = 3;
        dto.TotalAmount = 3000;
        dto.Status = "Confirmed";

        // Assert
        Assert.Equal(date, dto.BookingDate);
        Assert.Equal(3, dto.NumberOfPersons);
        Assert.Equal(3000, dto.TotalAmount);
        Assert.Equal("Confirmed", dto.Status);
    }
}
