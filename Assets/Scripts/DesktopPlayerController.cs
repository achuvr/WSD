using Fusion;
using UnityEngine;

public class DesktopPlayerController : NetworkBehaviour
{
    [SerializeField] private VRPlayerController _targetVRPlayer;
    [SerializeField] private Camera _desktopCamera;

    public override void Spawned()
    {
        // リモートインスタンス（VR側など）ではカメラ・AudioListenerを無効化
        if (!HasStateAuthority)
        {
            foreach (var cam in GetComponentsInChildren<Camera>(true))
                cam.enabled = false;
            foreach (var listener in GetComponentsInChildren<AudioListener>(true))
                listener.enabled = false;
        }
    }

    public override void Render()
    {
        if (!HasStateAuthority)
            return;

        if (_desktopCamera == null)
        {
            _desktopCamera = transform.Find("Camera").GetComponent<Camera>();
            if (_desktopCamera == null)
                return;
            else 
                Debug.Log("Find camera!");
        }

        if (_targetVRPlayer == null || _targetVRPlayer.Object == null || !_targetVRPlayer.Object.IsValid)
        {
            _targetVRPlayer = FindVRPlayer();
            if (_targetVRPlayer == null)
                return;
        }

        _desktopCamera.transform.position = _targetVRPlayer.NetworkedHeadPosition;
        _desktopCamera.transform.rotation = _targetVRPlayer.NetworkedHeadRotation;
    }

    private VRPlayerController FindVRPlayer()
    {
        foreach (var obj in Runner.GetAllNetworkObjects())
        {
            if (obj.TryGetComponent<VRPlayerController>(out var vrPlayer) && vrPlayer.IsVRPlayer)
                return vrPlayer;
        }

        return null;
    }
}
