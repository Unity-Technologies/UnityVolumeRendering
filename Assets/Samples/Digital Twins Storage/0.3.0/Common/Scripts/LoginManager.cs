#if !DT_EXCLUDE_SAMPLES
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Storage.Samples
{
#if USE_DT_IDENTITY
    using Identity;

    public class LoginManager : MonoBehaviour
    {
        static class Labels
        {
            internal const string k_Login = "Login";
            internal const string k_Logout = "Logout";
            internal const string k_AwaitingLogin = "Logging in...";
            internal const string k_AwaitingLogout = "Logging out...";
            internal const string k_NoUser = "<no user name>";
            internal const string k_LoginWithPAT = " - Logged in with PAT";
        }

        [SerializeField]
        Text m_UserNameText;

        [SerializeField]
        Text m_StatusText;

        [SerializeField]
        Button m_Button;

        IInteractiveAuthenticator m_Authenticator;
        IUserInfoProvider m_UserInfoProvider;

        public AuthenticationState AuthenticationState => m_Authenticator?.AuthenticationState ?? AuthenticationState.LoggedOut;
        public event Action<AuthenticationState> AuthenticationStateChanged;

        void Awake()
        {
            m_Button.onClick.AddListener(OnButtonClick);

            m_Authenticator = PlatformServices.Authenticator;
            m_Authenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;

            m_UserInfoProvider = PlatformServices.UserInfoProvider;

            // Update UI with current state
            ApplyAuthenticationState(m_Authenticator.AuthenticationState);
        }

        void OnDestroy()
        {
            m_Authenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;

            m_Button.onClick.RemoveListener(OnButtonClick);
        }

        void OnButtonClick()
        {
            HandleButtonClick();
        }

        async Task HandleButtonClick()
        {
            if (m_Authenticator != null)
            {
                switch (m_Authenticator?.AuthenticationState)
                {
                    case AuthenticationState.LoggedOut:
                        await m_Authenticator.LoginAsync();
                        break;
                    case AuthenticationState.LoggedIn:
                        await m_Authenticator.LogoutAsync();
                        break;
                }
            }
        }

        void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            AuthenticationStateChanged?.Invoke(newAuthenticationState);

            ApplyAuthenticationState(newAuthenticationState);
        }

        async Task ApplyAuthenticationState(AuthenticationState state)
        {
            switch (state)
            {
                case AuthenticationState.LoggedOut:
                    m_UserNameText.text = string.Empty;
                    m_StatusText.text = string.Empty;
                    UpdateButton(m_Button, m_Authenticator.Interactive, Labels.k_Login);
                    break;
                case AuthenticationState.LoggedIn:
                    await SetUserInfo();
                    m_StatusText.text = string.Empty;
                    UpdateButton(m_Button, m_Authenticator.Interactive, Labels.k_Logout);
                    break;
                case AuthenticationState.AwaitingLogin:
                    m_StatusText.text = Labels.k_AwaitingLogin;
                    UpdateButton(m_Button, false, Labels.k_AwaitingLogin);
                    break;
                case AuthenticationState.AwaitingLogout:
                    m_StatusText.text = Labels.k_AwaitingLogout;
                    UpdateButton(m_Button, false, Labels.k_AwaitingLogout);
                    break;
            }
        }

        static void UpdateButton(Button button, bool enabled, string label = null)
        {
            if (button != null)
            {
                button.interactable = enabled;

                if (label != null)
                {
                    var buttonLabel = button.GetComponentInChildren<Text>();
                    buttonLabel.text = label;
                }
            }
        }

        async Task SetUserInfo()
        {
            var userInfo = await m_UserInfoProvider.GetUserInfoAsync();

            var userNameText = userInfo != null ? userInfo.Name : Labels.k_NoUser;
            m_UserNameText.text = userNameText;
            if (!m_Authenticator.Interactive)
                m_UserNameText.text += Labels.k_LoginWithPAT;
        }
    }
#else
#error Missing dependency to com.digital-twins.identity package.
#endif
}
#endif
