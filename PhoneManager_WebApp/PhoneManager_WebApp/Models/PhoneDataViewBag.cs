using TelephoneManager.Models;

namespace PhoneManager_WebApp.Models
{
    public class PhoneDataViewBag
    {
        public List<InputData> InputDataList { get; set; }
        public List<Area> AreaList { get; set; }
        public List<Country> CountryList { get; set; }
        public List<OutputData> OutputDataList { get; set; }

        public string OutPutDataJSON { get; set; }

    }
}
