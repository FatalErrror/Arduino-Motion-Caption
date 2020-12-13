using UnityEngine;

public class ControlableCharacter : MonoBehaviour
{
    public Skeleton CharactersSkeleton;
    public VirtualSkeleton FromVirtualSkeleton;
   
    private GameObject _container;

    
    // Start is called before the first frame update
    void Start()
    {
        if (!FromVirtualSkeleton) throw new UnityException("The field 'FromSkeleton' is null, when it can't be null");
        if (!CharactersSkeleton) throw new UnityException("The field 'CharactersSkeleton' is null, when it can't be null");

        _container = FromVirtualSkeleton.Container;

        Transform[] a = CharactersSkeleton.GetBonesArray();
        Transform[] b = FromVirtualSkeleton.Skeleton.GetBonesArray();
        for (int i = 0;i < b.Length;i++)
        {
            Transform item = a[i];
            Transform container = GameObject.Instantiate(_container,item).transform;
            container.parent = item.parent;
            container.rotation = b[i].rotation;
            Quaternion q = item.rotation;
            item.parent = container;
            item.rotation = q;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform[] a = CharactersSkeleton.GetBonesArray();
        Transform[] b = FromVirtualSkeleton.Skeleton.GetBonesArray();
        for (int i = 0; i < b.Length; i++)
        {
            Quaternion q = b[i].rotation;
            a[i].parent.rotation = q;
        }
        
    }
}
