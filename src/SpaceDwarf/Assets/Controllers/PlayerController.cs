using Assets.Model;
using QGame;
using UnityEngine;

namespace Assets.Controllers
{
    public class PlayerController : QScript
    {
        public GameObject CharacterPrefab;
        public float MoveSpeed = 5;

        private PlayerCharacter _character;
        private Animator _characterAnimator;


        // Use this for initialization
        void Start ()
        {
            // todo: load character profile
           _character = new PlayerCharacter(Vector2.zero);
            
            // create Player game object
            CreatePlayerView();

            // hook updates
            OnEveryUpdate += UpdateCharacter;
        }

        private void CreatePlayerView()
        {
            var characterGo = Instantiate(CharacterPrefab);

            // assign parent and position
            characterGo.transform.parent = transform;
            characterGo.transform.position = _character.Position;
            
            //// create sprite component
            //var spriteRenderer = characterGo.AddComponent<SpriteRenderer>();
            //spriteRenderer.sprite = CharacterSprite;
            //spriteRenderer.sortingLayerName = "Player";

            _characterAnimator = characterGo.GetComponent<Animator>();

            // center camera on player
            CenterCameraOnPlayer();

            // hook player events
            _character.RegisterOnPositionChangedCallback((character, position) =>
            {
                OnCharacterPositionChanged(character, position, characterGo);
            });
        }

        private void OnCharacterPositionChanged(PlayerCharacter character, Vector2 oldPosition, GameObject characterGo)
        {
            // update game object to character position
            characterGo.transform.position = character.Position;
            
            // center camera
            CenterCameraOnPlayer();
        }

        private void CenterCameraOnPlayer()
        {
            Camera.main.transform.position = new Vector3(
                _character.Position.x, 
                _character.Position.y, 
                Camera.main.transform.position.z);
        }

        private void UpdateCharacter(float timeDelta)
        {
            UpdateCharacterPosition(timeDelta);
        }

        private void UpdateCharacterPosition(float timeDelta)
        {
            // check for input, calculate movement vector
            var horizontal = Input.GetAxis("Horizontal") * MoveSpeed * timeDelta;
            var vertical = Input.GetAxis("Vertical") * MoveSpeed * timeDelta;
            var movement = new Vector2(horizontal, vertical);

            // update animation
            _characterAnimator.SetFloat("MoveX", horizontal);
            _characterAnimator.SetFloat("MoveY", vertical);

            //Debug.Log(string.Format("MoveY: ({0})", vertical));

            // update player character
            _character.Move(movement);
        }
    }
}
