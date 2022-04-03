using FlightRewinder.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlightRewinder.Classes
{
    public class SaveData
    {
        public string? Title;
        public List<RecordedFrame>? Frames;
        
        public SaveData(string aircraftTitle, List<RecordedFrame> frames)
        {
            Title = aircraftTitle;
            Frames = frames;
        }

        public static string ToJson(SaveData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Cannot use null instance.");
            return JsonConvert.SerializeObject(data);
        }
    }
}
