using System.Runtime.Serialization;

namespace TelephoneManager.Models
{

    public class OutputData
    {
        public string CountryName { get; set; }
        public string AreaName { get; set; }
        public int CountryCallingCode { get; set; }
        public int AreaCallingCode { get; set; }
        public int SumCallingTime { get; set; }//sec
    }
}
