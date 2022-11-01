#if !DT_EXCLUDE_SAMPLES
using UnityEngine;

namespace Unity.DigitalTwins.Annotation.Samples
{
    [DefaultExecutionOrder(int.MaxValue)]
    public class PlatformServicesShutdown : MonoBehaviour
    {
        void OnDestroy()
        {
            PlatformServices.Shutdown();
        }
    }
}
#endif
