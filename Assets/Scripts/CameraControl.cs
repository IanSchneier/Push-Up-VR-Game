using UnityEngine;
using UnityEngine.XR;
public class CameraControl : MonoBehaviour
{
    
    void Start()
    {
        XRDevice.DisableAutoXRCameraTracking(gameObject.GetComponent<Camera>(), true);
        gameObject.GetComponent<Camera>().transform.rotation = Quaternion.Euler(0, 0, 0);
    }

}