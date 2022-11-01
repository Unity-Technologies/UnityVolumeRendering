#if !DT_EXCLUDE_SAMPLES
using System.Threading.Tasks;
using Unity.DigitalTwins.Common;
using UnityEngine;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public abstract class AnnotationSampleBehaviour : MonoBehaviour
    {
        public abstract Task InitializeAsync(IScene scene, IAccessTokenProvider accessTokenProvider, CloudConfiguration cloudConfiguration);
    }
}
#endif
