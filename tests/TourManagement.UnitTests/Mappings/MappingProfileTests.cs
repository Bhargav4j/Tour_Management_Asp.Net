using AutoMapper;
using TourManagement.Application.Mappings;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using Xunit;

namespace TourManagement.UnitTests.Mappings;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void MappingProfile_Configuration_ShouldBeValid()
    {
        // Arrange & Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_TourToTourDto_ShouldMapCorrectly()
    {
        // Arrange
        var tour = new Tour
        {
            Id = 1,
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 7,
            Price = 1500.00m,
            Locations = "Eiffel Tower",
            TourInfo = "Amazing tour",
            PicturePath = "/images/paris.jpg",
            IsActive = true
        };

        // Act
        var result = _mapper.Map<TourDto>(tour);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
        Assert.Equal("Paris", result.Place);
        Assert.Equal(7, result.Days);
        Assert.Equal(1500.00m, result.Price);
    }

    [Fact]
    public void Map_TourCreateDtoToTour_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "Rome Tour",
            Place = "Rome",
            Days = 5,
            Price = 1200.00m,
            Locations = "Colosseum",
            TourInfo = "Historic tour"
        };

        // Act
        var result = _mapper.Map<Tour>(createDto);

        // Assert
        Assert.Equal("Rome Tour", result.TourName);
        Assert.Equal("Rome", result.Place);
        Assert.Equal(5, result.Days);
        Assert.Equal(1200.00m, result.Price);
        Assert.True(result.IsActive);
        Assert.Equal("System", result.CreatedBy);
    }

    [Fact]
    public void Map_UserToUserDto_ShouldMapCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "Main St",
            City = "Boston",
            State = "MA",
            IsActive = true
        };

        // Act
        var result = _mapper.Map<UserDto>(user);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("Male", result.Gender);
    }

    [Fact]
    public void Map_UserCreateDtoToUser_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new UserCreateDto
        {
            Email = "new@user.com",
            FirstName = "Jane",
            LastName = "Smith",
            Gender = "Female",
            Password = "password123",
            DateOfBirth = new DateTime(1985, 5, 15),
            Street = "Oak Ave",
            City = "Seattle",
            State = "WA"
        };

        // Act
        var result = _mapper.Map<User>(createDto);

        // Assert
        Assert.Equal("new@user.com", result.Email);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal("Female", result.Gender);
        Assert.True(result.IsActive);
        Assert.Equal("System", result.CreatedBy);
    }

    [Fact]
    public void Map_BookingToBookingDto_ShouldMapCorrectly()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 1,
            UserId = 10,
            TourId = 5,
            BookingDate = DateTime.UtcNow,
            NumberOfPeople = 4,
            TotalAmount = 6000.00m,
            Status = "Confirmed",
            User = new User { FirstName = "John", LastName = "Doe" },
            Tour = new Tour { TourName = "Paris Tour" }
        };

        // Act
        var result = _mapper.Map<BookingDto>(booking);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.UserId);
        Assert.Equal(5, result.TourId);
        Assert.Equal(4, result.NumberOfPeople);
        Assert.Equal(6000.00m, result.TotalAmount);
        Assert.Equal("Confirmed", result.Status);
        Assert.Equal("John Doe", result.UserName);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public void Map_BookingCreateDtoToBooking_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            UserId = 20,
            TourId = 8,
            BookingDate = DateTime.UtcNow,
            NumberOfPeople = 2,
            TotalAmount = 3000.00m
        };

        // Act
        var result = _mapper.Map<Booking>(createDto);

        // Assert
        Assert.Equal(20, result.UserId);
        Assert.Equal(8, result.TourId);
        Assert.Equal(2, result.NumberOfPeople);
        Assert.Equal(3000.00m, result.TotalAmount);
        Assert.True(result.IsActive);
        Assert.Equal("System", result.CreatedBy);
    }
}
