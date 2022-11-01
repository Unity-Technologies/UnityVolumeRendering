using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.DigitalTwins.Annotation;
using Unity.DigitalTwins.Annotation.Samples;
using Unity.DigitalTwins.Common;
using Unity.DigitalTwins.Common.Runtime;
using Unity.DigitalTwins.DataStreaming.Runtime;
using Unity.DigitalTwins.Identity;
using Unity.DigitalTwins.Identity.Runtime;
using Unity.DigitalTwins.Presence.Runtime.Netcode;
using Unity.DigitalTwins.Storage;
using UnityEngine;
using UnityVolumeRendering;

public class PointCloudGrabber : MonoBehaviour
{
    public string SceneName = "My First Scene";
    
    CloudConfiguration m_CloudConfiguration;
    UnityHttpClient m_HttpClient;
    CompositeAuthenticator m_Authenticator;
    ServiceHttpClient m_Service;
    AnnotationRepository m_AnnotationRepository;

    string baseUrl = "https://dt.unity.com/api/datasets/";
    string workspaceId = "abe3d1a9-160d-4950-9122-63b0253408ab";
    string orgId = "2474827961124";
    string datasetId = "95c4cf38-7a18-4034-91f9-51b169f80b05";
    string versionId = "7b92b8a9-bd2d-4592-9900-a464e1e142dd";
    string artifactName = "VisMale.raw";
    
    // Start is called before the first frame update
    async Task Start()
    {
        m_CloudConfiguration = UnityCloudConfigurationFactory.Create();
        m_HttpClient = new UnityHttpClient();

        m_Authenticator = new CompositeAuthenticator(m_HttpClient, DigitalTwinsPlayerSettings.Instance, DigitalTwinsPlayerSettings.Instance);
        await m_Authenticator.InitializeAsync();
        await m_Authenticator.LogoutAsync(); // if (m_Authenticator.AuthenticationState == AuthenticationState.LoggedOut)

        //     await m_Authenticator.LoginAsync();
        if (m_Authenticator.AuthenticationState == AuthenticationState.LoggedOut)
            await m_Authenticator.LoginAsync();

        // We will get our data from this service.
        m_Service = new ServiceHttpClient(m_HttpClient, m_Authenticator, DigitalTwinsPlayerSettings.Instance);

        // Retrieving our uploaded scene through the available Scenes.
        var sceneProvider = new SceneProvider(m_Service, m_CloudConfiguration);

        var workspaceProvider = new WorkspaceProvider(m_Service, m_CloudConfiguration);

        var workspace = await workspaceProvider.GetWorkspaceAsync(workspaceId);

        var nugget = await OurSweetMethod();
        Debug.Log($"nugget: {nugget}");

        var importer = new DtRawDatasetImporter(nugget);

        var dataset = importer.Import();

        if (dataset != null)
        {
            var obj = VolumeObjectFactory.CreateObject(dataset);

            // Create cross section pane
            var plane = VolumeObjectFactory.SpawnOrientedCrossSectionPlane(obj);
            CrossPaneManager.Instance.SetCrossPlane(plane);
            plane.SetActive(false);
        }
        else
        {
            Debug.LogError("Failed to import datset");
        }

        var scenes = await workspace.ListScenesAsync();
        var scene = scenes.First();

        m_AnnotationRepository = new AnnotationRepository(scene, m_Service, m_CloudConfiguration);

        try
        {
            var topics = await m_AnnotationRepository.GetTopicsAsync();

            var t = topics.First();

            // Fetches an existing topic using that existing topic's ID
            var fetchedTopic = await m_AnnotationRepository.GetTopicAsync(t.Id);

            // Outputs the fetched topic information
            Debug.Log($"Feched Topic ID: {fetchedTopic.Id}, Created Topic Title: {fetchedTopic.Title}");


        }
        catch (ServiceException e)
        {
            Debug.LogError(e);
            throw;
        }
    }


    async Task<byte[]> OurSweetMethod()
    {
        try
        {
            var response = await m_Service.GetAsync($"{baseUrl}{datasetId}/versions/{versionId}/artifacts/{artifactName}");
            var byteArray = await response.Content.ReadAsByteArrayAsync();
            return byteArray;
        }
        catch (Exception ex)
        {
            Debug.Log($"Error retrieving scene: '{ex.Message}'");
        }

        return null;
    }
    
    void OnDestroy()
    {
        m_Authenticator?.Dispose();
    }
}
