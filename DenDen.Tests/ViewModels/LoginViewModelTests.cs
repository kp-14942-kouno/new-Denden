using System.Collections.ObjectModel;
using DenDen.Data.Repositories;
using DenDen.Models.DTOs;
using DenDen.Models.Entities;
using DenDen.Services;
using DenDen.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace DenDen.Tests.ViewModels;

/// <summary>
/// LoginViewModelのテスト
/// </summary>
public class LoginViewModelTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly Mock<IDialogService> _mockDialogService;
    private readonly SessionManager _sessionManager;
    private readonly LoginViewModel _sut;

    public LoginViewModelTests()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();
        _mockDialogService = new Mock<IDialogService>();
        _sessionManager = new SessionManager();

        _sut = new LoginViewModel(
            _mockProjectRepository.Object,
            _mockAuthService.Object,
            _sessionManager,
            _mockDialogService.Object);
    }

    #region Property Tests

    [Fact]
    public void LoginId_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_sut.LoginId))
                propertyChangedRaised = true;
        };

        // Act
        _sut.LoginId = "testuser";

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _sut.LoginId.Should().Be("testuser");
    }

    [Fact]
    public void Password_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_sut.Password))
                propertyChangedRaised = true;
        };

        // Act
        _sut.Password = "secret";

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _sut.Password.Should().Be("secret");
    }

    [Fact]
    public void SelectedProject_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_sut.SelectedProject))
                propertyChangedRaised = true;
        };

        var project = new ProjectMaster { ProjectID = 1, ProjectName = "Test" };

        // Act
        _sut.SelectedProject = project;

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _sut.SelectedProject.Should().Be(project);
    }

    [Fact]
    public void IsLoading_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_sut.IsLoading))
                propertyChangedRaised = true;
        };

        // Act
        _sut.IsLoading = true;

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _sut.IsLoading.Should().BeTrue();
    }

    [Fact]
    public void ErrorMessage_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_sut.ErrorMessage))
                propertyChangedRaised = true;
        };

        // Act
        _sut.ErrorMessage = "エラーが発生しました";

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _sut.ErrorMessage.Should().Be("エラーが発生しました");
    }

    [Fact]
    public void LoginId_WhenChanged_ShouldClearErrorMessage()
    {
        // Arrange
        _sut.ErrorMessage = "Previous error";

        // Act
        _sut.LoginId = "newuser";

        // Assert
        _sut.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public void Password_WhenChanged_ShouldClearErrorMessage()
    {
        // Arrange
        _sut.ErrorMessage = "Previous error";

        // Act
        _sut.Password = "newpassword";

        // Assert
        _sut.ErrorMessage.Should().BeEmpty();
    }

    #endregion

    #region LoadProjectsAsync Tests

    [Fact]
    public async Task LoadProjectsAsync_ShouldLoadProjects()
    {
        // Arrange
        var projects = new List<ProjectMaster>
        {
            new ProjectMaster { ProjectID = 1, ProjectName = "Project 1" },
            new ProjectMaster { ProjectID = 2, ProjectName = "Project 2" }
        };

        _mockProjectRepository.Setup(x => x.GetAllActiveAsync())
            .ReturnsAsync(projects);

        // Act
        await _sut.LoadProjectsAsync();

        // Assert
        _sut.Projects.Should().HaveCount(2);
        _sut.SelectedProject.Should().NotBeNull();
        _sut.SelectedProject!.ProjectID.Should().Be(1);
    }

    [Fact]
    public async Task LoadProjectsAsync_EmptyList_ShouldNotSelectAnyProject()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetAllActiveAsync())
            .ReturnsAsync(new List<ProjectMaster>());

        // Act
        await _sut.LoadProjectsAsync();

        // Assert
        _sut.Projects.Should().BeEmpty();
        _sut.SelectedProject.Should().BeNull();
    }

    [Fact]
    public async Task LoadProjectsAsync_WhenException_ShouldShowError()
    {
        // Arrange
        _mockProjectRepository.Setup(x => x.GetAllActiveAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        await _sut.LoadProjectsAsync();

        // Assert
        _mockDialogService.Verify(x => x.ShowError(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task LoadProjectsAsync_ShouldSetIsLoadingDuringLoad()
    {
        // Arrange
        var loadingStates = new List<bool>();
        _sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_sut.IsLoading))
                loadingStates.Add(_sut.IsLoading);
        };

        _mockProjectRepository.Setup(x => x.GetAllActiveAsync())
            .ReturnsAsync(new List<ProjectMaster>());

        // Act
        await _sut.LoadProjectsAsync();

        // Assert
        loadingStates.Should().Contain(true);
        loadingStates.Last().Should().BeFalse();
    }

    #endregion

    #region LoginCommand Tests

    [Fact]
    public void LoginCommand_ShouldNotBeNull()
    {
        // Assert
        _sut.LoginCommand.Should().NotBeNull();
    }

    [Fact]
    public void ExitCommand_ShouldNotBeNull()
    {
        // Assert
        _sut.ExitCommand.Should().NotBeNull();
    }

    #endregion

    #region Initial State Tests

    [Fact]
    public void Constructor_ShouldInitializeWithEmptyProjects()
    {
        // Assert
        _sut.Projects.Should().NotBeNull();
        _sut.Projects.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithEmptyLoginId()
    {
        // Assert
        _sut.LoginId.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithEmptyPassword()
    {
        // Assert
        _sut.Password.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithFalseIsLoading()
    {
        // Assert
        _sut.IsLoading.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithEmptyErrorMessage()
    {
        // Assert
        _sut.ErrorMessage.Should().BeEmpty();
    }

    #endregion
}
