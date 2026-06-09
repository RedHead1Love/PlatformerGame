using UnityEngine;
using YG;

namespace Assets.Scripts.InteractionWithEnvironment
{
    public sealed class MobileInterfaceSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _mobilePanel;

        private void Awake()
        {
            ApplyMobilePanelState();
        }

        private void ApplyMobilePanelState()
        {
            if (_mobilePanel == null)
            {
                return;
            }

            _mobilePanel.SetActive(YG2.envir.isMobile);
        }
    }
}