using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

class RebootX
{
    
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    private static extern bool GetMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const uint MOD_SHIFT = 0x0004;
    const uint MOD_CONTROL = 0x0002;
    const int HOTKEY_ID_REBOOT = 1;
    const int HOTKEY_ID_SHUTDOWN = 2;
    private const int WM_HOTKEY = 0x0312;

    const int SW_MINIMIZE = 6;

    static void Main()
    {
        Console.WriteLine("RebootX by Duremih");
        Console.WriteLine("Комбинации клавиш:");
        Console.WriteLine("Ctrl + Shift + R Перезагрузка");
        Console.WriteLine("Ctrl + Shift + S Выключение");

        Thread.Sleep(10000);

        IntPtr handle = GetConsoleWindow();
        ShowWindow(handle, SW_MINIMIZE);

        RegisterHotKey(IntPtr.Zero, HOTKEY_ID_REBOOT, MOD_CONTROL | MOD_SHIFT, (uint)ConsoleKey.R);
        RegisterHotKey(IntPtr.Zero, HOTKEY_ID_SHUTDOWN, MOD_CONTROL | MOD_SHIFT, (uint)ConsoleKey.S);

        Thread monitorThread = new Thread(SystemMonitor);
        monitorThread.IsBackground = true;
        monitorThread.Start();

        NativeMessage msg = new NativeMessage();
        while (true)
        {
            if (GetMessage(out msg, IntPtr.Zero, 0, 0))
            {
                if (msg.message == WM_HOTKEY)
                {
                    if ((int)msg.wParam == HOTKEY_ID_REBOOT)
                        Reboot();
                    else if ((int)msg.wParam == HOTKEY_ID_SHUTDOWN)
                        Shutdown();
                }
            }
        }
    }

    static void SystemMonitor()
    {
        while (true)
        {
            try
            {
                if (Process.GetProcessesByName("explorer").Length == 0)
                {
                    Console.WriteLine("explorer.exe не отвечает Перезагрузка...");
                    Reboot();
                }
            }
            catch { }
            Thread.Sleep(5000);
        }
    }

    static void Reboot() => Process.Start("shutdown", "/r /f /t 0");
    static void Shutdown() => Process.Start("shutdown", "/s /f /t 0");

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        public IntPtr hWnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public System.Drawing.Point p;
    }
}
