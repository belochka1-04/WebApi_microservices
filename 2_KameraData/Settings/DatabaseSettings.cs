
using static System.String;

namespace KameraData.Settings
{
    public sealed class DatabaseSettings
    {
        /// <summary>
        /// Database Host
        /// </summary>
        public string Host { get; set; } = Empty;

        /// <summary>
        /// Database Login
        /// </summary>
        public string Login { get; set; } = Empty;

        /// <summary>
        /// Database Password
        /// </summary>
        public string Password { get; set; } = Empty;

        /// <summary>
        /// Database Database
        /// </summary>
        public string Database { get; set; } = Empty;

        /// <summary>
        /// Database Port
        /// </summary>
        public int Port { get; set; } = 0;
    }
}
