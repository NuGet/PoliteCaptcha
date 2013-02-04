namespace PoliteCaptcha
{
    public interface IConfigurationSource
    {
        string GetConfigurationValue(string key);
    }
}
