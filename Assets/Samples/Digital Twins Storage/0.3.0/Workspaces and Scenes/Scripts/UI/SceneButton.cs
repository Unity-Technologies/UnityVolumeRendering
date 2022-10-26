#if !DT_EXCLUDE_SAMPLES
using System;
using Unity.DigitalTwins.Common;
using UnityEngine;

namespace Unity.DigitalTwins.Storage.Samples
{
    public class SceneButton : ScrollButton<IScene>
    {
        public override void SetItem(IScene scene, string displayContent, Action<IScene, string> onButtonClicked)
        {
            SetLabel(scene.Name);
            base.SetItem(scene, displayContent, onButtonClicked);
        }
    }
}
#endif
