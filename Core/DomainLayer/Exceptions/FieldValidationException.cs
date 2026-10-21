using System;

namespace DomainLayer.Exceptions
{
    public class FieldValidationException(string fieldName, string message) : Exception(message)
    {
        public string Field { get; } = fieldName;
    }
}

