using UnityEngine;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Notification and Dialog tests.
	/// </summary>
	public class UITestSamples : MonoBehaviour
	{
		/// <summary>
		/// Question icon.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("questionIcon")]
		protected Sprite QuestionIcon;

		/// <summary>
		/// Attention icon.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("attentionIcon")]
		protected Sprite AttentionIcon;

		/// <summary>
		/// Simple notification template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notifySimple")]
		protected Notify NotifySimpleTemplate;

		/// <summary>
		/// Autohide notification template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notifyAutoHide")]
		protected Notify NotifyAutoHideTemplate;

		/// <summary>
		/// Sample dialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dialogSample")]
		protected Dialog DialogSampleTemplate;

		/// <summary>
		/// Sign-in dialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dialogSignIn")]
		protected Dialog DialogSignInTemplate;

		/// <summary>
		/// TreeView dialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("dialogTreeView")]
		protected Dialog DialogTreeViewTemplate;

		/// <summary>
		/// Popup template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("popupSample")]
		protected Popup PopupTemplate;

		/// <summary>
		/// Modal popup template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("popupModalSample")]
		protected Popup PopupModalTemplate;

		/// <summary>
		/// Show sticky notification.
		/// </summary>
		public void ShowNotifySticky()
		{
			NotifySimpleTemplate.Clone().Show(
				"Sticky Notification. Click on the × above to close.",
				customHideDelay: 0f
			);
		}

		/// <summary>
		/// Show 3 notification, one by one in this order:
		/// - Queue Notification 3
		/// - Queue Notification 2
		/// - Queue Notification 1
		/// </summary>
		public void ShowNotifyStack()
		{
			NotifySimpleTemplate.Clone().Show("Stack Notification 1.",
											customHideDelay: 3f,
											sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show("Stack Notification 2.",
											customHideDelay: 3f,
											sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show("Stack Notification 3.",
											customHideDelay: 3f,
											sequenceType: NotifySequence.First);
		}

		/// <summary>
		/// Show 3 notification, one by one in this order:
		/// - Queue Notification 1.
		/// - Queue Notification 2.
		/// - Queue Notification 3.
		/// </summary>
		public void ShowNotifyQueue()
		{
			NotifySimpleTemplate.Clone().Show("Queue Notification 1.",
											customHideDelay: 3f,
											sequenceType: NotifySequence.Last);
			NotifySimpleTemplate.Clone().Show("Queue Notification 2.",
											customHideDelay: 3f,
											sequenceType: NotifySequence.Last);
			NotifySimpleTemplate.Clone().Show("Queue Notification 3.",
											customHideDelay: 3f,
											sequenceType: NotifySequence.Last);
		}


		/// <summary>
		/// Show only one notification and hide current notifications from sequence, if exists.
		/// Will be displayed only Queue Notification 3.
		/// </summary>
		public void ShowNotifySequenceClear()
		{
			NotifySimpleTemplate.Clone().Show("Stack Notification 1.",
											customHideDelay: 3f,
											sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show("Stack Notification 2.",
											customHideDelay: 3f,
											sequenceType: NotifySequence.First);
			NotifySimpleTemplate.Clone().Show("Stack Notification 3.",
											customHideDelay: 3f,
											sequenceType: NotifySequence.First,
											clearSequence: true);
		}

		/// <summary>
		/// Show notify and close after 3 seconds.
		/// </summary>
		public void ShowNotifyAutohide()
		{
			NotifyAutoHideTemplate.Clone().Show("Achievement unlocked. Hide after 3 seconds.", customHideDelay: 3f);
		}

		bool CallShowNotifyAutohide()
		{
			ShowNotifyAutohide();
			return true;
		}

		/// <summary>
		/// Show notify with rotate animation.
		/// </summary>
		public void ShowNotifyAutohideRotate()
		{
			NotifyAutoHideTemplate.Clone().Show(
				"Achievement unlocked. Hide after 4 seconds.",
				customHideDelay: 4f,
				hideAnimation: Notify.AnimationRotate
			);
		}

		/// <summary>
		/// Show notify wuth collapse animation.
		/// </summary>
		public void ShowNotifyBlack()
		{
			NotifyAutoHideTemplate.Clone().Show(
				"Another Notification. Hide after 5 seconds or click on the × above to close.",
				customHideDelay: 5f,
				hideAnimation: Notify.AnimationCollapse,
				slideUpOnHide: false
			);
		}

		bool ShowNotifyYes()
		{
			NotifyAutoHideTemplate.Clone().Show("Action on 'Yes' button click.", customHideDelay: 3f);
			return true;
		}

		bool ShowNotifyNo()
		{
			NotifyAutoHideTemplate.Clone().Show("Action on 'No' button click.", customHideDelay: 3f);
			return true;
		}

		/// <summary>
		/// Show simple dialog.
		/// </summary>
		public void ShowDialogSimple()
		{
			var canvas = Utilites.FindTopmostCanvas(transform).GetComponent<Canvas>();

			var dialog = DialogSampleTemplate.Clone();

			dialog.Show(
				title: "Simple Dialog",
				message: "Simple dialog with only close button.",
				buttons: new DialogActions(){
					{"Close", Dialog.Close},
				},
				focusButton: "Close",
				canvas: canvas
			);
		}

		/// <summary>
		/// Show dialog in the same position when it was closed.
		/// </summary>
		public void ShowDialogInPosition()
		{
			var dialog = DialogSampleTemplate.Clone();
			dialog.Show(
				title: "Simple Dialog",
				message: "Simple dialog with only close button.",
				buttons: new DialogActions(){
					{ "Close", () => Close(dialog) },
				},
				focusButton: "Close",
				position: dialog.transform.localPosition
			);
		}
		
		/// <summary>
		/// Check if dialog can be closed.
		/// </summary>
		/// <param name="currentInstance">Current dialog.</param>
		/// <returns>true if dialog can be closed; otherwise, false.</returns>
		public virtual bool Close(Dialog currentInstance)
		{
			return true;
		}

		bool CallShowDialogSimple()
		{
			ShowDialogSimple();
			return true;
		}

		/// <summary>
		/// Show warning.
		/// </summary>
		public void ShowWarning()
		{
			DialogSampleTemplate.Clone().Show(
				title: "Warning window",
				message: "Warning test",
				buttons: new DialogActions(){
					{"OK", Dialog.Close},
				},
				focusButton: "OK",
				icon: AttentionIcon
			);
		}

		/// <summary>
		/// Show dialog with Yes/No/Cancel buttons.
		/// </summary>
		public void ShowDialogYesNoCancel()
		{
			DialogSampleTemplate.Clone().Show(
				title: "Dialog Yes No Cancel",
				message: "Question?",
				buttons: new DialogActions(){
					{"Yes", ShowNotifyYes},
					{"No", ShowNotifyNo},
					{"Cancel", Dialog.Close},
				},
				focusButton: "Yes",
				icon: QuestionIcon
			);
		}

		/// <summary>
		/// Show dialog with lots of text.
		/// </summary>
		public void ShowDialogExtended()
		{
			DialogSampleTemplate.Clone().Show(
				title: "Another Dialog",
				message: "Same template with another position and long text.\nChange\nheight\nto\nfit\ntext.",
				buttons: new DialogActions(){
					{"Show notification", CallShowNotifyAutohide},
					{"Open simple dialog", CallShowDialogSimple},
					{"Close", Dialog.Close},
				},
				focusButton: "Show notification",
				position: new Vector3(40, -40, 0)
			);
		}

		/// <summary>
		/// Show modal dialog.
		/// </summary>
		public void ShowDialogModal()
		{
			DialogSampleTemplate.Clone().Show(
				title: "Modal Dialog",
				message: "Simple Modal Dialog.",
				buttons: new DialogActions(){
					{"Close", Dialog.Close},
				},
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f)
			);
		}

		/// <summary>
		/// Show sing-in dialog.
		/// </summary>
		public void ShowDialogSignIn()
		{
			// create dialog from template
			var dialog = DialogSignInTemplate.Clone();
			// helper component with references to input fields
			var helper = dialog.GetComponent<DialogInputHelper>();
			// reset input fields to default
			helper.Refresh();

			// open dialog
			dialog.Show(
				title: "Sign into your Account",
				buttons: new DialogActions(){
					// on click call SignInNotify
					{"Sign in", () => SignInNotify(helper)},
					// on click close dialog
					{"Cancel", Dialog.Close},
				},
				focusButton: "Sign in",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f)
			);
		}

		// using dialog
		bool SignInNotify(DialogInputHelper helper)
		{
			// return true if Username.text and Password not empty; otherwise, false
			if (!helper.Validate())
			{
				// return false to keep dialog open
				return false;
			}

			// using dialog input 
			var message = "Sign in.\nUsername: " + helper.Username.text + "\nPassword: <hidden>";
			NotifyAutoHideTemplate.Clone().Show(message, customHideDelay: 3f);

			// return true to close dialog
			return true;
		}

		/// <summary>
		/// Show dialog with TreeView.
		/// </summary>
		public void ShowDialogTreeView()
		{
			// create dialog from template
			var dialog = DialogTreeViewTemplate.Clone();
			// helper component with references to input fields
			var helper = dialog.GetComponent<DialogTreeViewInputHelper>();
			
			// open dialog
			dialog.Show(
				title: "Dialog with TreeView",
				buttons: new DialogActions(){
					// on click close dialog
					{"Close", Dialog.Close},
				},
				focusButton: "Close",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f)
			);
			
			// reset input fields to default
			helper.Refresh();
		}

		/// <summary>
		/// Show simple popup.
		/// </summary>
		public void ShowPopup()
		{
			PopupTemplate.Clone().Show(
				title: "Simple Popup",
				message: "Simple Popup."
			);
		}

		/// <summary>
		/// Show modal popup.
		/// </summary>
		public void ShowPopupModal()
		{
			PopupModalTemplate.Clone().Show(
				title: "Modal Popup",
				message: "Alert text.",
				modal: true,
				modalColor: new Color(0.0f, 0.0f, 0.0f, 0.8f)
			);
		}
	}
}