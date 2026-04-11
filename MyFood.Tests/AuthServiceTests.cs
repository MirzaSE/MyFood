using Microsoft.AspNetCore.Identity;
using Moq;
using MyFood.Application.Dtos;
using MyFood.Application.Repositories;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Tests;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<ITokenService> _mockTokenService = null!;
    private Mock<IPasswordService> _mockPasswordService = null!;
    private Mock<IVerificationTokenService> _mockVerificationTokenService = null!;
    private Mock<IEmailService> _mockEmailService = null!;
    private AuthService _authService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockVerificationTokenService = new Mock<IVerificationTokenService>();
        _mockEmailService = new Mock<IEmailService>();

        _authService = new AuthService(
            _mockUserRepository.Object,
            _mockTokenService.Object,
            _mockPasswordService.Object,
            _mockVerificationTokenService.Object,
            _mockEmailService.Object);
    }

    [TestMethod]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        var dto = new RegisterDto
        {
            Email = "newuser@test.com",
            Username = "newuser",
            Password = "StrongPassword123!"
        };

        _mockUserRepository.Setup(r => r.FindByUsernameAsync(dto.Username))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(r => r.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _mockPasswordService.Setup(p => p.IsPasswordStrong(dto.Password))
            .Returns(true);
        _mockPasswordService.Setup(p => p.HashPassword(dto.Password))
            .Returns("hashed-password");
        _mockVerificationTokenService.Setup(v => v.GenerateToken())
            .Returns("verify-token");
        _mockVerificationTokenService.Setup(v => v.GetTokenExpiryUtc(It.IsAny<int>()))
            .Returns(DateTime.UtcNow.AddMinutes(60));
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _authService.RegisterAsync(dto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("User created successfully. Please verify your email before login.", result.Message);
        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        _mockEmailService.Verify(
            e => e.SendVerificationEmailAsync(dto.Email, It.Is<string>(link => link.Contains("verify-email"))),
            Times.Once);
    }

    [TestMethod]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        var dto = new RegisterDto
        {
            Email = "existing@test.com",
            Username = "newuser",
            Password = "StrongPassword123!"
        };

        _mockUserRepository.Setup(r => r.FindByUsernameAsync(dto.Username))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(r => r.FindByEmailAsync(dto.Email))
            .ReturnsAsync(new ApplicationUser { Email = dto.Email, UserName = "existing-user" });

        var result = await _authService.RegisterAsync(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Email already exists.", result.Message);
        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        _mockEmailService.Verify(e => e.SendVerificationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        var dto = new LoginDto
        {
            Username = "user1",
            Password = "wrong-password"
        };

        var existingUser = new ApplicationUser
        {
            UserName = dto.Username,
            PasswordHash = "stored-hash",
            IsEmailVerified = true
        };

        _mockUserRepository.Setup(r => r.FindByUsernameAsync(dto.Username))
            .ReturnsAsync(existingUser);
        _mockPasswordService.Setup(p => p.VerifyPassword(dto.Password, existingUser.PasswordHash!))
            .Returns(false);

        var result = await _authService.LoginAsync(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Invalid username or password.", result.Message);
        Assert.IsNull(result.Token);
        _mockTokenService.Verify(t => t.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        var dto = new LoginDto
        {
            Username = "valid-user",
            Password = "correct-password"
        };

        var existingUser = new ApplicationUser
        {
            UserName = dto.Username,
            PasswordHash = "stored-hash",
            IsEmailVerified = true
        };
        var roles = new List<string> { "Customer" };

        _mockUserRepository.Setup(r => r.FindByUsernameAsync(dto.Username))
            .ReturnsAsync(existingUser);
        _mockPasswordService.Setup(p => p.VerifyPassword(dto.Password, existingUser.PasswordHash!))
            .Returns(true);
        _mockUserRepository.Setup(r => r.GetRolesAsync(existingUser))
            .ReturnsAsync(roles);
        _mockTokenService.Setup(t => t.GenerateToken(existingUser, roles))
            .Returns("jwt-token");

        var result = await _authService.LoginAsync(dto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Login successful.", result.Message);
        Assert.AreEqual("jwt-token", result.Token);
    }
}
