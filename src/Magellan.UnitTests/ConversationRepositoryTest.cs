using Magellan.DataAccess;
using Magellan.Domain;
using Magellan.Domain.Exceptions;
using Microsoft.Azure.Cosmos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class ConversationRepositoryTests
{
    [Fact]
    public async Task GetConversationsAsync_ReturnsConversations()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var expectedConversations = new List<ConversationEntity>
        {
            new ConversationEntity { Id = Guid.NewGuid(), Title = "Test Conversation" }
        };

        var userEntity = new UserEntity
        {
            Id = userId,
            Conversations = expectedConversations
        };

        var containerMock = new Mock<Container>();
        var feedResponseMock = new Mock<FeedResponse<UserEntity>>();
        var feedIteratorMock = new Mock<FeedIterator<UserEntity>>();

        feedResponseMock.Setup(_ => _.GetEnumerator()).Returns(new List<UserEntity> { userEntity }.GetEnumerator());

        feedIteratorMock.Setup(_ => _.HasMoreResults).Returns(true);
        feedIteratorMock.SetupSequence(_ => _.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedResponseMock.Object);
    
        containerMock.Setup(_ => _.GetItemQueryIterator<UserEntity>(
                It.IsAny<QueryDefinition>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
            .Returns(feedIteratorMock.Object);

        var conversationRepository = new ConversationRepository(containerMock.Object);

        // Act
        var conversations = await conversationRepository.GetConversationsAsync(userId);

        // Assert
        Assert.NotNull(conversations);
        Assert.Equal(expectedConversations.Count, conversations.Count());
        Assert.Equal(expectedConversations.First().Title, conversations.First().Title);
    }

}