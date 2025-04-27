using UnityEngine;

namespace AL.Monetization.CustomAdmobAds.Scripts
{
    /// <summary>
    /// This script initializes the AdMob SDK and other required components.
    /// Attach this to a GameObject in your first scene.
    /// </summary>
    public class AdInitializer : MonoBehaviour
    {
        [SerializeField] private GameObject _adManagerPrefab;
        [SerializeField] private bool _forceTestMode = false;
        
        private void Awake()
        {
            // Create the AdMobInitializer if it doesn't exist
            if (AdMobInitializer.Instance == null)
            {
                if (_adManagerPrefab != null)
                {
                    Instantiate(_adManagerPrefab);
                }
                else
                {
                    GameObject adManager = new GameObject("AdMobManager");
                    AdMobInitializer initializer = adManager.AddComponent<AdMobInitializer>();
                    
                    if (_forceTestMode)
                    {
                        initializer.AddTestDevice();
                    }
                }
                
                Debug.Log("AdMob SDK initialization started");
            }
            else
            {
                Debug.Log("AdMob SDK already initialized");
            }
        }
    }
} 