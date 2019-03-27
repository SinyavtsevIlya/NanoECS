using UnityEngine;

public class ContextObserverBehaviour : MonoBehaviour
{
    System.Action OnCreateButtonPress;
    public string ContextName;

    public void Initialize(System.Action onCreateButtonPress, string contextName)
    {
        OnCreateButtonPress = onCreateButtonPress;
        ContextName = contextName;
    }

    public void CreateEntity()
    {
        if (OnCreateButtonPress != null)
        {
            OnCreateButtonPress.Invoke();
        }
    }
}