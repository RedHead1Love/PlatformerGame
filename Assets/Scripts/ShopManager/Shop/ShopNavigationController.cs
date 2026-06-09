using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShopLogic
{
    public sealed class ShopNavigationController : MonoBehaviour
    {
        [SerializeField] private Selectable _firstSelected;
        [SerializeField] private ShopUIManager _shopUIManager;

        private void Update()
        {
            if (_shopUIManager != null && _shopUIManager.IsShopOpen)
            {
                EnsureItemSelected();
            }
        }

        private void EnsureItemSelected()
        {
            if (EventSystem.current.currentSelectedGameObject == null && _firstSelected != null)
            {
                _firstSelected.Select();
            }
        }

        public void FocusFirstItem()
        {
            if (_firstSelected != null)
            {
                _firstSelected.Select();
            }
        }
    }
}
