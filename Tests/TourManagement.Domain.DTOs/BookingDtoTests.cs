using Xunit;
using TourManagement.Domain.DTOs;

namespace TourManagement.Domain.DTOs.Tests;

public class BookingDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new BookingDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void BookingId_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.BookingId = 1;

        // Assert
        Assert.Equal(1, dto.BookingId);
    }

    [Fact]
    public void UserId_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.UserId = 10;

        // Assert
        Assert.Equal(10, dto.UserId);
    }

    [Fact]
    public void TourId_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.TourId = 20;

        // Assert
        Assert.Equal(20, dto.TourId);
    }

    [Fact]
    public void BookingDate_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingDto();
        var date = DateTime.Now;

        // Act
        dto.BookingDate = date;

        // Assert
        Assert.Equal(date, dto.BookingDate);
    }

    [Fact]
    public void NumberOfPeople_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.NumberOfPeople = 5;

        // Assert
        Assert.Equal(5, dto.NumberOfPeople);
    }

    [Fact]
    public void TotalPrice_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.TotalPrice = 500.50m;

        // Assert
        Assert.Equal(500.50m, dto.TotalPrice);
    }

    [Fact]
    public void Status_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var dto = new BookingDto();

        // Assert
        Assert.Equal("Pending", dto.Status);
    }

    [Fact]
    public void Status_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.Status = "Confirmed";

        // Assert
        Assert.Equal("Confirmed", dto.Status);
    }

    [Fact]
    public void UserName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.UserName = "John Doe";

        // Assert
        Assert.Equal("John Doe", dto.UserName);
    }

    [Fact]
    public void UserName_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var dto = new BookingDto();

        // Assert
        Assert.Equal(string.Empty, dto.UserName);
    }

    [Fact]
    public void TourName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingDto();

        // Act
        dto.TourName = "European Adventure";

        // Assert
        Assert.Equal("European Adventure", dto.TourName);
    }

    [Fact]
    public void TourName_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var dto = new BookingDto();

        // Assert
        Assert.Equal(string.Empty, dto.TourName);
    }

    [Fact]
    public void BookingDto_ShouldSetAllProperties()
    {
        // Arrange
        var date = DateTime.Now;
        var dto = new BookingDto
        {
            BookingId = 1,
            UserId = 10,
            TourId = 20,
            BookingDate = date,
            NumberOfPeople = 5,
            TotalPrice = 500.50m,
            Status = "Confirmed",
            UserName = "John Doe",
            TourName = "European Adventure"
        };

        // Assert
        Assert.Equal(1, dto.BookingId);
        Assert.Equal(10, dto.UserId);
        Assert.Equal(20, dto.TourId);
        Assert.Equal(date, dto.BookingDate);
        Assert.Equal(5, dto.NumberOfPeople);
        Assert.Equal(500.50m, dto.TotalPrice);
        Assert.Equal("Confirmed", dto.Status);
        Assert.Equal("John Doe", dto.UserName);
        Assert.Equal("European Adventure", dto.TourName);
    }
}

public class BookingCreateDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new BookingCreateDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void UserId_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingCreateDto();

        // Act
        dto.UserId = 10;

        // Assert
        Assert.Equal(10, dto.UserId);
    }

    [Fact]
    public void TourId_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingCreateDto();

        // Act
        dto.TourId = 20;

        // Assert
        Assert.Equal(20, dto.TourId);
    }

    [Fact]
    public void BookingDate_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingCreateDto();
        var date = DateTime.Now;

        // Act
        dto.BookingDate = date;

        // Assert
        Assert.Equal(date, dto.BookingDate);
    }

    [Fact]
    public void NumberOfPeople_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingCreateDto();

        // Act
        dto.NumberOfPeople = 5;

        // Assert
        Assert.Equal(5, dto.NumberOfPeople);
    }

    [Fact]
    public void BookingCreateDto_ShouldSetAllProperties()
    {
        // Arrange
        var date = DateTime.Now;
        var dto = new BookingCreateDto
        {
            UserId = 10,
            TourId = 20,
            BookingDate = date,
            NumberOfPeople = 5
        };

        // Assert
        Assert.Equal(10, dto.UserId);
        Assert.Equal(20, dto.TourId);
        Assert.Equal(date, dto.BookingDate);
        Assert.Equal(5, dto.NumberOfPeople);
    }
}

public class BookingUpdateDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new BookingUpdateDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void BookingDate_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingUpdateDto();
        var date = DateTime.Now;

        // Act
        dto.BookingDate = date;

        // Assert
        Assert.Equal(date, dto.BookingDate);
    }

    [Fact]
    public void NumberOfPeople_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingUpdateDto();

        // Act
        dto.NumberOfPeople = 5;

        // Assert
        Assert.Equal(5, dto.NumberOfPeople);
    }

    [Fact]
    public void Status_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var dto = new BookingUpdateDto();

        // Assert
        Assert.Equal("Pending", dto.Status);
    }

    [Fact]
    public void Status_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new BookingUpdateDto();

        // Act
        dto.Status = "Confirmed";

        // Assert
        Assert.Equal("Confirmed", dto.Status);
    }

    [Fact]
    public void BookingUpdateDto_ShouldSetAllProperties()
    {
        // Arrange
        var date = DateTime.Now;
        var dto = new BookingUpdateDto
        {
            BookingDate = date,
            NumberOfPeople = 5,
            Status = "Confirmed"
        };

        // Assert
        Assert.Equal(date, dto.BookingDate);
        Assert.Equal(5, dto.NumberOfPeople);
        Assert.Equal("Confirmed", dto.Status);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Confirmed")]
    [InlineData("Cancelled")]
    [InlineData("Completed")]
    public void Status_ShouldAcceptDifferentValues(string status)
    {
        // Arrange
        var dto = new BookingUpdateDto();

        // Act
        dto.Status = status;

        // Assert
        Assert.Equal(status, dto.Status);
    }
}
