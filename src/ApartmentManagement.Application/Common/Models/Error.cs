namespace ApartmentManagement.Application.Common.Models;

public record Error(string Code, string Message)
{
    public static Error NotFound(string entity) => new("NotFound", $"{entity} not found.");
    public static Error Validation(string message) => new("Validation", message);
    public static Error Conflict(string message) => new("Conflict", message);
    public static Error Unauthorized(string message = "You are not authorized to perform this operation.") => new("Unauthorized", message);
    public static Error Forbidden(string message = "Access to this resource is forbidden.") => new("Forbidden", message);
    public static Error Failure(string message) => new("Failure", message);
}
