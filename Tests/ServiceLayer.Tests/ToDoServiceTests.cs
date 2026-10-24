using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Exceptions;
using DomainLayer.Models;
using Moq;
using Shared.DataTransferObjects;
using Shared.Enums;

namespace ServiceLayer.Tests
{
    public class ToDoServiceTests
    {
        [Fact]
        // Method Name: what we test_in which case_expected result
        public async Task CreateToDoAsync_WithPastDueDate_ThrowsFieldValidationException()
        {
            // Arrange
            var repositoryMock = new Mock<IToDoRepository>();
            var service = new ToDoService(repositoryMock.Object);
            var dto = new CreateToDoDto
            {
                Title = "Test",
                DueDate = DateTime.UtcNow.AddDays(-1) // Past date
            };

            // Act + Assert
            await Assert.ThrowsAsync<FieldValidationException>(() => service.CreateToDoAsync(dto));
        }

        [Fact]
        public async Task CreateToDoAsync_WithoutPriority_CreatesPendingMediumTask()
        {
            // Arrange (جهّز): Create a mock repository and the service takes the mock repository as a dependency.
            var repositoryMock = new Mock<IToDoRepository>();
            var service = new ToDoService(repositoryMock.Object);
            // Create a DTO without specifying the priority (it will be null).
            var dto = new CreateToDoDto { Title = "Test" };
        
            // Act (نفّذ): Call the service method to create a ToDo item.
            var result = await service.CreateToDoAsync(dto);
        
            // Assert (تحقّق): Verify that the result has the expected default values for priority and status, and that the repository's SaveChangesAsync method was called once.
            Assert.Equal(ToDoPriority.Medium, result.Priority);
            Assert.Equal(ToDoStatus.Pending, result.Status);
            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
        [Fact]
        public async Task UpdateToDoAsync_WithUnchangedPastDueDate_SavesChanges()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pastDate = DateTime.UtcNow.Date.AddDays(-5);
            var existingTodo = new ToDo { Id = id, Title = "Old title", DueDate = pastDate };
        
            var repositoryMock = new Mock<IToDoRepository>();
            repositoryMock
                .Setup(r => r.GetToDoByIdAsync(id))
                .ReturnsAsync(existingTodo);
            var service = new ToDoService(repositoryMock.Object);
        
            var dto = new UpdateToDoDto
            {
                Title = "New title",
                DueDate = pastDate
            };
        
            // Act
            await service.UpdateToDoAsync(id, dto);
        
            // Assert
            Assert.Equal("New title", existingTodo.Title);
            repositoryMock.Verify(r => r.GetToDoByIdAsync(id), Times.Once);
            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}