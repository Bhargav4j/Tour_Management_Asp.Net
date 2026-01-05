using AutoMapper;
using Xunit;
using TourManagement.Application.Mappings;
using TourManagement.Domain.DTOs;
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
    public void MappingProfile_ShouldBeValid()
    {
        // Arrange & Act
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        // Assert
        configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void Tour_ToTourDto_ShouldMap()
    {
        // Arrange
        var tour = new Tour
        {
            TourId = 1,
            TourName = "Test Tour",
            Place = "Paris",
            Days = 7,
            Price = 1000m,
            Locations = "Paris, Rome",
            TourInfo = "Info",
            PictureFileName = "pic.jpg",
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<TourDto>(tour);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(tour.TourId, dto.TourId);
        Assert.Equal(tour.TourName, dto.TourName);
        Assert.Equal(tour.Place, dto.Place);
        Assert.Equal(tour.Days, dto.Days);
        Assert.Equal(tour.Price, dto.Price);
        Assert.Equal(tour.IsActive, dto.IsActive);
    }

    [Fact]
    public void TourCreateDto_ToTour_ShouldMap()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "New Tour",
            Place = "London",
            Days = 5,
            Price = 800m,
            Locations = "London, Paris",
            TourInfo = "Tour info",
            PictureFileName = "image.jpg"
        };

        // Act
        var tour = _mapper.Map<Tour>(createDto);

        // Assert
        Assert.NotNull(tour);
        Assert.Equal(createDto.TourName, tour.TourName);
        Assert.Equal(createDto.Place, tour.Place);
        Assert.Equal(createDto.Days, tour.Days);
        Assert.Equal(createDto.Price, tour.Price);
    }

    [Fact]
    public void TourUpdateDto_ToTour_ShouldMap()
    {
        // Arrange
        var updateDto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Rome",
            Days = 10,
            Price = 1500m,
            Locations = "Rome, Venice",
            TourInfo = "Updated info",
            PictureFileName = "updated.jpg",
            IsActive = false
        };

        // Act
        var tour = _mapper.Map<Tour>(updateDto);

        // Assert
        Assert.NotNull(tour);
        Assert.Equal(updateDto.TourName, tour.TourName);
        Assert.Equal(updateDto.Place, tour.Place);
        Assert.Equal(updateDto.Days, tour.Days);
        Assert.Equal(updateDto.Price, tour.Price);
        Assert.Equal(updateDto.IsActive, tour.IsActive);
    }

    [Fact]
    public void User_ToUserDto_ShouldMap()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<UserDto>(user);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(user.UserId, dto.UserId);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.FirstName, dto.FirstName);
        Assert.Equal(user.LastName, dto.LastName);
        Assert.Equal(user.IsActive, dto.IsActive);
    }

    [Fact]
    public void UserRegisterDto_ToUser_ShouldMap()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Email = "new@test.com",
            FirstName = "Jane",
            LastName = "Smith",
            Gender = "Female",
            Password = "password123",
            DateOfBirth = new DateTime(1995, 5, 15),
            Street = "456 Oak Ave",
            City = "Los Angeles",
            State = "CA"
        };

        // Act
        var user = _mapper.Map<User>(registerDto);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(registerDto.Email, user.Email);
        Assert.Equal(registerDto.FirstName, user.FirstName);
        Assert.Equal(registerDto.LastName, user.LastName);
        Assert.Equal(registerDto.Gender, user.Gender);
    }

    [Fact]
    public void UserRegisterDto_ToUser_ShouldIgnorePasswordHash()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Email = "new@test.com",
            FirstName = "Jane",
            LastName = "Smith",
            Password = "password123"
        };

        // Act
        var user = _mapper.Map<User>(registerDto);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(string.Empty, user.PasswordHash);
    }

    [Fact]
    public void UserUpdateDto_ToUser_ShouldMap()
    {
        // Arrange
        var updateDto = new UserUpdateDto
        {
            FirstName = "Updated",
            LastName = "User",
            Gender = "Other",
            DateOfBirth = new DateTime(1985, 3, 20),
            Street = "789 Pine Rd",
            City = "Chicago",
            State = "IL"
        };

        // Act
        var user = _mapper.Map<User>(updateDto);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(updateDto.FirstName, user.FirstName);
        Assert.Equal(updateDto.LastName, user.LastName);
        Assert.Equal(updateDto.Gender, user.Gender);
    }

    [Fact]
    public void Booking_ToBookingDto_ShouldMapWithUserName()
    {
        // Arrange
        var booking = new Booking
        {
            BookingId = 1,
            UserId = 1,
            TourId = 1,
            NumberOfPeople = 2,
            TotalPrice = 200m,
            Status = "Confirmed",
            BookingDate = DateTime.Now,
            User = new User { FirstName = "John", LastName = "Doe" },
            Tour = new Tour { TourName = "Test Tour" }
        };

        // Act
        var dto = _mapper.Map<BookingDto>(booking);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(booking.BookingId, dto.BookingId);
        Assert.Equal(booking.UserId, dto.UserId);
        Assert.Equal(booking.TourId, dto.TourId);
        Assert.Equal("John Doe", dto.UserName);
        Assert.Equal("Test Tour", dto.TourName);
    }

    [Fact]
    public void BookingCreateDto_ToBooking_ShouldMap()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            UserId = 1,
            TourId = 1,
            NumberOfPeople = 2,
            BookingDate = DateTime.Now
        };

        // Act
        var booking = _mapper.Map<Booking>(createDto);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(createDto.UserId, booking.UserId);
        Assert.Equal(createDto.TourId, booking.TourId);
        Assert.Equal(createDto.NumberOfPeople, booking.NumberOfPeople);
    }

    [Fact]
    public void BookingUpdateDto_ToBooking_ShouldMap()
    {
        // Arrange
        var updateDto = new BookingUpdateDto
        {
            NumberOfPeople = 5,
            Status = "Confirmed",
            BookingDate = DateTime.Now
        };

        // Act
        var booking = _mapper.Map<Booking>(updateDto);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(updateDto.NumberOfPeople, booking.NumberOfPeople);
        Assert.Equal(updateDto.Status, booking.Status);
    }

    [Fact]
    public void MappingProfile_ShouldMapAllTourProperties()
    {
        // Arrange
        var tour = new Tour
        {
            TourId = 1,
            TourName = "Complete Tour",
            Place = "Paris",
            Days = 7,
            Price = 1000m,
            Locations = "Paris, Rome, Barcelona",
            TourInfo = "Complete tour information",
            PictureFileName = "tour.jpg",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<TourDto>(tour);

        // Assert
        Assert.Equal(tour.TourId, dto.TourId);
        Assert.Equal(tour.TourName, dto.TourName);
        Assert.Equal(tour.Place, dto.Place);
        Assert.Equal(tour.Days, dto.Days);
        Assert.Equal(tour.Price, dto.Price);
        Assert.Equal(tour.Locations, dto.Locations);
        Assert.Equal(tour.TourInfo, dto.TourInfo);
        Assert.Equal(tour.PictureFileName, dto.PictureFileName);
        Assert.Equal(tour.IsActive, dto.IsActive);
    }

    [Fact]
    public void MappingProfile_ShouldMapAllUserProperties()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Email = "complete@test.com",
            FirstName = "Complete",
            LastName = "User",
            Gender = "Male",
            PasswordHash = "hash123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Complete St",
            City = "Complete City",
            State = "CC",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            IsActive = true
        };

        // Act
        var dto = _mapper.Map<UserDto>(user);

        // Assert
        Assert.Equal(user.UserId, dto.UserId);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.FirstName, dto.FirstName);
        Assert.Equal(user.LastName, dto.LastName);
        Assert.Equal(user.Gender, dto.Gender);
        Assert.Equal(user.DateOfBirth, dto.DateOfBirth);
        Assert.Equal(user.Street, dto.Street);
        Assert.Equal(user.City, dto.City);
        Assert.Equal(user.State, dto.State);
        Assert.Equal(user.IsActive, dto.IsActive);
    }
}
