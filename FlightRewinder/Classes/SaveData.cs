using FlightRewinderData.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlightRewinderData.Classes
{
    public class SaveData
    {
        public string? Title;
        public long? StartingTime;
        public long? EndingTime;
        public List<RecordedFrame>? Frames;
        
        public SaveData(string aircraftTitle, long startingTime, long endingTime, List<RecordedFrame> frames)
        {
            Title = aircraftTitle;
            StartingTime = startingTime;
            EndingTime = endingTime;
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
