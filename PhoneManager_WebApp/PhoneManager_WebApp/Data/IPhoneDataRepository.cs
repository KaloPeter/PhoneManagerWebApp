using PhoneManager_WebApp.Models;

namespace PhoneManager_WebApp.Data
{
    public interface IPhoneDataRepository
    {
       PhoneDataViewBag ReadAllPhoneData();
    }
}
