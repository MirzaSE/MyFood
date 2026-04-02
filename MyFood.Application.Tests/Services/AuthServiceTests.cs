using Moq;
using MyFood.Application.Dtos;
using MyFood.Application.Repositories;
using MyFood.Application.Services;
using MyFood.Domain.Entities;

namespace MyFood.Application.Tests.Services;

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
            Username = "newuser",
            Email = "newuser@myfood.test",
            Password = "Strong@123"
        };

        _mockPasswordService
            .Setup(x => x.IsPasswordStrong(registerDto.Password))
            .Returns(true);
        _mockPasswordService
            .Setup(x => x.HashPassword(registerDto.Password))
            .Returns("hashed_password");
        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync(registerDto.Username))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _mockVerificationTokenService
            .Setup(x => x.GenerateToken())
            .Returns("verification-token");
        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync((true, Enumerable.Empty<string>()));
        _mockEmailService
            .Setup(x => x.SendVerificationEmailAsync(registerDto.Email, "verification-token"))
            .Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(registerDto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("User created successfully. Please verify your email before logging in.", result.Message);
        Assert.IsNull(result.Token);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(registerDto.Username, result.User.Username);
        Assert.AreEqual(registerDto.Email, result.User.Email);

        _mockUserRepository.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(u =>
            u.UserName == registerDto.Username &&
            u.Email == registerDto.Email &&
            u.PasswordHash == "hashed_password" &&
            u.EmailConfirmed == false &&
            u.EmailVerificationToken == "verification-token")), Times.Once);
        _mockEmailService.Verify(x =>
            x.SendVerificationEmailAsync(registerDto.Email, "verification-token"), Times.Once);
    }

    [TestMethod]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "existing@myfood.test",
            Password = "Strong@123"
        };

        _mockPasswordService
            .Setup(x => x.IsPasswordStrong(registerDto.Password))
            .Returns(true);
        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync(registerDto.Username))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(registerDto.Email))
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
            Username = "user1",
            Password = "Wrong@123"
        };

        var existingUser = new ApplicationUser
        {
            UserName = loginDto.Username,
            EmailConfirmed = true,
            PasswordHash = "stored_hash"
        };

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync(existingUser);
        _mockPasswordService
            .Setup(x => x.VerifyPassword(loginDto.Password, "stored_hash"))
            .Returns(false);

        var result = await _authService.LoginAsync(loginDto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Invalid username or password.", result.Message);
        Assert.IsNull(result.Token);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        var loginDto = new LoginDto
        {
            Username = "user1",
            Password = "Valid@123"
        };

        var existingUser = new ApplicationUser
        {
            Id = "u-1",
            UserName = loginDto.Username,
            Email = "user1@myfood.test",
            EmailConfirmed = true,
            PasswordHash = "stored_hash"
        };

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync(existingUser);
        _mockPasswordService
            .Setup(x => x.VerifyPassword(loginDto.Password, "stored_hash"))
            .Returns(true);
        _mockUserRepository
            .Setup(x => x.GetRolesAsync(existingUser))
            .ReturnsAsync(new List<string> { "User" });
        _mockTokenService
            .Setup(x => x.GenerateToken(existingUser, It.IsAny<IEnumerable<string>>()))
            .Returns("jwt-token");

        var result = await _authService.LoginAsync(loginDto);

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Login successful.", result.Message);
        Assert.AreEqual("jwt-token", result.Token);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(existingUser.UserName, result.User.Username);
        Assert.AreEqual(existingUser.Email, result.User.Email);
    }

    [TestMethod]
    public async Task LoginAsync_WithUnverifiedEmail_ShouldReturnError()
    {
        var loginDto = new LoginDto
        {
            Username = "user2",
            Password = "Valid@123"
        };

        var existingUser = new ApplicationUser
        {
            UserName = loginDto.Username,
            EmailConfirmed = false,
            PasswordHash = "stored_hash"
        };

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync(existingUser);
        _mockPasswordService
            .Setup(x => x.VerifyPassword(loginDto.Password, "stored_hash"))
            .Returns(true);

        var result = await _authService.LoginAsync(loginDto);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Email is not verified. Please verify your email before logging in.", result.Message);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }
}
