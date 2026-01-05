using Microsoft.EntityFrameworkCore;
using Xunit;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations.Tests;

public class UserConfigurationTests
{
    private DbContextOptions<TourManagementDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Configure_ShouldSetTableName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("UserInfo", entityType.GetTableName());
    }

    [Fact]
    public void Configure_ShouldSetPrimaryKey()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var primaryKey = entityType?.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("UserId", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_Email_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var emailProperty = entityType?.FindProperty("Email");

        // Assert
        Assert.NotNull(emailProperty);
        Assert.False(emailProperty.IsNullable);
    }

    [Fact]
    public void Configure_Email_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var emailProperty = entityType?.FindProperty("Email");

        // Assert
        Assert.NotNull(emailProperty);
        Assert.Equal(200, emailProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_Email_ShouldHaveUniqueIndex()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var emailIndex = entityType?.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == "Email"));

        // Assert
        Assert.NotNull(emailIndex);
        Assert.True(emailIndex.IsUnique);
    }

    [Fact]
    public void Configure_FirstName_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var firstNameProperty = entityType?.FindProperty("FirstName");

        // Assert
        Assert.NotNull(firstNameProperty);
        Assert.False(firstNameProperty.IsNullable);
    }

    [Fact]
    public void Configure_FirstName_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var firstNameProperty = entityType?.FindProperty("FirstName");

        // Assert
        Assert.NotNull(firstNameProperty);
        Assert.Equal(100, firstNameProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_LastName_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var lastNameProperty = entityType?.FindProperty("LastName");

        // Assert
        Assert.NotNull(lastNameProperty);
        Assert.False(lastNameProperty.IsNullable);
    }

    [Fact]
    public void Configure_LastName_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var lastNameProperty = entityType?.FindProperty("LastName");

        // Assert
        Assert.NotNull(lastNameProperty);
        Assert.Equal(100, lastNameProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_Gender_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var genderProperty = entityType?.FindProperty("Gender");

        // Assert
        Assert.NotNull(genderProperty);
        Assert.Equal(20, genderProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_PasswordHash_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var passwordProperty = entityType?.FindProperty("PasswordHash");

        // Assert
        Assert.NotNull(passwordProperty);
        Assert.False(passwordProperty.IsNullable);
    }

    [Fact]
    public void Configure_PasswordHash_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var passwordProperty = entityType?.FindProperty("PasswordHash");

        // Assert
        Assert.NotNull(passwordProperty);
        Assert.Equal("Password", passwordProperty.GetColumnName());
    }

    [Fact]
    public void Configure_PasswordHash_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var passwordProperty = entityType?.FindProperty("PasswordHash");

        // Assert
        Assert.NotNull(passwordProperty);
        Assert.Equal(500, passwordProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_DateOfBirth_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var dobProperty = entityType?.FindProperty("DateOfBirth");

        // Assert
        Assert.NotNull(dobProperty);
        Assert.Equal("dob", dobProperty.GetColumnName());
    }

    [Fact]
    public void Configure_Street_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var streetProperty = entityType?.FindProperty("Street");

        // Assert
        Assert.NotNull(streetProperty);
        Assert.Equal(200, streetProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_City_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var cityProperty = entityType?.FindProperty("City");

        // Assert
        Assert.NotNull(cityProperty);
        Assert.Equal(100, cityProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_State_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var stateProperty = entityType?.FindProperty("State");

        // Assert
        Assert.NotNull(stateProperty);
        Assert.Equal(100, stateProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_Bookings_ShouldHaveRelationship()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(User));
        var bookingsNavigation = entityType?.FindNavigation("Bookings");

        // Assert
        Assert.NotNull(bookingsNavigation);
    }
}
