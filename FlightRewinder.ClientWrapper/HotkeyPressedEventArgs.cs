using System;

namespace FlightRewinder.ClientWrapper
{
    public class HotkeyPressedEventArgs :  EventArgs
    {
        public int HotkeyID;
        public uint VirtualKey;
        public uint KeyModifier;

        public HotkeyPressedEventArgs(int id, uint key, uint mod)
        {
            HotkeyID = id;
            KeyModifier = mod;
            VirtualKey = key;
        }
    }
}