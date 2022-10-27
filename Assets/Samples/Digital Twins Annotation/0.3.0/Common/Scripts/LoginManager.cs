#if !DT_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.DigitalTwins.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Annotation.Samples
{
    using System.Linq;
#if USE_DT_IDENTITY && USE_DT_STORAGE
    using Identity;

    public class LoginManager : MonoBehaviour
    {
        static class Labels
        {
            internal const string k_NoUser = "Log in to run scene.";
            internal const string k_LoggedInNoUsername = "Signed in - no username";
            internal const string k_LoggingIn = "Logging in...";
            internal const string k_LoggingOut = "Logging out...";
            internal const string k_InvalidSceneFormat = "Scene ID invalid. Please enter a valid GUID.";
            internal const string k_LoginWithPAT = " - Logged in with PAT";
        }

        public event Action<IAccessTokenProvider, Guid, string, CloudConfiguration> LaunchScene;
        public event Action LoggedOut;

        [SerializeField]
        Text m_LoginText;

        [SerializeField]
        List<Button> m_LoginButtons = new ();

        [SerializeField]
        List<Button> m_LogoutButtons = new ();

        [SerializeField]
        Dropdown m_WorkspaceDropdown;

        [SerializeField]
        Dropdown m_SceneDropdown;

        [SerializeField]
        Button m_LaunchSceneButton;

        [SerializeField]
        Text m_SceneText;

        IInteractiveAuthenticator m_Authenticator;
        IUserInfoProvider m_UserInfoProvider;
        CloudConfiguration m_CloudConfiguration;
        string m_UserName;
        List<IWorkspace> m_CurrentWorkspaces;
        List<IScene> m_CurrentScenes;
        Guid m_CurrentSceneId;

        async Task Start()
        {
            RegisterButtons();

            m_CloudConfiguration = PlatformServices.CloudConfiguration;
            m_LoginText.text = Labels.k_NoUser;

            if (m_Authenticator == null)
            {
                m_Authenticator = PlatformServices.Authenticator;
                m_Authenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;

                m_UserInfoProvider = PlatformServices.UserInfoProvider;

                // Update UI with current state
                _ = ApplyAuthenticationState(m_Authenticator.AuthenticationState);
            }

            m_WorkspaceDropdown.onValueChanged.AddListener(OnWorkspaceOptionChanged);
            m_SceneDropdown.onValueChanged.AddListener(OnSceneOptionChanged);
        }

        void OnDestroy()
        {
            UnregisterButtons();

            ClearDropdown(m_WorkspaceDropdown);
            m_WorkspaceDropdown.onValueChanged.RemoveAllListeners();

            ClearDropdown(m_SceneDropdown);
            m_SceneDropdown.onValueChanged.RemoveAllListeners();
        }

        public void Login()
        {
            try
            {
                m_Authenticator.LoginAsync();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    m_LoginText.text = ex.Message;
                }
                throw;
            }
        }

        public void Logout()
        {
            try
            {
                m_Authenticator.LogoutAsync();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    m_LoginText.text = ex.Message;
                }
                throw;
            }
        } 

        void LaunchSceneClicked()
        {
            m_SceneText.text = String.Empty;

            try
            {
                var sceneId = m_CurrentSceneId;
                LaunchScene?.Invoke(m_Authenticator, sceneId, m_UserName, m_CloudConfiguration);
            }
            catch (FormatException e)
            {
                m_SceneText.text = Labels.k_InvalidSceneFormat;
                throw e;
            }
        }

        void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            _ = ApplyAuthenticationState(newAuthenticationState);
        }

        async Task ApplyAuthenticationState(AuthenticationState state)
        {
            // Clear status text on authentication change
            m_SceneText.text = String.Empty;

            switch (state)
            {
                case AuthenticationState.LoggedIn:
                    UpdateButtons(m_LoginButtons, false);
                    UpdateButtons(m_LogoutButtons, m_Authenticator.Interactive);
                    m_UserName = await GetUserInfo();
                    await PopulateWorkspaces();
                    UpdateDropdowns(true);
                    break;
                case AuthenticationState.LoggedOut:
                    LoggedOut?.Invoke();
                    UpdateButtons(m_LoginButtons, m_Authenticator.Interactive);
                    UpdateButtons(m_LogoutButtons, false);
                    UpdateButton(m_LaunchSceneButton, false);
                    m_UserName = string.Empty;
                    m_LoginText.text = Labels.k_NoUser;
                    UpdateDropdowns(false);
                    break;
                case AuthenticationState.AwaitingLogin:
                    UpdateButtons(m_LoginButtons, false);
                    UpdateButtons(m_LogoutButtons, false);
                    m_LoginText.text = Labels.k_LoggingIn;
                    break;
                case AuthenticationState.AwaitingLogout:
                    UpdateButtons(m_LoginButtons, false);
                    UpdateButtons(m_LogoutButtons, false);
                    m_LoginText.text = Labels.k_LoggingOut;
                    break;
            }
        }

        async Task PopulateWorkspaces()
        {
            var workspaces = await PlatformServices.WorkspaceProvider.ListWorkspacesAsync();
            await SetWorkspaces(workspaces.ToList());
        }

        async Task<string> GetUserInfo()
        {
            try
            {
                var userInfo = await m_UserInfoProvider.GetUserInfoAsync();

                var userNameText = userInfo != null ? userInfo.Name : Labels.k_LoggedInNoUsername;
                if (!m_Authenticator.Interactive)
                    userNameText += Labels.k_LoginWithPAT;

                m_LoginText.text = userNameText;

                return userNameText;
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException
                    or UnauthorizedException
                    or ConnectionException
                    or ForbiddenException)
                {
                    m_LoginText.text = ex.Message;
                }
                Debug.LogError(ex.Message);
                throw;
            }
        }

        static void UpdateButton(Button button, bool enabled)
        {
            if (button != null)
                button.interactable = enabled;
        }

        static void UpdateButtons(List<Button> buttons, bool enabled)
        {
            foreach (var button in buttons)
                UpdateButton(button, enabled);
        }

        void UpdateDropdowns(bool enabled)
        {
            if (m_WorkspaceDropdown != null)
                m_WorkspaceDropdown.interactable = enabled;

            if (m_SceneDropdown != null)
                m_SceneDropdown.interactable = enabled;
        }

        void RegisterButtons()
        {
            foreach (var loginButton in m_LoginButtons)
            {
                if (loginButton != null)
                    loginButton.onClick.AddListener(Login);
            }

            foreach (var logoutButton in m_LogoutButtons)
            {
                if (logoutButton != null)
                    logoutButton.onClick.AddListener(Logout);
            }

            if (m_LaunchSceneButton != null)
            {
                m_LaunchSceneButton.onClick.AddListener(LaunchSceneClicked);
                UpdateButton(m_LaunchSceneButton, false);
            }
        }

        void UnregisterButtons()
        {
            foreach (var loginButton in m_LoginButtons)
            {
                if (loginButton != null)
                    loginButton.onClick.RemoveListener(Login);
            }

            foreach (var loginButton in m_LogoutButtons)
            {
                if (loginButton != null)
                    loginButton.onClick.RemoveListener(Logout);
            }

            if (m_LaunchSceneButton != null)
                m_LaunchSceneButton.onClick.RemoveListener(LaunchSceneClicked);
        }

        async Task SetWorkspaces(List<IWorkspace> workspaces)
        {
            ClearDropdown(m_WorkspaceDropdown);
            ClearDropdown(m_SceneDropdown);

            m_CurrentWorkspaces = workspaces;

            var workspaceOptions = new List<Dropdown.OptionData>();
            foreach (var workspace in m_CurrentWorkspaces)
            {
                workspaceOptions.Add(new Dropdown.OptionData(workspace.Name));
            }

            PopulateDropdown(m_WorkspaceDropdown, workspaceOptions);

            if (m_CurrentWorkspaces.Count > 0)
            {
                var selectedWorkspace = m_CurrentWorkspaces[m_WorkspaceDropdown.value];
                var scenes = await selectedWorkspace.ListScenesAsync();
                SetScenes(scenes.ToList());
            }
        }

        void SetScenes(List<IScene> scenes)
        {
            ClearDropdown(m_SceneDropdown);

            m_CurrentScenes = scenes;

            var sceneOptions = new List<Dropdown.OptionData>();
            foreach (var scene in m_CurrentScenes)
            {
                sceneOptions.Add(new Dropdown.OptionData(scene.Name));
            }

            PopulateDropdown(m_SceneDropdown, sceneOptions);
            OnSceneOptionChanged(m_SceneDropdown.value);
        }

        static void PopulateDropdown(Dropdown dropdown, List<Dropdown.OptionData> options)
        {
            dropdown.options = options;
            dropdown.interactable = true;
            dropdown.value = 0;
        }

        static void ClearDropdown(Dropdown dropdown)
        {
            dropdown.ClearOptions();
            dropdown.interactable = false;
            dropdown.value = 0;
        }

       void OnWorkspaceOptionChanged(int index)
       {
            _ = PopulateScenes(index);
       }

        async Task PopulateScenes(int index)
        {
            m_CurrentSceneId = Guid.Empty;
            UpdateButton(m_LaunchSceneButton, false);

            if (m_CurrentWorkspaces.Count > 0)
            {
                var scenes = await m_CurrentWorkspaces[index].ListScenesAsync();
                SetScenes(scenes.ToList());
            }
        }

        void OnSceneOptionChanged(int index)
        {
            if (m_CurrentScenes.Count > 0)
            {
                var scene = m_CurrentScenes[index];

                if(scene != null)
                { 
                    m_CurrentSceneId = scene.Id;
                    UpdateButton(m_LaunchSceneButton, true);
                }
            }
        }
    }
#else
#error Missing dependency to com.digital-twins.identity and/or com.digital-twins.storage.
#endif
}
#endif
