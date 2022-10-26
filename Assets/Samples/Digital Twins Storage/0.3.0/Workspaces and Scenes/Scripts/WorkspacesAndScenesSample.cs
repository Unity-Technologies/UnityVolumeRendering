#if !DT_EXCLUDE_SAMPLES
using System;
using System.Threading.Tasks;
using Unity.DigitalTwins.Common;
using UnityEngine;

namespace Unity.DigitalTwins.Storage.Samples.WorkspacesAndScenes
{
#if USE_DT_IDENTITY
    using Identity;

    public class WorkspacesAndScenesSample : MonoBehaviour
    {
        [SerializeField]
        ContentScroller m_ContentScroller;

        IAuthenticator m_Authenticator;
        IWorkspaceProvider m_WorkspaceProvider;

        async Task Awake()
        {
            // Fetch the services
            m_Authenticator = PlatformServices.Authenticator;
            m_Authenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;

            m_WorkspaceProvider = PlatformServices.WorkspaceProvider;

            // Register to workspace-clicked action
            m_ContentScroller.WorkspaceButtonClicked += PopulateScenes;
        }

        void OnEnable()
        {
            ApplyAuthenticationState(m_Authenticator.AuthenticationState);
        }

        void OnDisable()
        {
            m_ContentScroller.Clear();
        }

        void OnDestroy()
        {
            m_ContentScroller.WorkspaceButtonClicked -= PopulateScenes;
        }

        void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            ApplyAuthenticationState(newAuthenticationState);
        }

        async Task ApplyAuthenticationState(AuthenticationState state)
        {
            switch (state)
            {
                case AuthenticationState.LoggedOut:
                    m_ContentScroller.Clear();
                    break;
                case AuthenticationState.LoggedIn:
                    await PopulateWorkspaces();
                    break;
            }
        }

        async Task PopulateWorkspaces()
        {
            var workspaces = await m_WorkspaceProvider.ListWorkspacesAsync();
            m_ContentScroller.SetWorkspaceContent(workspaces);
        }

        async void PopulateScenes(IWorkspace workspace)
        {
            var scenes = await workspace.ListScenesAsync();
            m_ContentScroller.SetSceneContent(scenes);
        }
    }
#else
#error Missing dependecy to com.digital-twins.identity package.
#endif
}
#endif
