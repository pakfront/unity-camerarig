using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class CameraRig : MonoBehaviour
{
    [Range(1,40)]
    public float focusDistance = 2;
    [Range(-60,60)] public float pan, tilt, roll;
    [Header("Dolly")]
    public float dolly;
    public float track1;
    public float track2;

    [Header("Crane")]
    [Range(0,90)] public float craneAzimuth = 30f;
    [Range(0,3)]public float craneLength = .25f;
    [Range(0,3)]public float craneHeight = 1.4f;

    [Header("Tracks")]
    public Vector3 track1Direction = Vector3.right;
    public Vector3 track2Direction = Vector3.right;

    [Header("References")]
    [SerializeField]
    Transform dollyTransform;
    [SerializeField]
    Transform cranePivotTransform, craneHeadTransform, cameraHeadTransform;

    [SerializeField]
    new Camera camera;
    [SerializeField]
    PostProcessVolume postProcessVolume;
    Transform cameraTransform;

    void OnEnable()
    {
        enabled = TryAssign<Camera>("Main Camera", ref camera);
        enabled = TryAssign<Transform>("Dolly", ref dollyTransform);
        enabled = TryAssign<Transform>("Dolly/CranePivot", ref cranePivotTransform);
        enabled = TryAssign<Transform>("Dolly/CranePivot/CraneHead", ref craneHeadTransform);
        enabled = TryAssign<Transform>("Dolly/CranePivot/CraneHead/CameraHead", ref cameraHeadTransform);
        if (! enabled)
        {
            Debug.LogError("Unable to Initialize rig",this);
            return;
        }
        OnValidate();
    }

    void LateUpdate()
    {
        Apply();
    }

    void OnValidate()
    {
        cameraTransform = camera.transform;
        track1Direction = track1Direction.normalized;
        track2Direction = track2Direction.normalized;
        if (postProcessVolume == null) return;

    }

    public void Apply()
    {
        dollyTransform.localPosition = 
        Vector3.forward * dolly 
        + track1Direction * track1
        + track2Direction * track2; 

        cranePivotTransform.localPosition = Vector3.up * craneHeight;
        cranePivotTransform.localRotation = Quaternion.Euler( -1 * craneAzimuth, 0, 0);
        craneHeadTransform.localPosition = Vector3.forward * craneLength;
        craneHeadTransform.localRotation = Quaternion.Euler(tilt, pan, roll);
        
        cameraTransform.position = cameraHeadTransform.position;

        if (postProcessVolume == null) return;
        var depthOfField = postProcessVolume.profile.GetSetting<DepthOfField>();

        if (depthOfField != null && depthOfField.enabled)
        {
            depthOfField.focusDistance.Override(focusDistance);
        } else {
            Debug.LogError("Depth of field is null");
        }
    }

    bool TryAssign<T>(string find, ref T obj) where T : Component
    {
        if (obj != null) return true;
        Transform child = transform.Find(find);
        if (child == null) {
            Debug.LogError(name+" Unable to Find child "+find,this);
            return false;
        }
        obj = child.GetComponent<T>();
        if (obj != null)
        {
            return true;
        }
        Debug.LogError(name+" unable to GetComponent "+typeof(T)+" on child",child);
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(transform.position, dollyTransform.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(dollyTransform.position, cranePivotTransform.position);
        Gizmos.DrawLine(cranePivotTransform.position, craneHeadTransform.position);
        Gizmos.DrawLine(craneHeadTransform.position, cameraHeadTransform.position);
    }

}
