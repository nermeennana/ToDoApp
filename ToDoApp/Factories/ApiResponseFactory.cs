using Microsoft.AspNetCore.Mvc;
using Shared.ErrorModels;

namespace ToDoApp.Factories
{
    public class ApiResponseFactory
    {
        public static IActionResult GenerateApiValidationErrorResponse(ActionContext context)
        {
            var entries = context.ModelState.Where(m => m.Value != null && m.Value.Errors.Any()).ToList();

            if (entries.Any(m => m.Key.StartsWith("$")))
            {
                entries = entries.Where(m => m.Key.StartsWith("$")).ToList();
            }

            var errors = entries
                .Select(m => new ValidationError()
                {
                    Field = m.Key.Replace("$.", ""),
                    Errors = m.Value!.Errors.Select(e => GetFriendlyMessage(m.Key, e.ErrorMessage)).ToList()
                });
            var response = new ValidationErrorToReturn()
            {
                ValidationErrors = errors
            };
            return new BadRequestObjectResult(response);
        }

        private static string GetFriendlyMessage(string key, string message)
        {
            if (key.StartsWith("$"))
            {
                if (message.Contains("DateTime"))
                {
                    return "Invalid date. Use the format YYYY-MM-DD, for example 2026-10-14.";
                }
                else
                {
                    return "The value is not valid.";
                }
            }
            else
            {
                return message;
            }
        }
    }
}
