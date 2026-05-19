using Microsoft.AspNetCore.Identity;
using Moq;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
using MyFood.Application.Repositories;
using MyFood.Application.Services;

namespace MyFood.Application.Tests;

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
        var registerDto = new RegisterDto
        {
            Username = "lejla",
            Email = "lejla@example.com",
            Password = "Password1!",
            FullName = "Lejla Doric"
        };

        _mockPasswordService.Setup(x => x.IsPasswordStrong(registerDto.Password!)).Returns(true);
        _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username!)).ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email!)).ReturnsAsync((ApplicationUser?)null);
        _mockVerificationTokenService.Setup(x => x.GenerateToken()).Returns("verification-token");
        _mockPasswordService.Setup(x => x.HashPassword(registerDto.Password!)).Returns("hashed-password");
        _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
        _mockEmailService.Setup(x => x.SendVerificationEmailAsync(registerDto.Email!, "verification-token"))
            .Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(registerDto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("User registered successfully. Please verify your email.", result.Message);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(registerDto.Username, result.User.Username);

        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        _mockEmailService.Verify(x => x.SendVerificationEmailAsync(registerDto.Email!, "verification-token"), Times.Once);
    }

    [TestMethod]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "existing@example.com",
            Password = "Password1!"
        };

        _mockPasswordService.Setup(x => x.IsPasswordStrong(registerDto.Password!)).Returns(true);
        _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username!)).ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email!))
            .ReturnsAsync(new ApplicationUser { Email = registerDto.Email });

        var result = await _authService.RegisterAsync(registerDto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Email is already in use.", result.Message);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        _mockEmailService.Verify(x => x.SendVerificationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        var loginDto = new LoginDto
        {
            Username = "lejla",
            Password = "WrongPassword1!"
        };

        var existingUser = new ApplicationUser
        {
            UserName = "lejla",
            IsEmailVerified = true,
            PasswordHash = "stored-hash"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username!)).ReturnsAsync(existingUser);
        _mockPasswordService.Setup(x => x.VerifyPassword(loginDto.Password!, existingUser.PasswordHash!)).Returns(false);

        var result = await _authService.LoginAsync(loginDto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Invalid username or password.", result.Message);
        Assert.AreEqual(string.Empty, result.Token);
    }

    [TestMethod]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        var loginDto = new LoginDto
        {
            Username = "lejla",
            Password = "Password1!"
        };

        var existingUser = new ApplicationUser
        {
            Id = "user-id-1",
            UserName = "lejla",
            FullName = "Lejla Doric",
            IsEmailVerified = true,
            PasswordHash = "stored-hash"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username!)).ReturnsAsync(existingUser);
        _mockPasswordService.Setup(x => x.VerifyPassword(loginDto.Password!, existingUser.PasswordHash!)).Returns(true);
        _mockTokenService.Setup(x => x.GenerateToken(existingUser)).Returns("jwt-token");

        var result = await _authService.LoginAsync(loginDto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Login successful.", result.Message);
        Assert.AreEqual("jwt-token", result.Token);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(existingUser.UserName, result.User.Username);
    }
}
