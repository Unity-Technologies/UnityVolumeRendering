#if !DT_EXCLUDE_SAMPLES
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public class FlyoutDialog : MonoBehaviour
    {
        const string k_ResolveText = "Resolve";
        const string k_UnresolveText = "Unresolve";

        public event Action<string> Edit;
        public event Action Delete;
        public event Action ToggleResolution;

        [SerializeField]
        Button m_EditButton;

        [SerializeField]
        Button m_DeleteButton;

        [SerializeField]
        Button m_ResolveButton;

        [SerializeField]
        Text m_ResolutionStateText;

        [SerializeField]
        InputDialog m_InputDialog;

        void Awake()
        {
            m_EditButton.onClick.AddListener(CloseDialog);
            m_DeleteButton.onClick.AddListener(CloseDialog);
            m_ResolveButton.onClick.AddListener(CloseDialog);

            m_EditButton.onClick.AddListener(EditPrompt);
            m_DeleteButton.onClick.AddListener(OnDelete);
            m_ResolveButton.onClick.AddListener(OnToggleResolution);
        }

        void OnDestroy()
        {
            m_EditButton.onClick.RemoveAllListeners();
            m_DeleteButton.onClick.RemoveAllListeners();
            m_ResolveButton.onClick.RemoveAllListeners();
        }

        public void OpenDialog(bool isResolved = false)
        {
            m_ResolutionStateText.text = isResolved ? k_UnresolveText : k_ResolveText;

            gameObject.SetActive(true);
        }

        public void CloseDialog()
        {
            gameObject.SetActive(false);
        }

        void EditPrompt()
        {
            m_InputDialog.OpenDialog(newText => OnEdit(newText), "Enter new text..");
        }

        void OnEdit(string newText)
        {
            if (Edit != null)
                Edit.Invoke(newText);
        }

        void OnDelete()
        {
            if (Delete != null)
                Delete.Invoke();
        }

        void OnToggleResolution()
        {
            if (ToggleResolution != null)
                ToggleResolution.Invoke();
        }
    }
}
#endif
