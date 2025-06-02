namespace beauty_salon
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new AuthForm());
        }
    }

    internal static class Constants
    {
        internal static string ConnectionString = "Host=localhost;Port=5432;Database=beauty_salon;Username=postgres;Password=1111";
        internal static Color PrimaryBackgroundColor = ColorTranslator.FromHtml("#FFFFFF");
        internal static Color SecondaryBackgroundColor = ColorTranslator.FromHtml("#021BB2");
        internal static Color AccentColor = ColorTranslator.FromHtml("#FBCDCB");
    }
}