using System;
using HoloKit;

namespace Holoi.Library.HoloKitApp.UI
{
    public static class HoloKitAppUIEventManager
    {
        #region Mono AR Panel
        public static Action<HoloKitRenderMode> OnRenderModeChanged;

        public static Action OnExitReality;
        #endregion

        #region Reality Settings Panel
        public static Action<bool> OnHumanOcclusionToggled;
        #endregion

        #region Star AR Panel
        public static Action OnTriggered;

        public static Action OnBoosted;
        #endregion

        #region Others
        public static Action OnExitNoLiDARScene;
        #endregion
    }
}