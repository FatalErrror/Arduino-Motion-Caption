using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(AutoCalibrateArm)), CanEditMultipleObjects]
public class AutoCalibrateArmEditor : Editor
{
    ArcHandle m_ArcHandleMin = new ArcHandle();
    ArcHandle m_ArcHandleMax = new ArcHandle();

    protected virtual void OnEnable()
    {
        // arc handle has no radius handle by default
        m_ArcHandleMin.SetColorWithRadiusHandle(Color.green, 0.1f);
        m_ArcHandleMax.SetColorWithRadiusHandle(Color.green, 0.1f);

    }

    // the OnSceneGUI callback uses the Scene view camera for drawing handles by default
    protected virtual void OnSceneGUI()
    {
        if (!Application.isPlaying)
        {
            AutoCalibrateArm projectileExample = (AutoCalibrateArm)target;

            // copy the target object's data to the handle
            m_ArcHandleMin.angle = projectileExample.MinAngle;
            m_ArcHandleMax.angle = projectileExample.MaxAngle;

            m_ArcHandleMin.radius = projectileExample.radius;
            m_ArcHandleMax.radius = projectileExample.radius;

            // set the handle matrix so that angle extends upward from target's facing direction along ground
            Vector3 handleDirection = projectileExample.facingDirection;
            Vector3 handleNormal = Vector3.Cross(handleDirection, Vector3.right);
            Matrix4x4 handleMatrix = Matrix4x4.TRS(
                projectileExample.transform.position,
                Quaternion.LookRotation(handleDirection, handleNormal),
                Vector3.one
            );

            using (new Handles.DrawingScope(handleMatrix))
            {
                // draw the handle
                EditorGUI.BeginChangeCheck();
                m_ArcHandleMin.DrawHandle();
                m_ArcHandleMax.DrawHandle();

                if (EditorGUI.EndChangeCheck())
                {
                    // record the target object before setting new values so changes can be undone/redone
                    Undo.RecordObject(projectileExample, "Change Projectile Properties");

                    // copy the handle's updated data back to the target object
                    projectileExample.MinAngle = m_ArcHandleMin.angle;
                    projectileExample.MaxAngle = m_ArcHandleMax.angle;

                    //projectileExample.radius = m_ArcHandle.radius;
                }
            }
        }
    }
}
