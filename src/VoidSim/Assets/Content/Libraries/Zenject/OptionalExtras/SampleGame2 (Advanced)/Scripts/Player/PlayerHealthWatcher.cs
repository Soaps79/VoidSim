using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class PlayerHealthWatcher : ITickable
    {
        readonly AudioPlayer _audioPlayer;
        readonly Settings _settings;
        readonly GameEvents _gameEvents;
        readonly Explosion.Pool _explosionPool;
        readonly Player _player;

        public PlayerHealthWatcher(
            Player player,
            Explosion.Pool explosionPool,
            GameEvents gameEvents,
            Settings settings,
            AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
            _settings = settings;
            _gameEvents = gameEvents;
            _explosionPool = explosionPool;
            _player = player;
        }

        public void Tick()
        {
            if (_player.Health <= 0 && !_player.IsDead)
            {
                Die();
            }
        }

        void Die()
        {
            _player.IsDead = true;

            var explosion = _explosionPool.Spawn();
            explosion.transform.position = _player.Position;

            _player.Renderer.enabled = false;

            _gameEvents.PlayerDied();

            _audioPlayer.Play(_settings.DeathSound, _settings.DeathSoundVolume);
        }

        [Serializable]
        public class Settings
        {
            public AudioClip DeathSound;
            public float DeathSoundVolume = 1.0f;
        }
    }
}
