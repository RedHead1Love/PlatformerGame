using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    internal sealed class ArmorView
    {
        private const string ArmorPanelName = "ArmorPanel_Runtime";
        private const float DisabledArmorAlpha = 0.3f;
        private GameObject _armorPanelInstance;
        private Image[] _armorIcons;

        public void UpdateIcons(int current, int maximum)
        {
            if (_armorIcons == null)
            {
                return;
            }

            int iconCount = Mathf.Min(_armorIcons.Length, maximum);

            for (int i = 0; i < iconCount; i++)
            {
                Image armorIcon = _armorIcons[i];

                if (armorIcon == null)
                {
                    continue;
                }

                armorIcon.enabled = true;
                armorIcon.color = i < current ? Color.white : new Color(1f, 1f, 1f, DisabledArmorAlpha);
            }
        }

        public void Show(GameObject prefab)
        {
            if (_armorPanelInstance == null)
            {
                InitializeArmorUI(prefab);
            }

            if (_armorPanelInstance != null)
            {
                _armorPanelInstance.SetActive(true);
            }
        }

        public void Hide()
        {
            if (_armorPanelInstance != null)
            {
                _armorPanelInstance.SetActive(false);
            }
        }

        private void InitializeArmorUI(GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }

            GameObject armorContainer = GameObject.FindGameObjectWithTag("ArmorContainer");

            if (armorContainer == null)
            {
                return;
            }

            _armorPanelInstance = Object.Instantiate(prefab, armorContainer.transform);

            RectTransform rectTransform = _armorPanelInstance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }

            _armorPanelInstance.name = ArmorPanelName;
            _armorPanelInstance.SetActive(true);

            _armorIcons = _armorPanelInstance.GetComponentsInChildren<Image>(true);
        }
    }
}
