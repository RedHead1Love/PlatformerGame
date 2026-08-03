using System.Collections;
using UnityEngine;

public sealed class ObjectiveDisplay : MonoBehaviour
{
    private const float DefaultDisplayDuration = 5f;

    [Header("UI References")]
    [SerializeField] private GameObject _objectivePanel;

    [Header("Settings")]
    [SerializeField] private KeyCode _showKey = KeyCode.Tab;
    [SerializeField] private float _displayDuration = DefaultDisplayDuration;

    private bool _isShowing;

    private void Start()
    {
        if (_objectivePanel != null)
        {
            _objectivePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(_showKey) && _isShowing == false)
        {
            ShowObjective();
        }
    }

    private void ShowObjective()
    {
        if (_objectivePanel == null)
        {
            return;
        }

        StartCoroutine(DisplayRoutine());
    }

    private IEnumerator DisplayRoutine()
    {
        _isShowing = true;

        _objectivePanel.SetActive(true);

        yield return new WaitForSeconds(_displayDuration);

        _objectivePanel.SetActive(false);
        _isShowing = false;
    }
}