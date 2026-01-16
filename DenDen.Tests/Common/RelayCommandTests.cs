using DenDen.Common;
using FluentAssertions;
using Xunit;

namespace DenDen.Tests.Common;

/// <summary>
/// RelayCommandのテスト
/// </summary>
public class RelayCommandTests
{
    #region RelayCommand (パラメータなし) Tests

    [Fact]
    public void Constructor_WithNullExecute_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new RelayCommand(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Execute_ShouldInvokeAction()
    {
        // Arrange
        var wasExecuted = false;
        var command = new RelayCommand(() => wasExecuted = true);

        // Act
        command.Execute(null);

        // Assert
        wasExecuted.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WithNoCanExecuteFunc_ShouldReturnTrue()
    {
        // Arrange
        var command = new RelayCommand(() => { });

        // Act
        var canExecute = command.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WhenCanExecuteFuncReturnsTrue_ShouldReturnTrue()
    {
        // Arrange
        var command = new RelayCommand(() => { }, () => true);

        // Act
        var canExecute = command.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WhenCanExecuteFuncReturnsFalse_ShouldReturnFalse()
    {
        // Arrange
        var command = new RelayCommand(() => { }, () => false);

        // Act
        var canExecute = command.CanExecute(null);

        // Assert
        canExecute.Should().BeFalse();
    }

    [Fact]
    public void CanExecute_ShouldReflectDynamicCondition()
    {
        // Arrange
        var isEnabled = false;
        var command = new RelayCommand(() => { }, () => isEnabled);

        // Act & Assert
        command.CanExecute(null).Should().BeFalse();

        isEnabled = true;
        command.CanExecute(null).Should().BeTrue();
    }

    #endregion

    #region RelayCommand<T> (パラメータあり) Tests

    [Fact]
    public void GenericConstructor_WithNullExecute_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new RelayCommand<string>(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GenericExecute_ShouldInvokeActionWithParameter()
    {
        // Arrange
        string? receivedParameter = null;
        var command = new RelayCommand<string>(p => receivedParameter = p);

        // Act
        command.Execute("test");

        // Assert
        receivedParameter.Should().Be("test");
    }

    [Fact]
    public void GenericExecute_WithIntParameter_ShouldWork()
    {
        // Arrange
        int receivedValue = 0;
        var command = new RelayCommand<int>(p => receivedValue = p);

        // Act
        command.Execute(42);

        // Assert
        receivedValue.Should().Be(42);
    }

    [Fact]
    public void GenericCanExecute_WithNoCanExecuteFunc_ShouldReturnTrue()
    {
        // Arrange
        var command = new RelayCommand<string>(_ => { });

        // Act
        var canExecute = command.CanExecute("test");

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void GenericCanExecute_ShouldReceiveParameter()
    {
        // Arrange
        var command = new RelayCommand<string>(_ => { }, p => p == "allowed");

        // Act & Assert
        command.CanExecute("allowed").Should().BeTrue();
        command.CanExecute("denied").Should().BeFalse();
    }

    [Fact]
    public void GenericCanExecute_WithNullParameter_ShouldWork()
    {
        // Arrange
        var command = new RelayCommand<string?>(_ => { }, p => p == null);

        // Act
        var canExecute = command.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void GenericExecute_WithNullParameter_ShouldWork()
    {
        // Arrange
        string? receivedParameter = "initial";
        var command = new RelayCommand<string?>(p => receivedParameter = p);

        // Act
        command.Execute(null);

        // Assert
        receivedParameter.Should().BeNull();
    }

    #endregion
}
