
namespace ELANAPI
{
    public class Rootobject
    {
        public Event[] events { get; set; }
        public Browse browse { get; set; }
        public object messages { get; set; }
        public object controls { get; set; }
    }

    public class Browse
    {
        public int Total { get; set; }
        public bool Ok { get; set; }
        public object TextOrErrorMessage { get; set; }
        public int Start { get; set; }
        public Item[] Items { get; set; }
        public string[] ExtraAttributes { get; set; }
        public string Caption { get; set; }
        public bool SupportsNowPlaying { get; set; }
        public bool AlphaSort { get; set; }
        public string MessageId { get; set; }
        public int TimeoutInMilliseconds { get; set; }
        public int MsgSource { get; set; }
    }

    public class Item
    {
        public string Guid { get; set; }
        public object Handler { get; set; }
        public Extraattributes ExtraAttributes { get; set; }
        public bool HasChildren { get; set; }
        public bool IsNowPlaying { get; set; }
        public bool IsTuneBridgeSearch { get; set; }
        public bool NeedSingleInputBox { get; set; }
        public bool NeedMultipleInputBoxes { get; set; }
        public bool IsTopMenuItem { get; set; }
        public bool IsNoop { get; set; }
        public object SubTitle { get; set; }
        public object Service { get; set; }
        public string MediaInfoButtonState { get; set; }
        public bool LegacyControl4TextMode { get; set; }
        public string ArtUrl { get; set; }
        public string ArtGuid { get; set; }
        public object Action { get; set; }
        public int EnqueueModeFlags { get; set; }
        public int FieldCount { get; set; }
        public string Value { get; set; }
    }

    public class Extraattributes
    {
    }

    public class Event
    {
        public string name { get; set; }
        public object value { get; set; }
    }
}