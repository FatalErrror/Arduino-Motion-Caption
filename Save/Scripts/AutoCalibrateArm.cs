using UnityEngine;

public class AutoCalibrateArm : MonoBehaviour
{
    public VirtualSkeleton RootVirtualSkeleton;
    public Skeleton.Indexs offsetIndex;
    public bool use = false, test;

    public Vector3 localRot;

    public Transform Arm;
    public Transform Elbow;

    [SerializeField]
    private Transform _arm;

    public float NonXThrashold = 10;
    public float XThrashold = 5;

    public float angle, angle1;






    [SerializeField]
    public float MinAngle = -45;

    [SerializeField]
    public float MaxAngle = 45;

    [SerializeField]
    public float radius = 0.1f;

    public Vector3 facingDirection
    {
        get
        {
            //if (RootVirtualSkeleton != null) try { Arm = RootVirtualSkeleton.Skeleton.GetBonesArrayWithAwake()[(int)offsetIndex]; } catch { }
            Vector3 result = Arm ? Arm.forward : transform.forward;
            result.y = 0f;
            return result.sqrMagnitude == 0f ? Vector3.forward : result.normalized;
        }
    }

    private void Start()
    {
        _arm = GameObject.Instantiate(_arm.gameObject, Arm).transform;
    }


    private void Update()
    {
        localRot = Elbow.rotation.eulerAngles;
        angle = Quaternion.Angle(Elbow.rotation, Arm.rotation);
        angle1 = Quaternion.Angle(Elbow.rotation, _arm.rotation);
        if (angle1 > 90) angle = -angle;

        test = angle > MaxAngle + XThrashold || angle < MinAngle - XThrashold;

        if (use && test)
        {
            if (
                IsInThrashold(Elbow.rotation.eulerAngles.x, NonXThrashold) && 
                IsInThrashold(Elbow.rotation.eulerAngles.z - 90, NonXThrashold) && 
                IsInThrashold(Arm.rotation.eulerAngles.x, NonXThrashold) && 
                IsInThrashold(Arm.rotation.eulerAngles.z - 90, NonXThrashold)
                )
            {
                if (angle > MaxAngle) RootVirtualSkeleton.AddOffset((int)offsetIndex, angle - MaxAngle);
                if (angle < MinAngle) RootVirtualSkeleton.AddOffset((int)offsetIndex, angle - MinAngle);
            }
        }
    }

    private bool IsInThrashold(float value, float thrashold)
    {
        return (value < thrashold || value > 360 - thrashold);
    }



}
