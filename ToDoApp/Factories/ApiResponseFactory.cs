using Microsoft.AspNetCore.Mvc;
using Shared.ErrorModels;

namespace ToDoApp.Factories
{
    public class ApiResponseFactory
    {
        public static IActionResult GenerateApiValidationErrorResponse(ActionContext context)
        {
            var errors = context.ModelState.Where(m => m.Value != null && m.Value.Errors.Any())
                .Select(m => new ValidationError()
                {
                    Field = m.Key,
                    Errors = m.Value!.Errors.Select(e => e.ErrorMessage)
                });
            var response = new ValidationErrorToReturn()
            {
                ValidationErrors = errors
            };
            return new BadRequestObjectResult(response);
        }
    }
}
