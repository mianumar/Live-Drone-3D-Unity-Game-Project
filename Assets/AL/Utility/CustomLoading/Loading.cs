using System.Collections;
using AL.Monetization.CustomAdmobAds.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace AL.Utility.CustomLoading
{
    public class Loading : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject loadingMenu;
        public Text loadingPercentage;
        private float _loadPercent;
        public OpenApp aM;
        public static bool once;
        void Start()
        {
            _loadPercent = 0;
            StartCoroutine(ShowOpenAd());
        }

        // Update is called once per frame
        void Update()
        {
            _loadPercent += 100 * Time.deltaTime / 5;
            loadingPercentage.text = Mathf.Floor(_loadPercent) + "%";
            if (_loadPercent >= 100 || once)
            {
                loadingMenu.SetActive(false);
            }
        }

        IEnumerator ShowOpenAd()
        {
            yield return new WaitForSeconds(4f);
            if (!once)
            {
          
                once = true;
                aM.ShowAppOpenAd();
                Debug.Log("aaa");
                PlayerPrefs.SetInt("openApp",PlayerPrefs.GetInt("openApp")+1);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            once = false;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            once = false;
        }

        private void OnApplicationQuit()
        {
            once = false;
        }

        private void OnDidApplyAnimationProperties()
        {
            once = false;
        }
    }
}