using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyFood.Application.Dtos;
using MyFood.Application.Models;
using MyFood.Application.Services;

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
        var registerDto = new RegisterDto
        {
            Username = "nedim",
            Email = "nedim@example.com",
            Password = "StrongP@ss1",
            FullName = "Nedim Tahirovic"
        };

        _mockPasswordService.Setup(x => x.IsPasswordStrong(registerDto.Password)).Returns(true);
        _mockPasswordService.Setup(x => x.HashPassword(registerDto.Password)).Returns("hashed-password");
        _mockVerificationTokenService.Setup(x => x.GenerateToken()).Returns("verify-token");

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username)).ReturnsAsync((AuthUser?)null);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email)).ReturnsAsync((AuthUser?)null);
        _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<AuthUser>()))
            .ReturnsAsync(new AuthUser
            {
                Id = "1",
                Username = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                PasswordHash = "hashed-password",
                EmailVerificationToken = "verify-token",
                EmailVerified = false
            });

        var result = await _authService.RegisterAsync(registerDto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("User registered successfully. Please verify your email before logging in.", result.Message);
        Assert.AreEqual(string.Empty, result.Token);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(registerDto.Email, result.User.Email);
        _mockEmailService.Verify(x => x.SendVerificationEmailAsync(registerDto.Email, "verify-token"), Times.Once);
    }

    [TestMethod]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        var registerDto = new RegisterDto
        {
            Username = "nedim",
            Email = "existing@example.com",
            Password = "StrongP@ss1"
        };

        _mockPasswordService.Setup(x => x.IsPasswordStrong(registerDto.Password)).Returns(true);
        _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username)).ReturnsAsync((AuthUser?)null);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync(new AuthUser { Id = "7", Email = registerDto.Email, Username = "existing" });

        var result = await _authService.RegisterAsync(registerDto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Email already exists.", result.Message);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<AuthUser>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        var loginDto = new LoginDto
        {
            Username = "nedim",
            Password = "wrong-pass"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync(new AuthUser
            {
                Id = "1",
                Username = loginDto.Username,
                Email = "nedim@example.com",
                PasswordHash = "hash",
                EmailVerified = true
            });

        _mockPasswordService.Setup(x => x.VerifyPassword(loginDto.Password, "hash")).Returns(false);

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
            Username = "nedim",
            Password = "StrongP@ss1"
        };

        var user = new AuthUser
        {
            Id = "1",
            Username = loginDto.Username,
            Email = "nedim@example.com",
            FullName = "Nedim Tahirovic",
            PasswordHash = "hash",
            EmailVerified = true
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username)).ReturnsAsync(user);
        _mockPasswordService.Setup(x => x.VerifyPassword(loginDto.Password, user.PasswordHash)).Returns(true);
        _mockTokenService.Setup(x => x.GenerateToken(user)).Returns("jwt-token");

        var result = await _authService.LoginAsync(loginDto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Login successful.", result.Message);
        Assert.AreEqual("jwt-token", result.Token);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(user.Username, result.User.Username);
    }

    [TestMethod]
    public async Task LoginAsync_WithUnverifiedEmail_ShouldReturnError()
    {
        var loginDto = new LoginDto
        {
            Username = "nedim",
            Password = "StrongP@ss1"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync(new AuthUser
            {
                Id = "1",
                Username = loginDto.Username,
                Email = "nedim@example.com",
                PasswordHash = "hash",
                EmailVerified = false
            });

        var result = await _authService.LoginAsync(loginDto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Please verify your email before logging in.", result.Message);
    }

    [TestMethod]
    public async Task VerifyEmailAsync_WithValidToken_ShouldReturnSuccess()
    {
        var verifyDto = new VerifyEmailDto
        {
            Email = "nedim@example.com",
            Token = "verify-token"
        };

        var user = new AuthUser
        {
            Id = "1",
            Username = "nedim",
            Email = verifyDto.Email,
            FullName = "Nedim Tahirovic",
            PasswordHash = "hash",
            EmailVerified = false,
            EmailVerificationToken = "verify-token",
            EmailVerificationTokenExpiryUtc = DateTime.UtcNow.AddHours(1)
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(verifyDto.Email)).ReturnsAsync(user);
        _mockVerificationTokenService
            .Setup(x => x.ValidateToken(verifyDto.Token, user.EmailVerificationToken, user.EmailVerificationTokenExpiryUtc!.Value))
            .Returns(true);
        _mockUserRepository
            .Setup(x => x.UpdateAsync(It.IsAny<AuthUser>()))
            .ReturnsAsync((AuthUser u) => u);
        _mockTokenService.Setup(x => x.GenerateToken(It.IsAny<AuthUser>())).Returns("jwt-token");

        var result = await _authService.VerifyEmailAsync(verifyDto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Email verified successfully.", result.Message);
        Assert.AreEqual("jwt-token", result.Token);
    }
}
