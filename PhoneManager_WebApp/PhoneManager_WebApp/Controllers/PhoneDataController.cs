using Microsoft.AspNetCore.Mvc;
using PhoneManager_WebApp.Data;


namespace PhoneManager_WebApp.Controllers
{
    public class PhoneDataController : Controller
    {
        private readonly IPhoneDataRepository _repoPhoneData;

        public PhoneDataController(IPhoneDataRepository _repoPhoneData)
        {
            this._repoPhoneData = _repoPhoneData;
        }

        public IActionResult Index()
        {
            return View(_repoPhoneData.ReadAllPhoneData());
        }


    }
}
