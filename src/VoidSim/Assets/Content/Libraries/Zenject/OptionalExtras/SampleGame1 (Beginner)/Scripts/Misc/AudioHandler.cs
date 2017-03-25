using System;
using UnityEngine;
using Zenject;

namespace Zenject.Asteroids
{
    public class AudioHandler : IInitializable, IDisposable
    {
        readonly Settings _settings;
        readonly AudioSource _audioSource;

        GameEvents _gameEvents;

        public AudioHandler(
            GameEvents gameEvents,
            AudioSource audioSource,
            Settings settings)
        {
            _settings = settings;
            _audioSource = audioSource;
            _gameEvents = gameEvents;
        }

        public void Initialize()
        {
            _gameEvents.ShipCrashed += OnShipCrashed;
        }

        public void Dispose()
        {
            _gameEvents.ShipCrashed -= OnShipCrashed;
        }

        void OnShipCrashed()
        {
            _audioSource.PlayOneShot(_settings.CrashSound);
        }

        [Serializable]
        public class Settings
        {
            public AudioClip CrashSound;
        }
    }
}
