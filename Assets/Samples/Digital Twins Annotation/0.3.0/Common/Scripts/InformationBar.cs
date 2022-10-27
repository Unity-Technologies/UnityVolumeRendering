#if !DT_EXCLUDE_SAMPLES
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public class InformationBar : MonoBehaviour
    {
        [SerializeField]
        Text m_ProjectIdText;

        [SerializeField]
        Text m_UserNameText;

        public void SetProjectId(Guid value)
        {
            m_ProjectIdText.text = value.ToString();
        }

        public void SetUserName(string value)
        {
            m_UserNameText.text = value;
        }
    }
}
#endif
