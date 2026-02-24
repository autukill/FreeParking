using FreeParkingMaui.Models;

namespace FreeParkingMaui;

public static class Constants
{
    public const string ApiBaseUrl = "https://newapi.acyibo.com";
    
    // Auth Token Key for SecureStorage
    public const string AuthTokenKey = "auth_token";
    public const string UserPhoneKey = "user_phone";
    public const string AreaIdKey = "area_id";
    public const string AreaNameKey = "area_name";

    public static readonly List<Area> Areas =
    [
        new Area { Id = "c7bd547b-fb67-4554-a1e5-0842f69f4e56", Name = "华宁县" }
    ];

    public const string SmtpHost = "smtp.qq.com";
    public const int SmtpPort = 465;

    public const string SmtpUserKey = "smtp_user";
    public const string SmtpPasswordKey = "smtp_password";
}
