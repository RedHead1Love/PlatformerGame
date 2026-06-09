using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public sealed class MusicPlaylist : IMusicPlaylist
    {
        private const int ResetIndex = 0;

        private List<AudioClip> _tracks = new List<AudioClip>();
        private List<AudioClip> _shuffledTracks = new List<AudioClip>();
        private int _currentTrackIndex = 0;

        public bool HasTracks => _tracks.Count > 0;

        public void Initialize(List<AudioClip> tracks)
        {
            _tracks = new List<AudioClip>(tracks);
            _shuffledTracks = new List<AudioClip>(tracks);

            Shuffle();
        }

        public AudioClip GetNextTrack()
        {
            if (_shuffledTracks.Count == 0)
            {
                return null;
            }

            if (_currentTrackIndex >= _shuffledTracks.Count)
            {
                Shuffle();
            }

            AudioClip nextTrack = _shuffledTracks[_currentTrackIndex];
            _currentTrackIndex++;

            return nextTrack;
        }

        public void Shuffle()
        {
            _shuffledTracks = new List<AudioClip>(_tracks);

            for (int i = _shuffledTracks.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_shuffledTracks[i], _shuffledTracks[j]) = (_shuffledTracks[j], _shuffledTracks[i]);
            }

            _currentTrackIndex = ResetIndex;
        }
    }
}