#if !DT_EXCLUDE_SAMPLES
using System;
using System.Threading.Tasks;
using Unity.DigitalTwins.Common;
using Unity.DigitalTwins.Common.Runtime;
using UnityEngine;

namespace Unity.DigitalTwins.Storage.Samples
{
#if USE_DT_IDENTITY
    using Identity;
    using Identity.Runtime;

    /// <summary>
    /// A class to initialize and provide services and dependencies for the Digital Twins platform.
    /// </summary>
    public static class PlatformServices
    {
        static UnityHttpClient s_HttpClient;
        static ServiceHttpClient s_ServiceHttpClient;
        static CompositeAuthenticator s_CompositeAuthenticator;

        /// <summary>
        /// Returns an <see cref="IInteractiveAuthenticator"/>.
        /// </summary>
        public static IInteractiveAuthenticator Authenticator => s_CompositeAuthenticator;

        /// <summary>
        /// Returns an <see cref="IWorkspaceProvider"/>.
        /// </summary>
        public static IWorkspaceProvider WorkspaceProvider { get; private set; }

        /// <summary>
        /// Returns an <see cref="IWorkspaceProvider"/>.
        /// </summary>
        public static IUserInfoProvider UserInfoProvider { get; private set; }

        public static async Task InitializeAsync()
        {
            var cloudConfiguration = UnityCloudConfigurationFactory.Create();
            s_HttpClient = new UnityHttpClient();
            s_CompositeAuthenticator = new CompositeAuthenticator(s_HttpClient, DigitalTwinsPlayerSettings.Instance, DigitalTwinsPlayerSettings.Instance);

            s_ServiceHttpClient = new ServiceHttpClient(s_HttpClient, Authenticator, DigitalTwinsPlayerSettings.Instance);

            WorkspaceProvider = new WorkspaceProvider(s_ServiceHttpClient, cloudConfiguration);

            UserInfoProvider = new UserInfoProvider(s_ServiceHttpClient, cloudConfiguration);

            await Authenticator.InitializeAsync();
        }

        public static void Shutdown()
        {
            UserInfoProvider = null;

            WorkspaceProvider = null;

            s_ServiceHttpClient = null;

            s_CompositeAuthenticator.Dispose();
            s_CompositeAuthenticator = null;

            s_HttpClient = null;
        }
    }
#else
#error Missing dependency to com.digital-twins.identity package.
#endif
}
#endif
