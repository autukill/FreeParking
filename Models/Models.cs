namespace FreeParkingMaui.Models;

public class LoginResponse
{
    public Data Data { get; set; }
    public int Status { get; set; }
    public string Message { get; set; }
}

public class Data
{
    public string CustomerId { get; set; }
    public string NickName { get; set; }
    public string Phone { get; set; }
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string Issued { get; set; }
    public string Expires { get; set; }
}

public class VerificationCodeResponse
{
    public int Status { get; set; }
    public string Message { get; set; }
}

public class OrderResponse
{
    public OrderData Data { get; set; }
    public int Status { get; set; }
    public string Message { get; set; }
}

public class OrderData
{
    public string Code { get; set; }
    public string CarPointCode { get; set; }
    public string AreaName { get; set; }
    public string Car { get; set; }
    public string ParkName { get; set; }
    public string CarImage { get; set; }
    public string CarImageFull { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string UseTime { get; set; }
    public int State { get; set; }
    public string StateName { get; set; }
    public decimal OrderPrice { get; set; }
    public string PaymentTime { get; set; }
}

public class Area
{
    public string Id { get; set; }
    public string Name { get; set; }
}
