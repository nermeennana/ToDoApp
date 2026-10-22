using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Exceptions;
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
    }
}