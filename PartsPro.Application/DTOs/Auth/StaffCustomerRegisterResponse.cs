namespace PartsPro.Application.DTOs.Auth;

// We extend UserDto here because the response needs everything UserDto has,
// plus the auto-generated password so the staff can hand it to the customer
public class StaffCustomerRegisterResponse : UserDto
{
    public string GeneratedPassword { get; set; } = string.Empty;
}
