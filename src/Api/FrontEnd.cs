namespace Api;

// The site that calls this API with the session cookie, such as https://shift.developkerr.com.
public record FrontEnd(string Origin)
{
    public const string CorsPolicy = "FrontEnd";

    public static FrontEnd From(IConfiguration configuration) =>
        new(configuration["FrontEnd:Origin"] ?? throw new InvalidOperationException("FrontEnd:Origin isn't set."));
}
