namespace LoginDotnet.Models.Entities
{
    public class UserActivityLog
    {
        public int? UserId { get; set; }
        public string Email { get; set; }
        public ActivityType ActivityType { get; set; }

        public bool result { get; set; }

        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }   
    }
    public enum ActivityType
    {
        Login = 0,
        Logout = 1,
        PasswordChange = 2,
        ProfileUpdate = 3,
        Register = 4
    }
}
