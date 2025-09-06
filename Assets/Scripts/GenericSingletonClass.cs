using UnityEngine;

public class GenericSingletonClass<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            //if (instance == null)
            //{
            //    instance = FindObjectOfType<T>();
            //    Debug.LogWarning("Searching instance with Find!");

            //    //    if (instance == null) 
            //    //    {
            //    //      GameObject obj = new GameObject ();
            //    //      obj.name = typeof(T).Name;
            //    //      instance = obj.AddComponent<T>();
            //    //    }
            //}
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        if (instance == null)
        {
            instance = this as T;
            // DontDestroyOnLoad (this.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}