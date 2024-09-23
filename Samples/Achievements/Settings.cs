namespace Tinkering
{
    public class Settings
    {
        //Database configuration
        public const string Database = "ace_achievement";
        public const string Host = "127.0.0.1";
        public const string Port = "3306";
        public const string Username = "root";
        public const string Password = "";

        public int Interval { get; set; } = 10;
        public int Multiplier { get; set; } = 7;
    }
}