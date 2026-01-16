using DenDen.Data.Repositories;
using DenDen.Models.DTOs;
using DenDen.Models.Entities;
using DenDen.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace DenDen.Tests.Services;

/// <summary>
/// AuthenticationServiceのテスト
/// </summary>
public class AuthenticationServiceTests
{
    private readonly Mock<IOperatorRepository> _mockOperatorRepository;
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly AuthenticationService _sut;

    public AuthenticationServiceTests()
    {
        _mockOperatorRepository = new Mock<IOperatorRepository>();
        _mockProjectRepository = new Mock<IProjectRepository>();
        _sut = new AuthenticationService(_mockOperatorRepository.Object, _mockProjectRepository.Object);
    }

    #region HashPassword Tests

    [Fact]
    public void HashPassword_ShouldReturnNonEmptyHash()
    {
        // Arrange
        var password = "testPassword123";

        // Act
        var hash = _sut.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().StartWith("$2");  // BCryptのハッシュは$2で始まる
    }

    [Fact]
    public void HashPassword_SamePassword_ShouldReturnDifferentHashes()
    {
        // Arrange
        var password = "testPassword123";

        // Act
        var hash1 = _sut.HashPassword(password);
        var hash2 = _sut.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2);  // Salt が異なるため毎回異なるハッシュ
    }

    #endregion

    #region VerifyPassword Tests

    [Fact]
    public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "testPassword123";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "testPassword123";
        var wrongPassword = "wrongPassword";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_InvalidHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "testPassword123";
        var invalidHash = "invalidhash";

        // Act
        var result = _sut.VerifyPassword(password, invalidHash);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_EmptyProjectId_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            ProjectId = 0,
            LoginId = "admin",
            Password = "password"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("案件を選択してください。");
    }

    [Fact]
    public async Task LoginAsync_EmptyLoginId_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            ProjectId = 1,
            LoginId = "",
            Password = "password"
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("ログインIDを入力してください。");
    }

    [Fact]
    public async Task LoginAsync_EmptyPassword_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            ProjectId = 1,
            LoginId = "admin",
            Password = ""
        };

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("パスワードを入力してください。");
    }

    [Fact]
    public async Task LoginAsync_ProjectNotFound_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            ProjectId = 999,
            LoginId = "admin",
            Password = "password"
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((ProjectMaster?)null);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("選択された案件が見つかりません。");
    }

    [Fact]
    public async Task LoginAsync_ProjectInactive_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            ProjectId = 1,
            LoginId = "admin",
            Password = "password"
        };

        var inactiveProject = new ProjectMaster
        {
            ProjectID = 1,
            ProjectName = "Test Project",
            IsActive = false
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(inactiveProject);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("選択された案件は現在無効です。");
    }

    [Fact]
    public async Task LoginAsync_OperatorNotFound_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            ProjectId = 1,
            LoginId = "unknownuser",
            Password = "password"
        };

        var activeProject = new ProjectMaster
        {
            ProjectID = 1,
            ProjectName = "Test Project",
            IsActive = true
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(activeProject);

        _mockOperatorRepository.Setup(x => x.GetByLoginIdAsync(1, "unknownuser"))
            .ReturnsAsync((OperatorMaster?)null);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("ログインIDまたはパスワードが正しくありません。");
    }

    [Fact]
    public async Task LoginAsync_OperatorInactive_ShouldReturnError()
    {
        // Arrange
        var password = "password123";
        var request = new LoginRequest
        {
            ProjectId = 1,
            LoginId = "admin",
            Password = password
        };

        var activeProject = new ProjectMaster
        {
            ProjectID = 1,
            ProjectName = "Test Project",
            IsActive = true
        };

        var inactiveOperator = new OperatorMaster
        {
            OperatorID = 1,
            LoginID = "admin",
            PasswordHash = _sut.HashPassword(password),
            IsActive = false
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(activeProject);

        _mockOperatorRepository.Setup(x => x.GetByLoginIdAsync(1, "admin"))
            .ReturnsAsync(inactiveOperator);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("このアカウントは無効化されています。");
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest
        {
            ProjectId = 1,
            LoginId = "admin",
            Password = "wrongpassword"
        };

        var activeProject = new ProjectMaster
        {
            ProjectID = 1,
            ProjectName = "Test Project",
            IsActive = true
        };

        var activeOperator = new OperatorMaster
        {
            OperatorID = 1,
            LoginID = "admin",
            PasswordHash = _sut.HashPassword("correctpassword"),
            IsActive = true
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(activeProject);

        _mockOperatorRepository.Setup(x => x.GetByLoginIdAsync(1, "admin"))
            .ReturnsAsync(activeOperator);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("ログインIDまたはパスワードが正しくありません。");
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var password = "password123";
        var request = new LoginRequest
        {
            ProjectId = 1,
            LoginId = "admin",
            Password = password
        };

        var activeProject = new ProjectMaster
        {
            ProjectID = 1,
            ProjectName = "Test Project",
            IsActive = true
        };

        var activeOperator = new OperatorMaster
        {
            OperatorID = 1,
            LoginID = "admin",
            OperatorName = "Administrator",
            PasswordHash = _sut.HashPassword(password),
            IsActive = true
        };

        _mockProjectRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(activeProject);

        _mockOperatorRepository.Setup(x => x.GetByLoginIdAsync(1, "admin"))
            .ReturnsAsync(activeOperator);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNullOrEmpty();
        result.Operator.Should().NotBeNull();
        result.Operator!.OperatorName.Should().Be("Administrator");
        result.Project.Should().NotBeNull();
        result.Project!.ProjectName.Should().Be("Test Project");
    }

    #endregion
}
