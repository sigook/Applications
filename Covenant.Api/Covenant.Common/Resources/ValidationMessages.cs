namespace Covenant.Common.Resources
{
    public static class ValidationMessages
    {
        public static string RequiredMsg(string propertyName) => $"'{propertyName}' must not be empty.";
        public static string InvalidFormatMsg(string propertyName) => $"'{propertyName}' is not in the correct format.";
        public static string LengthMsg(string propertyName, int min, int max) => $"'{propertyName}' must be between {min} and {max} characters.";
        public static string BetweenMsg<T>(string propertyName, T min, T max) => $"'{propertyName}' must be between {min} and {max}.";
        public static string LengthMaxMsg(string propertyName, int max) => $"The length of '{propertyName}' must be {max} characters or fewer.";
        public static string AgeMsg(int age) => $"You must be at least {age} years old.";
        public static string LessThanMsg<T>(string propertyName, T value) => $"'{propertyName}' must be less than '{value}'.";
        public static string LessThanOrEqualMsg<T>(string propertyName, T value) => $"'{propertyName}' must be less than or equal to '{value}'.";
        public static string GreaterThan<T>(string propertyName, T value) => $"'{propertyName}' must be greater than '{value}'.";
        public static string GreaterThanOrEqualMsg<T>(string propertyName, T value) => $"'{propertyName}' must be greater than or equal to '{value}'.";
        public static string AlreadyExists(string propertyName) => $"'{propertyName}' already exists.";
    }
}
