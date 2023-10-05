namespace TelephoneManager.Models
{
    public class InputData
    {
        public DateOnly SourceCallDate { get; set; }
        public TimeOnly SourceCallTime { get; set; }
        public string SourcePhoneNumber { get; set; }
        public string DestinationPhoneNumber { get; set; }
        public int CallingTime { get; set; }//sec
    }
}
