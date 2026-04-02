using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    private Mock<IEmailService> _mockEmailService = null!;
    private Mock<IVerificationTokenService> _mockVerificationTokenService = null!;
    private AuthService _authService = null!;

    [TestInitialize]
    public void Setup()
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
            _mockVerificationTokenService.Object);
    }

    [TestMethod]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        var dto = new RegisterDto
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "Strong1!"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(dto.Username)).ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);
        _mockPasswordService.Setup(x => x.IsPasswordStrong(dto.Password)).Returns(true);
        _mockPasswordService.Setup(x => x.HashPassword(dto.Password)).Returns("hashed-password");
        _mockVerificationTokenService.Setup(x => x.GenerateToken()).Returns("verify-token");

        var result = await _authService.RegisterAsync(dto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("alice", result.User?.Username);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<ApplicationUser>()), Times.Once);
        _mockUserRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockEmailService.Verify(x => x.SendVerificationEmailAsync(dto.Email, "verify-token"), Times.Once);
    }

    [TestMethod]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        var dto = new RegisterDto
        {
            Username = "alice",
            Email = "alice@example.com",
            Password = "Strong1!"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(dto.Username)).ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync(new ApplicationUser());

        var result = await _authService.RegisterAsync(dto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("A user with this email already exists.", result.Message);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        _mockUserRepository.Setup(x => x.GetByUsernameAsync("alice")).ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.LoginAsync(new LoginDto
        {
            Username = "alice",
            Password = "wrong"
        });

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Invalid username or password.", result.Message);
    }

    [TestMethod]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        var user = new ApplicationUser
        {
            Id = "42",
            UserName = "alice",
            Email = "alice@example.com",
            PasswordHash = "hashed-password",
            IsEmailVerified = true
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync("alice")).ReturnsAsync(user);
        _mockPasswordService.Setup(x => x.VerifyPassword("Strong1!", "hashed-password")).Returns(true);
        _mockTokenService.Setup(x => x.GenerateToken(user)).Returns("jwt-token");

        var result = await _authService.LoginAsync(new LoginDto
        {
            Username = "alice",
            Password = "Strong1!"
        });

        Assert.IsTrue(result.Success);
        Assert.AreEqual("jwt-token", result.Token);
        Assert.AreEqual("alice", result.User?.Username);
    }
}
