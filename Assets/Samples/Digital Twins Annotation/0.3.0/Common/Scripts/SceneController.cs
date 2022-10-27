#if !DT_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using Unity.DigitalTwins.Common;
using UnityEngine;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public class SceneController : MonoBehaviour
    {
#if USE_DT_IDENTITY
        class SceneData : IScene
        {
            public void Dispose() { }
            public string Name { get; }
            public Guid Id { get; }
            public Guid WorkspaceId { get; }
            public string Url { get; }
            public List<Permission> Permissions { get; }
            public Guid LatestVersion { get; }

            public SceneData(Guid sceneID) => Id = sceneID;
        }

        [SerializeField]
        LoginManager m_LoginManager;

        [SerializeField]
        InformationBar m_InformationBar;

        [SerializeField]
        AnnotationSampleBehaviour m_SampleBehaviour;

        [SerializeField, Tooltip("GameObjects to activate when the user is logged in.")]
        List<GameObject> m_LoggedInUIElements = new();

        [SerializeField, Tooltip("GameObjects to activate when the user is logged out.")]
        List<GameObject> m_LoggedOutUIElements = new();

        void Awake()
        {
            m_LoginManager.LaunchScene += OnLaunchScene;
            m_LoginManager.LoggedOut += OnLoggedOut;

            ActivateElements(false);
        }

        void OnDestroy()
        {
            m_LoginManager.LaunchScene -= OnLaunchScene;
            m_LoginManager.LoggedOut -= OnLoggedOut;
        }

        async void OnLaunchScene(IAccessTokenProvider accessTokenProvider, Guid sceneId, string userName, CloudConfiguration cloudConfiguration)
        {
            ActivateElements(true);

            m_InformationBar.SetProjectId(sceneId);
            m_InformationBar.SetUserName(userName);

            var scene = new SceneData(sceneId);

            await m_SampleBehaviour.InitializeAsync(scene, accessTokenProvider, cloudConfiguration);
        }

        void OnLoggedOut()
        {
            m_InformationBar.SetProjectId(Guid.Empty);
            m_InformationBar.SetUserName(string.Empty);

            ActivateElements(false);
        }

        void ActivateElements(bool loggedIn)
        {
            m_LoggedInUIElements.ForEach(go => go.SetActive(loggedIn));
            m_LoggedOutUIElements.ForEach(go => go.SetActive(!loggedIn));
        }
#else
#error Missing dependency to com.digital-twins.identity package.
#endif
    }
}
#endif
