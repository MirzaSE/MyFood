using Moq;
using MyFood.Application.Dtos;
using MyFood.Application.Entities;
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
            Username = "jane",
            Email = "jane@example.com",
            Password = "StrongPassword123!"
        };

        ApplicationUser? createdUser = null;

        _mockPasswordService.Setup(x => x.IsPasswordStrong(registerDto.Password)).Returns(true);
        _mockPasswordService.Setup(x => x.HashPassword(registerDto.Password)).Returns("hashed-password");
        _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username)).ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email)).ReturnsAsync((ApplicationUser?)null);
        _mockVerificationTokenService.Setup(x => x.GenerateToken()).Returns("verify-token");
        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
            .Callback<ApplicationUser>(user =>
            {
                user.Id = "user-123";
                createdUser = user;
            })
            .ReturnsAsync((true, Enumerable.Empty<string>()));

        var result = await _authService.RegisterAsync(registerDto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("User registered successfully. Please verify your email before logging in.", result.Message);
        Assert.IsNotNull(result.User);
        Assert.AreEqual("user-123", result.User!.Id);
        Assert.AreEqual(registerDto.Username, result.User.Username);

        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        _mockEmailService.Verify(x => x.SendVerificationEmailAsync(registerDto.Email, "verify-token"), Times.Once);
        Assert.IsNotNull(createdUser);
        Assert.AreEqual("hashed-password", createdUser!.PasswordHash);
        Assert.AreEqual("verify-token", createdUser.VerificationToken);
    }

    [TestMethod]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        var registerDto = new RegisterDto
        {
            Username = "jane",
            Email = "jane@example.com",
            Password = "StrongPassword123!"
        };

        _mockPasswordService.Setup(x => x.IsPasswordStrong(registerDto.Password)).Returns(true);
        _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username)).ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync(new ApplicationUser { Email = registerDto.Email, UserName = "another-user" });

        var result = await _authService.RegisterAsync(registerDto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Email already exists.", result.Message);
        Assert.IsNull(result.User);

        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        _mockEmailService.Verify(x => x.SendVerificationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        var loginDto = new LoginDto
        {
            Username = "jane",
            Password = "WrongPassword"
        };

        var storedUser = new ApplicationUser
        {
            Id = "user-123",
            UserName = "jane",
            Email = "jane@example.com",
            EmailConfirmed = true,
            PasswordHash = "stored-hash"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username)).ReturnsAsync(storedUser);
        _mockPasswordService.Setup(x => x.VerifyPassword(loginDto.Password, "stored-hash")).Returns(false);

        var result = await _authService.LoginAsync(loginDto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Invalid username or password.", result.Message);
        Assert.IsNull(result.Token);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        var loginDto = new LoginDto
        {
            Username = "jane",
            Password = "StrongPassword123!"
        };

        var storedUser = new ApplicationUser
        {
            Id = "user-123",
            UserName = "jane",
            FullName = "Jane Doe",
            Email = "jane@example.com",
            EmailConfirmed = true,
            PasswordHash = "stored-hash"
        };

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.Username)).ReturnsAsync(storedUser);
        _mockPasswordService.Setup(x => x.VerifyPassword(loginDto.Password, "stored-hash")).Returns(true);
        _mockTokenService.Setup(x => x.GenerateToken(storedUser)).Returns("jwt-token");

        var result = await _authService.LoginAsync(loginDto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Login successful.", result.Message);
        Assert.AreEqual("jwt-token", result.Token);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(storedUser.Id, result.User!.Id);
        Assert.AreEqual(storedUser.UserName, result.User.Username);
        Assert.AreEqual(storedUser.FullName, result.User.FullName);

        _mockTokenService.Verify(x => x.GenerateToken(storedUser), Times.Once);
    }
}
