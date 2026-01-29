using Xunit;
using TourManagement.Application.DTOs;

namespace TourManagement.Tests.Application.DTOs;

public class TourDtoTests
{
    [Fact]
    public void TourDto_Constructor_InitializesWithDefaultValues()
    {
        var dto = new TourDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PicturePath);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void TourDto_SetProperties_ShouldStoreValues()
    {
        var dto = new TourDto
        {
            Id = 1,
            Name = "Rome Tour",
            Place = "Rome",
            Days = 5,
            Price = 1200.00m,
            Locations = "Colosseum, Vatican",
            TourInfo = "Historic Rome tour",
            PicturePath = "/images/rome.jpg",
            IsActive = true
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal("Rome Tour", dto.Name);
        Assert.Equal("Rome", dto.Place);
        Assert.Equal(5, dto.Days);
        Assert.Equal(1200.00m, dto.Price);
        Assert.Equal("Colosseum, Vatican", dto.Locations);
        Assert.Equal("Historic Rome tour", dto.TourInfo);
        Assert.Equal("/images/rome.jpg", dto.PicturePath);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void TourCreateDto_Constructor_InitializesWithDefaultValues()
    {
        var dto = new TourCreateDto();

        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PicturePath);
    }

    [Fact]
    public void TourCreateDto_SetProperties_ShouldStoreValues()
    {
        var dto = new TourCreateDto
        {
            Name = "London Tour",
            Place = "London",
            Days = 4,
            Price = 900.50m,
            Locations = "Big Ben, Tower Bridge",
            TourInfo = "Explore London",
            PicturePath = "/images/london.jpg"
        };

        Assert.Equal("London Tour", dto.Name);
        Assert.Equal("London", dto.Place);
        Assert.Equal(4, dto.Days);
        Assert.Equal(900.50m, dto.Price);
        Assert.Equal("Big Ben, Tower Bridge", dto.Locations);
        Assert.Equal("Explore London", dto.TourInfo);
        Assert.Equal("/images/london.jpg", dto.PicturePath);
    }

    [Fact]
    public void TourUpdateDto_Constructor_InitializesWithDefaultValues()
    {
        var dto = new TourUpdateDto();

        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PicturePath);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void TourUpdateDto_SetProperties_ShouldStoreValues()
    {
        var dto = new TourUpdateDto
        {
            Name = "Updated Tour",
            Place = "Barcelona",
            Days = 6,
            Price = 1500.75m,
            Locations = "Sagrada Familia, Park Guell",
            TourInfo = "Updated Barcelona tour",
            PicturePath = "/images/barcelona.jpg",
            IsActive = true
        };

        Assert.Equal("Updated Tour", dto.Name);
        Assert.Equal("Barcelona", dto.Place);
        Assert.Equal(6, dto.Days);
        Assert.Equal(1500.75m, dto.Price);
        Assert.Equal("Sagrada Familia, Park Guell", dto.Locations);
        Assert.Equal("Updated Barcelona tour", dto.TourInfo);
        Assert.Equal("/images/barcelona.jpg", dto.PicturePath);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void TourDto_PicturePath_CanBeNull()
    {
        var dto = new TourDto { PicturePath = null };

        Assert.Null(dto.PicturePath);
    }

    [Fact]
    public void TourCreateDto_PicturePath_CanBeNull()
    {
        var dto = new TourCreateDto { PicturePath = null };

        Assert.Null(dto.PicturePath);
    }

    [Fact]
    public void TourUpdateDto_PicturePath_CanBeNull()
    {
        var dto = new TourUpdateDto { PicturePath = null };

        Assert.Null(dto.PicturePath);
    }
}
