#if !DT_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Text;
using Unity.DigitalTwins.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Storage.Samples
{
    public class ContentScroller : MonoBehaviour
    {
        [SerializeField]
        WorkspaceButton m_WorkspaceButtonPrefab;

        [SerializeField]
        RectTransform m_WorkspaceScrollParent;

        [SerializeField]
        Text m_WorkspaceDisplayText;

        [SerializeField]
        SceneButton m_SceneButtonPrefab;

        [SerializeField]
        RectTransform m_SceneScrollParent;

        [SerializeField]
        Text m_SceneDisplayText;

        List<WorkspaceButton> m_WorkspaceButtons = new ();
        List<SceneButton> m_SceneButtons = new ();

        public event Action<IWorkspace> WorkspaceButtonClicked;

        public void SetWorkspaceContent(IEnumerable<IWorkspace> workspaces)
        {
            Clear();

            foreach (var workspace in workspaces)
            {
                var button = Instantiate(m_WorkspaceButtonPrefab, m_WorkspaceScrollParent);
                button.SetItem(workspace, GetDisplayContent(workspace), OnWorkspaceButtonClicked);

                m_WorkspaceButtons.Add(button);
            }
        }

        public void SetSceneContent(IEnumerable<IScene> scenes)
        {
            ClearSceneSection();

            foreach (var scene in scenes)
            {
                var button = Instantiate(m_SceneButtonPrefab, m_SceneScrollParent);
                button.SetItem(scene, GetDisplayContent(scene), OnSceneButtonClicked);

                m_SceneButtons.Add(button);
            }
        }

        public void Clear()
        {
            ClearWorkspaceSection();
            ClearSceneSection();
        }

        void ClearWorkspaceSection()
        {
            if (m_WorkspaceDisplayText != null)
                m_WorkspaceDisplayText.text = string.Empty;

            foreach (var button in m_WorkspaceButtons)
            {
                if (button != null)
                    Destroy(button.gameObject);
            }

            m_WorkspaceButtons.Clear();
        }

        void ClearSceneSection()
        {
            if (m_WorkspaceDisplayText != null)
                m_SceneDisplayText.text = string.Empty;

            foreach (var button in m_SceneButtons)
            {
                if (button != null)
                    Destroy(button.gameObject);
            }

            m_SceneButtons.Clear();
        }

        void OnWorkspaceButtonClicked(IWorkspace workSpace, string displayContent)
        {
            m_WorkspaceDisplayText.text = displayContent;

            WorkspaceButtonClicked?.Invoke(workSpace);
        }

        void OnSceneButtonClicked(IScene scene, string displayContent)
        {
            m_SceneDisplayText.text = displayContent;
        }

        static string GetDisplayContent(IWorkspace workspace)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Name: {workspace.Name}");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"Id: {workspace.Id}");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"Ord Id: {workspace.OrgId}");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"Url: {workspace.Url}");
            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }

        static string GetDisplayContent(IScene scene)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Name: {scene.Name}");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"Id: {scene.Id}");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"Workspace Id: {scene.WorkspaceId}");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"Url: {scene.Url}");
            stringBuilder.AppendLine();

            if (scene.Permissions != null)
            {
                stringBuilder.AppendLine("Permissions:");
                foreach (var permission in scene.Permissions)
                    stringBuilder.AppendLine($"\t{permission}");
            }

            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }
    }
}
#endif
