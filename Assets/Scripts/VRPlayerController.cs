using Fusion;
using UnityEngine;
using UnityEngine.XR;

public class VRPlayerController : NetworkBehaviour
{
    [SerializeField] private Transform headVisual;
    [SerializeField] private float fillSpeed = 0.4f;

    [Networked] public Vector3 NetworkedHeadPosition { get; set; }
    [Networked] public Quaternion NetworkedHeadRotation { get; set; }
    [Networked] public NetworkBool IsVRPlayer { get; set; }
    [Networked] public float TriggerFillAmount { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            IsVRPlayer = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        var cam = Camera.main;
        if (cam != null)
        {
            NetworkedHeadPosition = cam.transform.position;
            NetworkedHeadRotation = cam.transform.rotation;
        }

        // トリガー入力でゲージ更新
        if (IsTriggerPressed())
        {
            TriggerFillAmount += fillSpeed * Runner.DeltaTime;

            if (TriggerFillAmount >= 1f)
            {
                TriggerFillAmount = 1f;
                Runner.LoadScene(SceneRef.FromIndex(1));
            }
        }
        else
        {
            TriggerFillAmount = 0f;
        }
    }

    public override void Render()
    {
        if (headVisual == null)
            return;

        headVisual.position = NetworkedHeadPosition;
        headVisual.rotation = NetworkedHeadRotation;
    }

    private bool IsTriggerPressed()
    {
        var rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHand.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            return triggerValue > 0.5f;
        return false;
    }
}
