#if !DT_EXCLUDE_SAMPLES
using System;
using Unity.DigitalTwins.Common;
using UnityEngine;

namespace Unity.DigitalTwins.Storage.Samples
{
    public class WorkspaceButton : ScrollButton<IWorkspace>
    {
        public override void SetItem(IWorkspace workspace, string displayContent, Action<IWorkspace, string> onButtonClicked)
        {
            SetLabel(workspace.Name);
            base.SetItem(workspace, displayContent, onButtonClicked);
        }
    }
}
#endif
