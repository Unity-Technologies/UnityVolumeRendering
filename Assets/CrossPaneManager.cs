using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossPaneManager : MonoBehaviour
{
    public static CrossPaneManager Instance { get; private set; }
    
    GameObject m_CrossPane;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    
    public void SetCrossPlane(GameObject crossPlane)
    {
        m_CrossPane = crossPlane;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnToggleOnOff()
    {
        m_CrossPane.SetActive(!m_CrossPane.activeSelf);
    }

    public float zoomAmount;

    public void OnZoomIn()
    {
        var pos = m_CrossPane.transform.position;
        pos += m_CrossPane.transform.forward * zoomAmount;
        m_CrossPane.transform.position = pos;
    }

    public void OnZoomOut()
    {
        var pos = m_CrossPane.transform.position;
        pos -= m_CrossPane.transform.forward * zoomAmount;
        m_CrossPane.transform.position = pos;
    }
    
}
