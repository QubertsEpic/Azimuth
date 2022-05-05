using System;

namespace Azimuth.ClientWrapper
{
    public class HotkeyPressedEventArgs :  EventArgs
    {
        public int HotkeyID;
        public uint VirtualKey;
        public ConsoleKey KeyCode => (ConsoleKey)VirtualKey; 
        public uint KeyModifier;

        public HotkeyPressedEventArgs(int id, uint key, uint mod)
        {
            HotkeyID = id;
            KeyModifier = mod;
            VirtualKey = key;
        }
    }
}