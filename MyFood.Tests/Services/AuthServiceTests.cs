using Moq;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Domain.Entities;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace MyFood.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IVerificationTokenService> _mockVerificationTokenService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _mockPasswordService = new Mock<IPasswordService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockVerificationTokenService = new Mock<IVerificationTokenService>();
            
            _authService = new AuthService(
                _mockUserRepository.Object,
                _mockTokenService.Object,
                _mockPasswordService.Object,
                _mockEmailService.Object,
                _mockVerificationTokenService.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "StrongP@ssw0rd123",
                ConfirmPassword = "StrongP@ssw0rd123",
                FullName = "New User"
            };

            _mockPasswordService
                .Setup(x => x.GetPasswordValidationError(registerDto.Password))
                .Returns((string)null);
            
            _mockUserRepository
                .Setup(x => x.FindByUsernameAsync(registerDto.Username))
                .ReturnsAsync((ApplicationUser)null);
            
            _mockUserRepository
                .Setup(x => x.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync((ApplicationUser)null);
            
            _mockUserRepository
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
                .ReturnsAsync((true, new string[0]));
            
            _mockUserRepository
                .Setup(x => x.SetEmailVerificationTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);
            
            _mockVerificationTokenService
                .Setup(x => x.GenerateToken())
                .Returns("test-verification-token");
            
            // Act
            var result = await _authService.RegisterAsync(registerDto);
            
            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("check your email");
            result.Token.Should().BeNull(); // No token until email verified
            result.User.Should().NotBeNull();
            result.User.Username.Should().Be("newuser");
            
            _mockEmailService.Verify(
                x => x.SendVerificationEmailAsync(registerDto.Email, It.IsAny<string>(), It.IsAny<string>()), 
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithWeakPassword_ShouldReturnError()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "weak",
                ConfirmPassword = "weak",
                FullName = "New User"
            };

            _mockPasswordService
                .Setup(x => x.GetPasswordValidationError(registerDto.Password))
                .Returns("Password must be at least 8 characters long");
            
            // Act
            var result = await _authService.RegisterAsync(registerDto);
            
            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("at least 8 characters");
            
            _mockUserRepository.Verify(
                x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), 
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "existing@example.com",
                Password = "StrongP@ssw0rd123",
                ConfirmPassword = "StrongP@ssw0rd123",
                FullName = "New User"
            };
            
            var existingUser = new ApplicationUser 
            { 
                Id = "123", 
                UserName = "existinguser", 
                Email = "existing@example.com" 
            };

            _mockPasswordService
                .Setup(x => x.GetPasswordValidationError(registerDto.Password))
                .Returns((string)null);
            
            _mockUserRepository
                .Setup(x => x.FindByUsernameAsync(registerDto.Username))
                .ReturnsAsync((ApplicationUser)null);
            
            _mockUserRepository
                .Setup(x => x.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync(existingUser);
            
            // Act
            var result = await _authService.RegisterAsync(registerDto);
            
            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Email already exists");
            
            _mockUserRepository.Verify(
                x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), 
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WithMismatchedPasswords_ShouldReturnError()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "StrongP@ssw0rd123",
                ConfirmPassword = "DifferentPassword123",
                FullName = "New User"
            };
            
            // Act
            var result = await _authService.RegisterAsync(registerDto);
            
            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Passwords do not match");
            
            _mockUserRepository.Verify(
                x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), 
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "existinguser",
                Password = "StrongP@ssw0rd123"
            };
            
            var user = new ApplicationUser
            {
                Id = "123",
                UserName = "existinguser",
                Email = "user@example.com",
                FullName = "Existing User",
                IsEmailVerified = true
            };
            
            var expectedToken = "jwt-token-12345";

            _mockUserRepository
                .Setup(x => x.FindByUsernameAsync(loginDto.Username))
                .ReturnsAsync(user);
            
            _mockUserRepository
                .Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true);
            
            _mockTokenService
                .Setup(x => x.GenerateTokenAsync(user))
                .ReturnsAsync(expectedToken);
            
            // Act
            var result = await _authService.LoginAsync(loginDto);
            
            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Login successful");
            result.Token.Should().Be(expectedToken);
            result.User.Should().NotBeNull();
            result.User.Username.Should().Be("existinguser");
        }

        [Fact]
       
