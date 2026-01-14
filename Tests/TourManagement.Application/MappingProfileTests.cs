using Xunit;
using AutoMapper;
using TourManagement.Application.Mappings;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using System;

namespace TourManagement.Application.Tests;

/// <summary>
/// Test class for MappingProfile
/// </summary>
public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void MappingProfile_Configuration_IsValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());

        // Act & Assert
        configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_TourToTourDto_MapsAllProperties()
    {
        // Arrange
        var tour = new Tour
        {
            Id = 1,
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 500m,
            Locations = "Location1, Location2",
            TourInfo = "Test Info",
            PictureUrl = "https://example.com/tour.jpg",
            CreatedDate = new DateTime(2024, 1, 1),
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<TourDto>(tour);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(1, dto.Id);
        Assert.Equal("Test Tour", dto.TourName);
        Assert.Equal("Test Place", dto.Place);
        Assert.Equal(5, dto.Days);
        Assert.Equal(500m, dto.Price);
        Assert.Equal("Location1, Location2", dto.Locations);
        Assert.Equal("Test Info", dto.TourInfo);
        Assert.Equal("https://example.com/tour.jpg", dto.PictureUrl);
        Assert.Equal(new DateTime(2024, 1, 1), dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void Map_TourCreateDtoToTour_MapsAndSetsDefaults()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "New Tour",
            Place = "New Place",
            Days = 7,
            Price = 700m,
            Locations = "Loc1, Loc2",
            TourInfo = "New Info",
            PictureUrl = "https://example.com/new.jpg",
            CreatedBy = "Admin"
        };

        // Act
        var tour = _mapper.Map<Tour>(createDto);

        // Assert
        Assert.NotNull(tour);
        Assert.Equal("New Tour", tour.TourName);
        Assert.Equal("New Place", tour.Place);
        Assert.Equal(7, tour.Days);
        Assert.Equal(700m, tour.Price);
        Assert.Equal("Loc1, Loc2", tour.Locations);
        Assert.Equal("New Info", tour.TourInfo);
        Assert.Equal("https://example.com/new.jpg", tour.PictureUrl);
        Assert.True(tour.IsActive);
        Assert.NotEqual(DateTime.MinValue, tour.CreatedDate);
    }

    [Fact]
    public void Map_TourUpdateDtoToTour_UpdatesProperties()
    {
        // Arrange
        var updateDto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Updated Place",
            Days = 10,
            Price = 1000m,
            Locations = "UpdatedLoc1, UpdatedLoc2",
            TourInfo = "Updated Info",
            PictureUrl = "https://example.com/updated.jpg",
            ModifiedBy = "Editor"
        };

        var existingTour = new Tour { Id = 1, TourName = "Original Tour" };

        // Act
        _mapper.Map(updateDto, existingTour);

        // Assert
        Assert.Equal("Updated Tour", existingTour.TourName);
        Assert.Equal("Updated Place", existingTour.Place);
        Assert.Equal(10, existingTour.Days);
        Assert.Equal(1000m, existingTour.Price);
    }

    [Fact]
    public void Map_UserInfoToUserDto_MapsAllProperties()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            DateOfBirth = new DateTime(1990, 5, 15),
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            CreatedDate = new DateTime(2024, 1, 1),
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<UserDto>(userInfo);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal(new DateTime(1990, 5, 15), dto.DateOfBirth);
        Assert.Equal("123 Main St", dto.Street);
        Assert.Equal("New York", dto.City);
        Assert.Equal("NY", dto.State);
        Assert.Equal(new DateTime(2024, 1, 1), dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void Map_UserCreateDtoToUserInfo_MapsAndSetsDefaults()
    {
        // Arrange
        var createDto = new UserCreateDto
        {
            Email = "new@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Gender = "Female",
            Password = "Password123",
            DateOfBirth = new DateTime(1992, 8, 25),
            Street = "456 Oak Ave",
            City = "Los Angeles",
            State = "CA"
        };

        // Act
        var userInfo = _mapper.Map<UserInfo>(createDto);

        // Assert
        Assert.NotNull(userInfo);
        Assert.Equal("new@example.com", userInfo.Email);
        Assert.Equal("Jane", userInfo.FirstName);
        Assert.Equal("Smith", userInfo.LastName);
        Assert.Equal("Female", userInfo.Gender);
        Assert.Equal(new DateTime(1992, 8, 25), userInfo.DateOfBirth);
        Assert.Equal("456 Oak Ave", userInfo.Street);
        Assert.Equal("Los Angeles", userInfo.City);
        Assert.Equal("CA", userInfo.State);
        Assert.True(userInfo.IsActive);
        Assert.NotEqual(DateTime.MinValue, userInfo.CreatedDate);
    }

    [Fact]
    public void Map_UserCreateDtoToUserInfo_IgnoresPasswordHash()
    {
        // Arrange
        var createDto = new UserCreateDto
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        // Act
        var userInfo = _mapper.Map<UserInfo>(createDto);

        // Assert
        Assert.Equal(string.Empty, userInfo.PasswordHash);
    }

    [Fact]
    public void Map_UserUpdateDtoToUserInfo_UpdatesProperties()
    {
        // Arrange
        var updateDto = new UserUpdateDto
        {
            FirstName = "UpdatedJohn",
            LastName = "UpdatedDoe",
            Gender = "Male",
            DateOfBirth = new DateTime(1991, 6, 20),
            Street = "789 Pine Rd",
            City = "Chicago",
            State = "IL"
        };

        var existingUser = new UserInfo { Email = "test@example.com", FirstName = "Original" };

        // Act
        _mapper.Map(updateDto, existingUser);

        // Assert
        Assert.Equal("UpdatedJohn", existingUser.FirstName);
        Assert.Equal("UpdatedDoe", existingUser.LastName);
        Assert.Equal("Male", existingUser.Gender);
        Assert.Equal("789 Pine Rd", existingUser.Street);
        Assert.Equal("Chicago", existingUser.City);
        Assert.Equal("IL", existingUser.State);
    }

    [Fact]
    public void Map_BookingToBookingDto_MapsAllProperties()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 1,
            TourId = 100,
            TourName = "Test Tour",
            Place = "Test Place",
            Email = "test@example.com",
            FirstName = "John",
            BookingDate = new DateTime(2024, 6, 15),
            CreatedDate = new DateTime(2024, 1, 1),
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<BookingDto>(booking);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(1, dto.Id);
        Assert.Equal(100, dto.TourId);
        Assert.Equal("Test Tour", dto.TourName);
        Assert.Equal("Test Place", dto.Place);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal(new DateTime(2024, 6, 15), dto.BookingDate);
        Assert.Equal(new DateTime(2024, 1, 1), dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void Map_BookingCreateDtoToBooking_MapsAndSetsDefaults()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            TourId = 200,
            TourName = "New Tour",
            Place = "New Place",
            Email = "new@example.com",
            FirstName = "Jane",
            BookingDate = new DateTime(2024, 8, 10),
            CreatedBy = "Admin"
        };

        // Act
        var booking = _mapper.Map<Booking>(createDto);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(200, booking.TourId);
        Assert.Equal("New Tour", booking.TourName);
        Assert.Equal("New Place", booking.Place);
        Assert.Equal("new@example.com", booking.Email);
        Assert.Equal("Jane", booking.FirstName);
        Assert.Equal(new DateTime(2024, 8, 10), booking.BookingDate);
        Assert.True(booking.IsActive);
        Assert.NotEqual(DateTime.MinValue, booking.CreatedDate);
    }

    [Fact]
    public void Map_BookingUpdateDtoToBooking_UpdatesProperties()
    {
        // Arrange
        var updateDto = new BookingUpdateDto
        {
            BookingDate = new DateTime(2024, 9, 25),
            ModifiedBy = "Editor"
        };

        var existingBooking = new Booking
        {
            Id = 1,
            BookingDate = new DateTime(2024, 1, 1)
        };

        // Act
        _mapper.Map(updateDto, existingBooking);

        // Assert
        Assert.Equal(new DateTime(2024, 9, 25), existingBooking.BookingDate);
    }

    [Fact]
    public void Map_TourCreateDto_NullPictureUrl_HandlesNull()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "Test Tour",
            PictureUrl = null
        };

        // Act
        var tour = _mapper.Map<Tour>(createDto);

        // Assert
        Assert.Null(tour.PictureUrl);
    }

    [Fact]
    public void Map_Multiple_Tours_ToTourDtos()
    {
        // Arrange
        var tours = new[]
        {
            new Tour { Id = 1, TourName = "Tour1" },
            new Tour { Id = 2, TourName = "Tour2" }
        };

        // Act
        var dtos = _mapper.Map<TourDto[]>(tours);

        // Assert
        Assert.NotNull(dtos);
        Assert.Equal(2, dtos.Length);
        Assert.Equal("Tour1", dtos[0].TourName);
        Assert.Equal("Tour2", dtos[1].TourName);
    }
}
