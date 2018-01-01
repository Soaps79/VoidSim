using Assets.Placeables.Nodes;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Placeables.UI
{
    /// <summary>
    /// Represents the pop container placeable node.
    /// </summary>
    public class PopContainerViewModel : QScript
    {
        [SerializeField] private Image _personPrefab;
        [SerializeField] private Sprite _spriteFilled;
        [SerializeField] private Sprite _spriteOutlineFilled;
        [SerializeField] private Sprite _spriteOutlineEmpty;
        [SerializeField] private Color _residentColor;
        private int _childCount;

        public void Initialize(PopContainer popContainer)
        {
            if(_personPrefab == null || _spriteFilled == null 
                || _spriteOutlineFilled == null || _spriteOutlineEmpty == null)
                throw new UnityException("PopContainerViewModel missing static data");

            for (int i = 0; i < popContainer.CurrentOccupants.Count; i++)
            {
                AddPersonAvatar(_spriteFilled);
            }

            if (_childCount < popContainer.Reserved)
            {
                var reservedToDisplay = popContainer.Reserved - _childCount;
                for (int i = 0; i < reservedToDisplay; i++)
                {
                    AddPersonAvatar(_spriteOutlineFilled);
                }
            }

            if (_childCount < popContainer.MaxCapacity)
            {
                var emptyCount = popContainer.MaxCapacity - _childCount;
                for (int i = 0; i < emptyCount; i++)
                {
                    AddPersonAvatar(_spriteOutlineEmpty);
                }
            }
        }

        private void AddPersonAvatar(Sprite personSprite)
        {
            var avatar = Instantiate(_personPrefab, transform, false);
            avatar.sprite = personSprite;
            avatar.color = _residentColor;
            _childCount++;
        }
    }
}