namespace Ecommerce.DTOs;

public class ProfileUpdateDTO
{
    public string? Username { get; set; }

    public string? PhoneNumber { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? BusinessName { get; set; }

    public string? BusinessAddress { get; set; }

    public string? Gstnumber { get; set; }

    public string? PartnerName { get; set; }

    public string? VehicleNumber { get; set; }

    public string? WarehouseName { get; set; }

    public string? WarehouseLocation { get; set; }

    public string? WarehouseContactNumber { get; set; }
}
