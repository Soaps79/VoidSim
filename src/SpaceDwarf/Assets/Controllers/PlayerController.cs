using Assets.Controllers.GUI;
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

        private Vector3 _lastFramePosition;


        // Use this for initialization
        void Start ()
        {
            // todo: load character profile
           _character = new PlayerCharacter(Vector2.zero);
            
            // create Player game object
            CreatePlayerView();

            // hook updates
            OnEveryUpdate += UpdateCharacter;
            OnEveryUpdate += UpdateMouse;
        }

        private void CreatePlayerView()
        {
            var characterGo = Instantiate(CharacterPrefab);

            // assign parent and position
            characterGo.transform.parent = transform;
            characterGo.transform.position = _character.Position;
            characterGo.layer = 11;
            
            // grab reference to animator
            _characterAnimator = characterGo.GetComponent<Animator>();

            // todo: bake into prefab?
            // add collider and tooltip
            characterGo.AddComponent<BoxCollider>();
            var tooltip = characterGo.AddComponent<TooltipBehavior>();
            tooltip.TooltipText1 = "Character";
            tooltip.TooltipText2 = "The king baby!";

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

        private void UpdateMouse(float timeDelta)
        {
            // scroll wheel to zoom
            HandleMouseZoom();

            _lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
        }

        private void HandleMouseZoom()
        {
            // backward
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + 1, 10);
            }

            // forward
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - 1, 2);
            }
        }

        
    }
}
