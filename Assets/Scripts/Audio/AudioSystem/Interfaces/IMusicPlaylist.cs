using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public interface IMusicPlaylist
    {
        bool HasTracks { get; }

        void Initialize(List<AudioClip> tracks);
        AudioClip GetNextTrack();
        void Shuffle();
    }
}