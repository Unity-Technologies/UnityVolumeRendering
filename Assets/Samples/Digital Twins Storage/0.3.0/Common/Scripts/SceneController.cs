#if !DT_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.DigitalTwins.Storage.Samples
{
#if USE_DT_IDENTITY
    using Identity;

    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        LoginManager m_LoginManager;

        [SerializeField, Tooltip("GameObjects to activate when the user is logged in.")]
        List<GameObject> m_LoggedInUIElements = new();

        [SerializeField, Tooltip("GameObjects to activate when the user is logged out.")]
        List<GameObject> m_LoggedOutUIElements = new();

        void Awake()
        {
            m_LoginManager.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        void Start()
        {
            ActivateElements(m_LoginManager.AuthenticationState == AuthenticationState.LoggedIn);
        }

        void OnDestroy()
        {
            m_LoginManager.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        void OnAuthenticationStateChanged(AuthenticationState state)
        {
            ActivateElements(state == AuthenticationState.LoggedIn);
        }

        void ActivateElements(bool loggedIn)
        {
            m_LoggedInUIElements.ForEach(go => go.SetActive(loggedIn));
            m_LoggedOutUIElements.ForEach(go => go.SetActive(!loggedIn));
        }
    }
#else
#error Missing dependency to com.digital-twins.identity package.
#endif
}
#endif
