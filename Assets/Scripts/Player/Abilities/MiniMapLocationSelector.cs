using System.Collections.Generic;

public sealed class MiniMapLocationSelector
{
    private readonly IReadOnlyList<MiniMapData> _maps;
    public MiniMapData CurrentMap { get; private set; }

    public MiniMapLocationSelector(IReadOnlyList<MiniMapData> maps)
    {
        _maps = maps;
        if (maps != null && maps.Count > 0)
        {
            CurrentMap = maps[0];
        }
    }

    public bool TrySelect(int index)
    {
        if (_maps == null || index < 0 || index >= _maps.Count ||
            _maps[index] == null || _maps[index].MapTexture == null)
        {
            return false;
        }
        CurrentMap = _maps[index];
        return true;
    }

    public bool TrySelect(string locationName)
    {
        if (_maps == null)
        {
            return false;
        }
        for (int index = 0; index < _maps.Count; index++)
        {
            if (_maps[index] != null && _maps[index].LocationName == locationName)
            {
                return TrySelect(index);
            }
        }
        return false;
    }
}
