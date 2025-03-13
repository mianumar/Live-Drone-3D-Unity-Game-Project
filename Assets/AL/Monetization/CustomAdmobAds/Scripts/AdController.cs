using AL.Utility.InternetCheckAvailability.Scripts;
using UnityEngine;
namespace AL.Monetization.CustomAdmobAds.Scripts
{
    public class AdController : MonoBehaviour
    {
        // Start is called before the first frame update
        protected void OnEnable()
        {

            if  (InternetManager.instance)
                InternetManager.instance.loadAdAfterInternet += CheckAdLoad;

            Debug.Log("parent");
        }
        protected void OnDisable()
        {

            if  (InternetManager.instance)
                InternetManager.instance.loadAdAfterInternet -= CheckAdLoad;

        }

        protected virtual void CheckAdLoad()
        {
            Debug.Log("moaaz");
        }
    }
}