public async Task LoginAsync_WithUnverifiedEmail_ShouldReturnError()
{
    // Arrange
    var loginDto = new LoginDto
    {
        Username = "unverifieduser",
        Password = "StrongP@ssw0rd123"
    };
    
    var user = new ApplicationUser
    {
        Id = "456",
        UserName = "unverifieduser",
        Email = "unverified@example.com",
        FullName = "Unverified User",
        IsEmailVerified = false
    };

    _mockUserRepository
        .Setup(x => x.FindByUsernameAsync(loginDto.Username))
        .ReturnsAsync(user);
    
    // Act
    var result = await _authService.LoginAsync(loginDto);
    
    // Assert
    result.Success.Should().BeFalse();
    result.Message.Should().Contain("verify your email");
    result.Token.Should().BeEmpty();  // ← Change from BeNull() to BeEmpty()
    
    _mockTokenService.Verify(
        x => x.GenerateTokenAsync(It.IsAny<ApplicationUser>()), 
        Times.Never);
}

        [Fact]
       
public async Task LoginAsync_WithInvalidPassword_ShouldReturnError()
{
    // Arrange
    var loginDto = new LoginDto
    {
        Username = "existinguser",
        Password = "WrongPassword123"
    };
    
    var user = new ApplicationUser
    {
        Id = "123",
        UserName = "existinguser",
        Email = "user@example.com",
        IsEmailVerified = true
    };

    _mockUserRepository
        .Setup(x => x.FindByUsernameAsync(loginDto.Username))
        .ReturnsAsync(user);
    
    _mockUserRepository
        .Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
        .ReturnsAsync(false);
    
    // Act
    var result = await _authService.LoginAsync(loginDto);
    
    // Assert
    result.Success.Should().BeFalse();
    result.Message.Should().Be("Invalid username or password");
    result.Token.Should().BeEmpty();  // ← Change from BeNull() to BeEmpty()
    
    _mockTokenService.Verify(
        x => x.GenerateTokenAsync(It.IsAny<ApplicationUser>()), 
        Times.Never);
}

        [Fact]
        
public async Task LoginAsync_WithNonExistentUser_ShouldReturnError()
{
    // Arrange
    var loginDto = new LoginDto
    {
        Username = "nonexistent",
        Password = "AnyPassword123"
    };

    _mockUserRepository
        .Setup(x => x.FindByUsernameAsync(loginDto.Username))
        .ReturnsAsync((ApplicationUser)null);
    
    // Act
    var result = await _authService.LoginAsync(loginDto);
    
    // Assert
    result.Success.Should().BeFalse();
    result.Message.Should().Be("Invalid username or password");
    result.Token.Should().BeEmpty();  // ← Change from BeNull() to BeEmpty()
}

        [Fact]
        public async Task LoginAsync_WithEmptyUsername_ShouldReturnError()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "",
                Password = "StrongP@ssw0rd123"
            };
            
            // Act
            var result = await _authService.LoginAsync(loginDto);
            
            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Username is required");
        }

        [Fact]
        public async Task VerifyEmailAsync_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var userId = "123";
            var token = "valid-token-123";
            
            _mockUserRepository
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new ApplicationUser { Id = userId });
            
            _mockUserRepository
                .Setup(x => x.VerifyEmailAsync(userId, token))
                .ReturnsAsync(true);
            
            // Act
            var result = await _authService.VerifyEmailAsync(userId, token);
            
            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("Email verified successfully");
        }

        [Fact]
        public async Task VerifyEmailAsync_WithInvalidToken_ShouldReturnError()
        {
            // Arrange
            var userId = "123";
            var token = "invalid-token";
            
            _mockUserRepository
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(new ApplicationUser { Id = userId });
            
            _mockUserRepository
                .Setup(x => x.VerifyEmailAsync(userId, token))
                .ReturnsAsync(false);
            
            // Act
            var result = await _authService.VerifyEmailAsync(userId, token);
            
            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Invalid or expired");
        }
    }
}