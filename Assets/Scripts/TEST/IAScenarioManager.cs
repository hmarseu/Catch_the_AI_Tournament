using UnityEngine;
using YokaiNoMori.General;

public class IAScenarioManager : MonoBehaviour
{
    private static IAScenarioManager m_instance = null;
    public static IAScenarioManager Instance => m_instance;

    public int ScenarioNumber;

    public void Awake()
    {
        #region Singleton
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            m_instance = this;
        }
        DontDestroyOnLoad(gameObject);
        #endregion
    }

}
