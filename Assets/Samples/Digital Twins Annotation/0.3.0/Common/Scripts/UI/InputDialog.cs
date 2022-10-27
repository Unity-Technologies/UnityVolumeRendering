#if !DT_EXCLUDE_SAMPLES
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public class InputDialog : MonoBehaviour
    {
        const string k_DefaultPlaceholderLabel = "Input text..";

        [SerializeField]
        InputField m_TextInput;

        [SerializeField]
        Text m_PlaceholderLabel;

        [SerializeField]
        Button m_ConfirmButton;

        [SerializeField]
        Button m_CancelButton;

        event Action<string> m_SubmitTextInput;

        public void OpenDialog(Action<string> onTextInput, string placeholderLabel = null)
        {
            if (gameObject.activeSelf)
                ClearDialog();

            m_SubmitTextInput += onTextInput;

            m_TextInput.text = string.Empty;
            m_TextInput.onValueChanged.AddListener(OnInputChanged);
            m_TextInput.onSubmit.AddListener(OnSubmit);

            m_PlaceholderLabel.text = !string.IsNullOrEmpty(placeholderLabel) ? placeholderLabel : k_DefaultPlaceholderLabel;

            m_ConfirmButton.enabled = false;
            m_ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);

            m_CancelButton.onClick.AddListener(OnCancelButtonClicked);

            gameObject.SetActive(true);
        }

        public void ClearDialog()
        {
            m_SubmitTextInput = null;

            m_TextInput.text = string.Empty;

            m_PlaceholderLabel.text = k_DefaultPlaceholderLabel;

            m_TextInput.onValueChanged.RemoveListener(OnInputChanged);
            m_TextInput.onValueChanged.RemoveListener(OnSubmit);
            m_ConfirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            m_CancelButton.onClick.RemoveListener(OnCancelButtonClicked);

            gameObject.SetActive(false);
        }

        void OnInputChanged(string input)
        {
            m_ConfirmButton.enabled = !string.IsNullOrEmpty(input);
        }

        void OnSubmit(string input)
        {
            m_SubmitTextInput?.Invoke(input);

            ClearDialog();
        }

        void OnConfirmButtonClicked()
        {
            m_SubmitTextInput?.Invoke(m_TextInput.text);

            ClearDialog();
        }

        void OnCancelButtonClicked()
        {
            ClearDialog();
        }
    }
}
#endif
