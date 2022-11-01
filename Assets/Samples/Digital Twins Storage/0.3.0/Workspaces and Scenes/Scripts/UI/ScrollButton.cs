#if !DT_EXCLUDE_SAMPLES
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Storage.Samples
{
    public abstract class ScrollButton<TItem> : MonoBehaviour
    {
        [SerializeField]
        Button m_Button;

        [SerializeField]
        Text m_ButtonLabel;

        TItem m_Item;
        string m_DisplayContent;
        Action<TItem, string> m_ButtonClicked;

        public TItem Item => m_Item;

        void Awake()
        {
            m_Button.onClick.AddListener(OnButtonClicked);
        }

        void OnDestroy()
        {
            m_Button.onClick.RemoveListener(OnButtonClicked);
            m_ButtonClicked = null;
        }

        public virtual void SetItem(TItem item, string displayContent, Action<TItem, string> onButtonClicked)
        {
            m_Item = item;
            m_DisplayContent = displayContent;
            m_ButtonClicked = onButtonClicked;
        }

        protected void SetLabel(string label)
        {
            m_ButtonLabel.text = label;
        }

        void OnButtonClicked()
        {
            m_ButtonClicked?.Invoke(m_Item, m_DisplayContent);
        }
    }
}
#endif
