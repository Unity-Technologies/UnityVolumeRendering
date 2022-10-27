#if !DT_EXCLUDE_SAMPLES
using System.Threading.Tasks;
using Unity.DigitalTwins.Common;
using Unity.DigitalTwins.Common.Runtime;
using Unity.DigitalTwins.Identity;
using Unity.DigitalTwins.Identity.Runtime;
using Unity.DigitalTwins.Storage;

namespace Unity.DigitalTwins.Annotation.Samples
{
    /// <summary>
    /// A class to initialize and provide services and dependencies for the Digital Twins platform.
    /// </summary>
    public static class PlatformServices
    {
        static UnityHttpClient s_HttpClient;
        static CompositeAuthenticator s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="IServiceHttpClient"/>.
        /// </summary>
        public static IServiceHttpClient ServiceHttpClient { get; private set; }

        /// <summary>
        /// Returns a <see cref="IInteractiveAuthenticator"/>.
        /// </summary>
        public static IInteractiveAuthenticator Authenticator => s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="IUserInfoProvider"/>.
        /// </summary>
        public static IUserInfoProvider UserInfoProvider { get; private set; }

        /// <summary>
        /// REturns a <see cref="CloudConfiguration"/>
        /// </summary>
        public static CloudConfiguration CloudConfiguration { get; private set; }

        /// <summary>
        /// Returns a <see cref="IWorkspaceProvider"/>.
        /// </summary>
        public static IWorkspaceProvider WorkspaceProvider { get; private set; }

        public static async Task InitializeAsync()
        {
            CloudConfiguration = UnityCloudConfigurationFactory.Create();

            s_HttpClient = new UnityHttpClient();

            s_CompositeAuthenticator = new CompositeAuthenticator(s_HttpClient, DigitalTwinsPlayerSettings.Instance, DigitalTwinsPlayerSettings.Instance);

            ServiceHttpClient = new ServiceHttpClient(s_HttpClient, Authenticator, DigitalTwinsPlayerSettings.Instance);

            UserInfoProvider = new UserInfoProvider(ServiceHttpClient, CloudConfiguration);

            WorkspaceProvider = new WorkspaceProvider(ServiceHttpClient, CloudConfiguration);

            await Authenticator.InitializeAsync();
        }

        public static void Shutdown()
        {
            UserInfoProvider = null;

            s_CompositeAuthenticator.Dispose();
            s_CompositeAuthenticator = null;

            ServiceHttpClient = null;

            s_HttpClient = null;

            WorkspaceProvider = null;
        }
    }
}
#endif
