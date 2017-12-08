using Assets.Narrative.Conversations;
using Assets.Narrative.Missions;
using Assets.Narrative.Notifications;
using Assets.Narrative.UI;
using Assets.Scripts.Initialization;
using Assets.Scripts.Serialization;
using QGame;
using UnityEngine;

namespace Assets.Narrative
{
	public class NarrativeProgressData
	{
		public MissionsProgressData Missions;
	}

	/// <summary>
	/// Orchestrate the Narrative by handing objects off to a number of specialized subsytems
	/// </summary>
	public class NarrativeMonitor : QScript, ISerializeData<NarrativeProgressData>
	{
		[SerializeField] private Conversation _initialConversation;
		[SerializeField] private ConversationViewModel _conversationViewModelPrefab;
		private ConversationViewModel _conversationViewModel;

		[SerializeField] private NotificationListViewModel _notificationPrefab;
		private NotificationListViewModel _notificationsViewModel;

		[SerializeField] private LevelPackage _initialPackage;

		private readonly CollectionSerializer<NarrativeProgressData> _serializer
			= new CollectionSerializer<NarrativeProgressData>();

		private GameObject _canvas;
		private MissionsMonitor _missionsMonitor;
		private ConversationsMonitor _conversationsMonitor;
		private MissionsProgressData _deserializedMissionData;


		void Start()
		{
			// delay to let the game objects get set up
			OnNextUpdate += Initialize;
			_missionsMonitor = gameObject.GetComponent<MissionsMonitor>();
			_conversationsMonitor = gameObject.GetComponent<ConversationsMonitor>();
		}

		private void Initialize(float obj)
		{
			_canvas = GameObject.Find("InfoCanvas");

			// if there is loading data, bring missions up to date
			if (_serializer.HasDataFor(this, "Narrative"))
				HandleDataLoad();

			_missionsMonitor.Initialize(_initialPackage, _deserializedMissionData);

			InitializeConversations();
			InitializeNotifications();
		}

		private void HandleDataLoad()
		{
			var data = _serializer.DeserializeData();
			if (data != null)
			{
				_deserializedMissionData = data.Missions;
			}
		}

		private void InitializeConversations()
		{
			_conversationViewModel = Instantiate(_conversationViewModelPrefab, _canvas.transform, false);
			_conversationViewModel.OnMissionsNeedStart += _missionsMonitor.ActivateMissionGroup;
			_conversationViewModel.gameObject.SetActive(false);
		}

		private void InitializeNotifications()
		{
			_notificationsViewModel = Instantiate(_notificationPrefab, _canvas.transform, false);
			_notificationsViewModel.Initialize(_conversationViewModel);
			
			_conversationsMonitor.InitializeUi(_notificationsViewModel);
			_conversationsMonitor.HandleLevelPackage(_initialPackage);
		}

		

		public NarrativeProgressData GetData()
		{
			var data = new NarrativeProgressData
			{
				Missions = _missionsMonitor.GetData()
			};
			return data;
		}
	}
}