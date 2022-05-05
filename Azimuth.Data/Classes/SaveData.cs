using Newtonsoft.Json;

namespace Azimuth.Classes
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
