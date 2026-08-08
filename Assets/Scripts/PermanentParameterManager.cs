using UnityEngine;

public class PermanentParameterManager : MonoBehaviour
{
    public PermanentParameterSO permanentParameters;
    public static PermanentParameterManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
