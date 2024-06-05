namespace ContactApi.ViewModel
{
    public class UserRegisterDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class UserLoginDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class ResetPasswordDto
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}