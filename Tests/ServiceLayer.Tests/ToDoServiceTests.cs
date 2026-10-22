using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Exceptions;
using Moq;
using Shared.DataTransferObjects;

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
    }
}