using System.Collections.Generic;
using Assets.Narrative.Conversations;
using Assets.Narrative.Missions;
using Assets.Narrative.Notifications;
using Assets.Narrative.UI;
using Assets.Scripts;
using Assets.Scripts.Initialization;
using Assets.Scripts.Serialization;
using QGame;
using UnityEngine;

namespace Assets.Narrative
{
	public class NarrativeProgressData
	{
		public MissionsProgressData Missions;
		public ConversationsProgressData Conversations;
	}

	/// <summary>
	/// Orchestrate the Narrative by handing objects off to a number of specialized subsytems
	/// </summary>
	public class NarrativeMonitor : QScript, ISerializeData<NarrativeProgressData>
	{
		[SerializeField] private NotificationListViewModel _notificationPrefab;
		private NotificationListViewModel _notificationsViewModel;

		[SerializeField] private DialogueChain _chain;
	    [SerializeField] private DialogueController _dialogueController;

		[SerializeField] private LevelPackage _initialPackage;

		private readonly CollectionSerializer<NarrativeProgressData> _serializer
			= new CollectionSerializer<NarrativeProgressData>();

		private Canvas _canvas;
		private MissionsMonitor _missionsMonitor;
		private ConversationsMonitor _conversationsMonitor;
		private NarrativeProgressData _data;


		void Start()
		{
			// delay to let the game objects get set up
			OnNextUpdate += Initialize;
			_missionsMonitor = gameObject.GetComponent<MissionsMonitor>();
			_conversationsMonitor = gameObject.GetComponent<ConversationsMonitor>();
		}

		private void Initialize(float obj)
		{
			_canvas = Locator.CanvasManager.GetCanvas(CanvasType.MediumUpdate);

            // if there is loading data, bring missions up to date
            if (_serializer.HasDataFor(this, "Narrative"))
				_data = _serializer.DeserializeData();

            InitializeMissions();
            //InitializeConversations();
            InitializeNotifications();
            InitializeConversationsMonitor();
            //_chain.StartChain();
        }

		private void InitializeMissions()
		{
			_missionsMonitor.Initialize(_initialPackage);
			if(_data != null)
				_missionsMonitor.SetFromData(_data.Missions);
		}

		private void InitializeConversationsMonitor()
		{
			// initialize the conversation monitor
			// it needs convo data to start, but missions data when handling the package
			// this could probably be refactored
			if (_data != null)
				_conversationsMonitor.SetFromData(_data.Conversations);
			_conversationsMonitor.HandleLevelPackage(
				_initialPackage, _data != null ? _data.Missions.CompletedMissions : new List<string>());
		}

		private void InitializeNotifications()
		{
			_notificationsViewModel = Instantiate(_notificationPrefab, _canvas.transform, false);
			_notificationsViewModel.Initialize(_conversationsMonitor);
			_conversationsMonitor.InitializeUi(_notificationsViewModel);
		}

		public NarrativeProgressData GetData()
		{
			var data = new NarrativeProgressData
			{
				Missions = _missionsMonitor.GetData(),
				Conversations = _conversationsMonitor.GetData()
			};
			return data;
		}
	}
}