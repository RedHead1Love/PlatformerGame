using Player.Input;
using UnityEngine;
using YG;

public sealed class MiniMapInputHandler
{
    private readonly KeyCode _toggleKey;
    private IInputProvider _provider;

    public MiniMapInputHandler(KeyCode toggleKey)
    {
        _toggleKey = toggleKey;
    }

    public bool IsTogglePressed()
    {
        if (_provider == null || (_provider is Object providerObject && providerObject == null))
        {
            FindProvider();
        }
        return (_provider != null && _provider.IsOpenMapPressed) || Input.GetKeyDown(_toggleKey);
    }

    public void Reset()
    {
        _provider = null;
    }

    private void FindProvider()
    {
        _provider = Object.FindFirstObjectByType<AggregatedInputProvider>();
        if (_provider == null && YG2.envir.isDesktop)
        {
            _provider = Object.FindFirstObjectByType<LegacyInputProvider>();
        }
        if (_provider == null && YG2.envir.isMobile)
        {
            _provider = Object.FindFirstObjectByType<JoystickInput>();
        }
    }
}
