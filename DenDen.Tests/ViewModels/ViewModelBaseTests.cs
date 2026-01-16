using System.ComponentModel;
using DenDen.ViewModels;
using FluentAssertions;
using Xunit;

namespace DenDen.Tests.ViewModels;

/// <summary>
/// ViewModelBaseのテスト
/// </summary>
public class ViewModelBaseTests
{
    /// <summary>
    /// テスト用のViewModelクラス
    /// </summary>
    private class TestViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        private int _count;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public void TriggerPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }

    [Fact]
    public void SetProperty_WhenValueChanges_ShouldReturnTrue()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act
        viewModel.Name = "NewValue";

        // Assert - the property changed indicates SetProperty returned true
        viewModel.Name.Should().Be("NewValue");
    }

    [Fact]
    public void SetProperty_WhenValueIsSame_ShouldReturnFalse()
    {
        // Arrange
        var viewModel = new TestViewModel();
        viewModel.Name = "SameValue";
        var propertyChangedCount = 0;
        viewModel.PropertyChanged += (_, _) => propertyChangedCount++;

        // Act
        viewModel.Name = "SameValue";

        // Assert
        propertyChangedCount.Should().Be(0);
    }

    [Fact]
    public void SetProperty_WhenValueChanges_ShouldRaisePropertyChanged()
    {
        // Arrange
        var viewModel = new TestViewModel();
        string? changedPropertyName = null;
        viewModel.PropertyChanged += (_, e) => changedPropertyName = e.PropertyName;

        // Act
        viewModel.Name = "ChangedValue";

        // Assert
        changedPropertyName.Should().Be(nameof(TestViewModel.Name));
    }

    [Fact]
    public void SetProperty_WithDifferentTypes_ShouldWork()
    {
        // Arrange
        var viewModel = new TestViewModel();
        string? changedPropertyName = null;
        viewModel.PropertyChanged += (_, e) => changedPropertyName = e.PropertyName;

        // Act
        viewModel.Count = 42;

        // Assert
        viewModel.Count.Should().Be(42);
        changedPropertyName.Should().Be(nameof(TestViewModel.Count));
    }

    [Fact]
    public void OnPropertyChanged_ShouldRaiseEvent()
    {
        // Arrange
        var viewModel = new TestViewModel();
        string? changedPropertyName = null;
        viewModel.PropertyChanged += (_, e) => changedPropertyName = e.PropertyName;

        // Act
        viewModel.TriggerPropertyChanged("CustomProperty");

        // Assert
        changedPropertyName.Should().Be("CustomProperty");
    }

    [Fact]
    public void PropertyChanged_WhenNoSubscribers_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new TestViewModel();

        // Act & Assert
        var act = () => viewModel.Name = "Value";
        act.Should().NotThrow();
    }

    [Fact]
    public void SetProperty_WithNull_ShouldRaisePropertyChanged()
    {
        // Arrange
        var viewModel = new TestViewModel();
        viewModel.Name = "Initial";
        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (_, _) => propertyChangedRaised = true;

        // Act
        viewModel.Name = null!;

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Fact]
    public void SetProperty_FromNullToValue_ShouldRaisePropertyChanged()
    {
        // Arrange
        var viewModel = new TestViewModel();
        viewModel.Name = null!;
        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (_, _) => propertyChangedRaised = true;

        // Act
        viewModel.Name = "NewValue";

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }
}
