using Xunit;
using AutoMapper;
using System;
using TourManagement.Application.Mappings;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Mappings.Tests;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void MappingProfile_Constructor_CreatesInstance()
    {
        // Arrange & Act
        var profile = new MappingProfile();

        // Assert
        Assert.NotNull(profile);
    }

    [Fact]
    public void MappingProfile_ConfigurationIsValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        // Act & Assert
        configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_TourToTourDto_MapsCorrectly()
    {
        // Arrange
        var tour = new Tour
        {
            Id = 1,
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000m,
            Locations = "Paris, Lyon",
            TourInfo = "Great tour",
            PicturePath = "/images/tour.jpg",
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<TourDto>(tour);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(tour.Id, dto.Id);
        Assert.Equal(tour.TourName, dto.TourName);
        Assert.Equal(tour.Place, dto.Place);
        Assert.Equal(tour.Days, dto.Days);
        Assert.Equal(tour.Price, dto.Price);
        Assert.Equal(tour.Locations, dto.Locations);
        Assert.Equal(tour.TourInfo, dto.TourInfo);
        Assert.Equal(tour.PicturePath, dto.PicturePath);
        Assert.Equal(tour.CreatedDate, dto.CreatedDate);
        Assert.Equal(tour.IsActive, dto.IsActive);
    }

    [Fact]
    public void Map_TourCreateDtoToTour_MapsCorrectly()
    {
        // Arrange
        var dto = new TourCreateDto
        {
            TourName = "Berlin Tour",
            Place = "Berlin",
            Days = 7,
            Price = 1500m,
            Locations = "Berlin, Munich",
            TourInfo = "Amazing tour",
            PicturePath = "/images/berlin.jpg"
        };

        // Act
        var tour = _mapper.Map<Tour>(dto);

        // Assert
        Assert.NotNull(tour);
        Assert.Equal(dto.TourName, tour.TourName);
        Assert.Equal(dto.Place, tour.Place);
        Assert.Equal(dto.Days, tour.Days);
        Assert.Equal(dto.Price, tour.Price);
        Assert.Equal(dto.Locations, tour.Locations);
        Assert.Equal(dto.TourInfo, tour.TourInfo);
        Assert.Equal(dto.PicturePath, tour.PicturePath);
        Assert.True(tour.IsActive);
        Assert.Equal("System", tour.CreatedBy);
    }

    [Fact]
    public void Map_TourUpdateDtoToTour_MapsCorrectly()
    {
        // Arrange
        var dto = new TourUpdateDto
        {
            TourName = "Rome Tour",
            Place = "Rome",
            Days = 6,
            Price = 1800m,
            Locations = "Rome, Florence",
            TourInfo = "Incredible tour",
            PicturePath = "/images/rome.jpg"
        };

        // Act
        var tour = _mapper.Map<Tour>(dto);

        // Assert
        Assert.NotNull(tour);
        Assert.Equal(dto.TourName, tour.TourName);
        Assert.Equal(dto.Place, tour.Place);
        Assert.Equal(dto.Days, tour.Days);
        Assert.Equal(dto.Price, tour.Price);
        Assert.Equal(dto.Locations, tour.Locations);
        Assert.Equal(dto.TourInfo, tour.TourInfo);
        Assert.Equal(dto.PicturePath, tour.PicturePath);
        Assert.Equal("System", tour.ModifiedBy);
    }

    [Fact]
    public void Map_UserToUserDto_MapsCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<UserDto>(user);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.FirstName, dto.FirstName);
        Assert.Equal(user.LastName, dto.LastName);
        Assert.Equal(user.PhoneNumber, dto.PhoneNumber);
        Assert.Equal(user.CreatedDate, dto.CreatedDate);
        Assert.Equal(user.IsActive, dto.IsActive);
    }

    [Fact]
    public void Map_UserCreateDtoToUser_MapsCorrectly()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Email = "newuser@example.com",
            Password = "Password123",
            FirstName = "Jane",
            LastName = "Smith",
            PhoneNumber = "+9876543210"
        };

        // Act
        var user = _mapper.Map<User>(dto);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.FirstName, user.FirstName);
        Assert.Equal(dto.LastName, user.LastName);
        Assert.Equal(dto.PhoneNumber, user.PhoneNumber);
        Assert.True(user.IsActive);
        Assert.Equal("System", user.CreatedBy);
    }

    [Fact]
    public void Map_UserUpdateDtoToUser_MapsCorrectly()
    {
        // Arrange
        var dto = new UserUpdateDto
        {
            Email = "updated@example.com",
            FirstName = "UpdatedFirst",
            LastName = "UpdatedLast",
            PhoneNumber = "+1112223333"
        };

        // Act
        var user = _mapper.Map<User>(dto);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.FirstName, user.FirstName);
        Assert.Equal(dto.LastName, user.LastName);
        Assert.Equal(dto.PhoneNumber, user.PhoneNumber);
        Assert.Equal("System", user.ModifiedBy);
    }

    [Fact]
    public void Map_BookingToBookingDto_MapsCorrectly()
    {
        // Arrange
        var booking = new Booking
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

        // Act
        var dto = _mapper.Map<BookingDto>(booking);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(booking.Id, dto.Id);
        Assert.Equal(booking.UserId, dto.UserId);
        Assert.Equal(booking.TourId, dto.TourId);
        Assert.Equal(booking.BookingDate, dto.BookingDate);
        Assert.Equal(booking.NumberOfPeople, dto.NumberOfPeople);
        Assert.Equal(booking.TotalAmount, dto.TotalAmount);
        Assert.Equal(booking.Status, dto.Status);
        Assert.Equal(booking.CreatedDate, dto.CreatedDate);
        Assert.Equal(booking.IsActive, dto.IsActive);
    }

    [Fact]
    public void Map_BookingCreateDtoToBooking_MapsCorrectly()
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

        // Act
        var booking = _mapper.Map<Booking>(dto);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(dto.UserId, booking.UserId);
        Assert.Equal(dto.TourId, booking.TourId);
        Assert.Equal(dto.BookingDate, booking.BookingDate);
        Assert.Equal(dto.NumberOfPeople, booking.NumberOfPeople);
        Assert.Equal(dto.TotalAmount, booking.TotalAmount);
        Assert.Equal(dto.Status, booking.Status);
        Assert.True(booking.IsActive);
        Assert.Equal("System", booking.CreatedBy);
    }

    [Fact]
    public void Map_BookingUpdateDtoToBooking_MapsCorrectly()
    {
        // Arrange
        var dto = new BookingUpdateDto
        {
            BookingDate = DateTime.Now,
            NumberOfPeople = 6,
            TotalAmount = 6000m,
            Status = "Cancelled"
        };

        // Act
        var booking = _mapper.Map<Booking>(dto);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(dto.BookingDate, booking.BookingDate);
        Assert.Equal(dto.NumberOfPeople, booking.NumberOfPeople);
        Assert.Equal(dto.TotalAmount, booking.TotalAmount);
        Assert.Equal(dto.Status, booking.Status);
        Assert.Equal("System", booking.ModifiedBy);
    }

    [Fact]
    public void Map_TourToTourDto_WithNullValues_MapsCorrectly()
    {
        // Arrange
        var tour = new Tour
        {
            Id = 1,
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 1,
            Price = 100m,
            Locations = "Location",
            TourInfo = "Info",
            PicturePath = null,
            CreatedDate = DateTime.Now,
            IsActive = false
        };

        // Act
        var dto = _mapper.Map<TourDto>(tour);

        // Assert
        Assert.NotNull(dto);
        Assert.Null(dto.PicturePath);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void Map_UserToUserDto_WithNullPhoneNumber_MapsCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = null,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<UserDto>(user);

        // Assert
        Assert.NotNull(dto);
        Assert.Null(dto.PhoneNumber);
    }
}
