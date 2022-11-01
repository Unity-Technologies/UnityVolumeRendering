#if !DT_EXCLUDE_SAMPLES
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.DigitalTwins.Storage.Samples
{
    [DefaultExecutionOrder(int.MinValue)]
    public class PlatformServicesInitialization : MonoBehaviour
    {
        async Task Awake()
        {
            DontDestroyOnLoad(gameObject);
            await PlatformServices.InitializeAsync();
        }
    }
}
#endif
