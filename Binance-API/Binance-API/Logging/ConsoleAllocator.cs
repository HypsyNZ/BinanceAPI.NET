using System;
using System.Runtime.InteropServices;

namespace BinanceAPI
{
    /// <summary>
    /// Allocates a Console to the current process
    /// </summary>
    public static class ConsoleAllocator
    {
        [DllImport(@"kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport(@"kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleTitle(string strMessage);

        [DllImport(@"kernel32.dll")]
        private static extern IntPtr FreeConsole();

        [DllImport(@"user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SwHide = 0;
        private const int SwShow = 5;

        /// <summary>
        /// True if the console window is Open
        /// </summary>
        public static bool ConsoleWindowOpen { get; set; } = false;

        private static IntPtr ConsoleHandle = IntPtr.Zero;

        /// <summary>
        /// Allocates a console to the currently running Winforms/WPF application
        /// The console must be started before it can recieve messages
        /// </summary>
        public static void StartConsole()
        {
            ConsoleHandle = GetConsoleWindow();

            if (ConsoleHandle == IntPtr.Zero)
            {
                AllocConsole();

                ConsoleHandle = GetConsoleWindow();
                SetConsoleTitle("Logging Output Console");
                ShowWindow(ConsoleHandle, 0);
            }
        }

        /// <summary>
        /// Initialize and/or Show the Console Window
        /// </summary>
        public static void ShowConsoleWindow()
        {
            try
            {
                ConsoleHandle = GetConsoleWindow();

                if (ConsoleHandle == IntPtr.Zero)
                {
                    AllocConsole();
                }
                else
                {
                    ShowWindow(ConsoleHandle, SwShow);
                }

                ConsoleWindowOpen = true;
            }
            catch { }
        }

        /// <summary>
        /// Hide the Console Window to be shown again later
        /// </summary>
        public static void HideConsoleWindow()
        {
            try
            {
                ConsoleHandle = GetConsoleWindow();

                ShowWindow(ConsoleHandle, SwHide);
                ConsoleWindowOpen = false;
            }
            catch { }
        }

        /// <summary>
        /// Destroys the Console Window if no other processes are using it
        /// </summary>
        public static void DestroyConsole()
        {
            try
            {
                FreeConsole();
            }
            catch { }
        }
    }
}