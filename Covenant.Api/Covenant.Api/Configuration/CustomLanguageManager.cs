namespace Covenant.Api.Configuration
{
    public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
    {
        public CustomLanguageManager()
        {
            AddTranslation("en", "NotNullValidator", "This is required {PropertyName}");
            AddTranslation("es", "NotNullValidator", "Este es requerido {PropertyName}");
        }
    }
}