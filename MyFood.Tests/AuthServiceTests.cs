using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyFood.Application.Dtos;
using MyFood.Application.Services;
using MyFood.Application.Services.Passwords;
using MyFood.Domain.Entities;

namespace MyFood.Tests;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<ITokenService> _mockTokenService = null!;
    private Mock<IPasswordService> _mockPasswordService = null!;
    private Mock<IEmailService> _mockEmailService = null!;
    private Mock<IVerificationTokenService> _mockVerificationTokenService = null!;
    private AuthService _authService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockTokenService = new Mock<ITokenService>(MockBehavior.Strict);
        _mockPasswordService = new Mock<IPasswordService>(MockBehavior.Strict);
        _mockEmailService = new Mock<IEmailService>(MockBehavior.Strict);
        _mockVerificationTokenService = new Mock<IVerificationTokenService>(MockBehavior.Strict);

        _authService = new AuthService(
            _mockUserRepository.Object,
            _mockTokenService.Object,
            _mockPasswordService.Object,
            _mockEmailService.Object,
            _mockVerificationTokenService.Object);
    }

    [TestMethod]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "StrongP@ss1"
        };

        _mockPasswordService
            .Setup(p => p.ValidateStrength(registerDto.Password))
            .Returns(new PasswordValidationResult(true, Array.Empty<string>()));
        _mockUserRepository.Setup(r => r.FindByUsernameAsync(registerDto.Username))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(r => r.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _mockVerificationTokenService
            .Setup(v => v.GenerateToken())
            .Returns("token-123");
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<ApplicationUser>()))
            .Returns(Task.CompletedTask);
        _mockEmailService.Setup(e => e.SendVerificationEmailAsync(registerDto.Email, "token-123"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual("Registration successful. Please verify your email to continue.", result.Message);
        _mockEmailService.Verify(e => e.SendVerificationEmailAsync(registerDto.Email, "token-123"), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<ApplicationUser>(u => u.EmailVerificationToken == "token-123")), Times.Once);

        VerifyAll();
    }

    [TestMethod]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "bob",
            Email = "bob@example.com",
            Password = "StrongP@ss1"
        };

        _mockPasswordService
            .Setup(p => p.ValidateStrength(registerDto.Password))
            .Returns(new PasswordValidationResult(true, Array.Empty<string>()));
        _mockUserRepository.Setup(r => r.FindByUsernameAsync(registerDto.Username))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(r => r.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync(new ApplicationUser());

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.IsFalse(result.Success);
        var errors = result.Errors?.ToList();
        Assert.IsNotNull(errors);
        CollectionAssert.Contains(errors!, "Email is already registered.");

        VerifyAll();
    }

    [TestMethod]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "chris", Password = "bad" };
        _mockUserRepository.Setup(r => r.FindByUsernameAsync(loginDto.Username))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.IsFalse(result.Success);
        var errors = result.Errors?.ToList();
        Assert.IsNotNull(errors);
        CollectionAssert.Contains(errors!, "Invalid username or password.");

        VerifyAll();
    }

    [TestMethod]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "diana", Password = "ValidPass123!" };
        var user = new ApplicationUser { UserName = loginDto.Username, IsEmailVerified = true };
        var roles = new List<string> { "User" };
        var tokenResult = new TokenResult("jwt-token", DateTime.UtcNow.AddHours(1));

        _mockUserRepository.Setup(r => r.FindByUsernameAsync(loginDto.Username))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.GetRolesAsync(user))
            .ReturnsAsync(roles);
        _mockTokenService.Setup(t => t.GenerateTokenAsync(user, roles))
            .ReturnsAsync(tokenResult);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual("jwt-token", result.Token);
        Assert.AreEqual(tokenResult.ExpiresAt, result.ExpiresAt);

        VerifyAll();
    }

    private void VerifyAll()
    {
        _mockUserRepository.VerifyAll();
        _mockTokenService.VerifyAll();
        _mockPasswordService.VerifyAll();
        _mockEmailService.VerifyAll();
        _mockVerificationTokenService.VerifyAll();
    }
}
