using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class StartGaugeDisplay : MonoBehaviour
{
    public Image gaugeFill;

    private VRPlayerController _vrPlayer;

    private void Update()
    {
        if (_vrPlayer == null)
        {
            _vrPlayer = FindVRPlayer();
            if (_vrPlayer == null)
                return;
        }

        if (_vrPlayer.Object == null || !_vrPlayer.Object.IsValid)
            return;

        gaugeFill.fillAmount = _vrPlayer.TriggerFillAmount;
    }

    private VRPlayerController FindVRPlayer()
    {
        var runner = FindObjectOfType<NetworkRunner>();
        if (runner == null)
            return null;

        foreach (var obj in runner.GetAllNetworkObjects())
        {
            if (obj.TryGetComponent<VRPlayerController>(out var vr) && vr.IsVRPlayer)
                return vr;
        }

        return null;
    }
}
