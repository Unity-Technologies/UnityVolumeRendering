using System.Collections;
using System.Collections.Generic;
using Unity.DigitalTwins.Annotation;
using Unity.DigitalTwins.Common;
using UnityEngine;

public class TopicManager : MonoBehaviour
{
    public static TopicManager Instance { get; private set; }

    AnnotationRepository m_AnnotationRepository;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void Setup(IScene scene, IServiceHttpClient service, CloudConfiguration cloudConfiguration)
    {
        m_AnnotationRepository = new AnnotationRepository(scene, service, cloudConfiguration);
    }
}
