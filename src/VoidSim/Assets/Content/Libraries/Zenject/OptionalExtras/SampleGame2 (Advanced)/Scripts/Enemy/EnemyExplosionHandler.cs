using System;
using UnityEngine;
using Zenject;

namespace Zenject.SpaceFighter
{
    public class EnemyDeathHandler
    {
        readonly GameEvents _gameEvents;
        readonly EnemyFacade.Pool _selfFactory;
        readonly Settings _settings;
        readonly Explosion.Pool _explosionPool;
        readonly AudioPlayer _audioPlayer;
        readonly Enemy _enemy;
        readonly EnemyFacade _facade;

        public EnemyDeathHandler(
            Enemy enemy,
            AudioPlayer audioPlayer,
            Explosion.Pool explosionPool,
            Settings settings,
            EnemyFacade.Pool selfFactory,
            GameEvents gameEvents,
            EnemyFacade facade)
        {
            _facade = facade;
            _gameEvents = gameEvents;
            _selfFactory = selfFactory;
            _settings = settings;
            _explosionPool = explosionPool;
            _audioPlayer = audioPlayer;
            _enemy = enemy;
        }

        public void Die()
        {
            var explosion = _explosionPool.Spawn();
            explosion.transform.position = _enemy.Position;

            _audioPlayer.Play(_settings.DeathSound, _settings.DeathSoundVolume);

            _gameEvents.EnemyKilled();

            _selfFactory.Despawn(_facade);
        }

        [Serializable]
        public class Settings
        {
            public AudioClip DeathSound;
            public float DeathSoundVolume = 1.0f;
        }
    }
}
