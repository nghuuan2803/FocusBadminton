namespace Domain.Common
{
    public sealed record Error(string Code, string Description)
    {
        public static readonly Error None = new(string.Empty, string.Empty);

        public static Error NotFound(string entityName, string id) =>
        new("NotFound", $"{entityName} with ID {id} was not found.");

        public static Error Validation(string description) =>
            new("ValidationError", description);

        public static Error Conflict(string description) =>
            new("Conflict", description);

        public static Error Unauthorized(string description) =>
            new("Unauthorized", description);
        public static Error UnknowError(string description) =>
            new("UnknowError", description);
    }
}
