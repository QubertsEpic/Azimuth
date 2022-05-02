using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinder.ClientWrapper
{
    internal class Hotkeys
    {
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        List<int> RegisteredHotkeyIds = new List<int>();

        public event EventHandler<HotkeyPressedEventArgs>? KeyPressed;

        public IntPtr WindowHandle;

        public const int HOTKEY_MESSAGE_ID = 0x0312;

        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CTRL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_NOREPEAT = 0x4000;
        public const uint MOD_WIN = 0x0008;

        public Hotkeys(IntPtr windowHandle)
        {
            WindowHandle = windowHandle;
        }

        public void HandleMessages(IntPtr wParam, IntPtr iParam)
        {
            int id = (UInt16)wParam;
            uint key = (uint)((long)iParam >> 16);
            uint mod = (UInt16)iParam;
            KeyPressed?.Invoke(this, new(id, key, mod));
        }

        public bool UnregisterHotKey(int id) 
        {
            if (!RegisteredHotkeyIds.Contains(id))
                return false;
            RegisteredHotkeyIds.Remove(id);
            return UnregisterHotKey(WindowHandle, id); 
        }

        public bool RegisterHotKey(int id, uint fsModifiers, uint vk)
        {
            if (!RegisteredHotkeyIds.Contains(id))
                RegisteredHotkeyIds.Add(id);
            return RegisterHotKey(WindowHandle, id, fsModifiers, vk);
        }

        public void RegisterHotKeys(int id, params (uint modifier, uint virtualKey)[] keys)
        {
            foreach ((uint mod, uint virtKey) in keys)
            {
                RegisterHotKey(id, mod, virtKey);
            }
        }
    }
}
