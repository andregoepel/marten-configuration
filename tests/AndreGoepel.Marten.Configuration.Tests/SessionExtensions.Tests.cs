using Marten;

namespace AndreGoepel.Marten.Configuration.Tests;

public sealed class SessionExtensionsTests
{
    [Fact]
    public async Task WithSessionAsync_NullStore_ThrowsBeforeInvokingWork()
    {
        // Arrange
        IDocumentStore store = null!;
        var invoked = false;

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            store.WithSessionAsync(
                (_, _) =>
                {
                    invoked = true;
                    return Task.CompletedTask;
                },
                TestContext.Current.CancellationToken
            )
        );

        // Assert
        Assert.Equal("store", exception.ParamName);
        Assert.False(invoked);
    }

    [Fact]
    public async Task WithSessionAsync_WithResultAndNullStore_ThrowsBeforeInvokingWork()
    {
        // Arrange
        IDocumentStore store = null!;
        var invoked = false;

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            store.WithSessionAsync(
                (_, _) =>
                {
                    invoked = true;
                    return Task.FromResult(42);
                },
                TestContext.Current.CancellationToken
            )
        );

        // Assert
        Assert.Equal("store", exception.ParamName);
        Assert.False(invoked);
    }

    [Fact]
    public async Task LoadOrDefaultAsync_NullSession_Throws()
    {
        // Arrange
        IQuerySession session = null!;

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            session.LoadOrDefaultAsync<object>("settings", TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Equal("session", exception.ParamName);
    }

    [Fact]
    public async Task StoreAndSaveAsync_NullStore_Throws()
    {
        // Arrange
        IDocumentStore store = null!;

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            store.StoreAndSaveAsync(new object(), TestContext.Current.CancellationToken)
        );

        // Assert
        Assert.Equal("store", exception.ParamName);
    }
}
