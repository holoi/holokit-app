using System;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_RescanQRCode : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_RescanQRCode";

        public override bool OverlayPreviousPanel => true;

        public static event Action OnRescanQRCode;

        public static event Action OnCancelRescanQRCode;

        private void Start()
        {
            OnRescanQRCode?.Invoke();
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            OnCancelRescanQRCode?.Invoke();
        }

        public void OnRescanButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
            HoloKitApp.Instance.UIPanelManager.PushUIPanel("MonoAR_ScanQRCode");
            HoloKitApp.Instance.MultiplayerManager.OnRescanQRCode();
        }
    }
}
