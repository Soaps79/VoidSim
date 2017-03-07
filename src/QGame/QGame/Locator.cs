using Messaging;


namespace QGame
{
	public static class Locator
	{
		private static IMessageHub _messages = new NullMessageHub();
		public static IMessageHub Messages { get { return _messages; } }

		private static IKeyValueDisplay _valueDisplay = new NullKeyValueDisplay();
		public static IKeyValueDisplay ValueDisplay { get { return _valueDisplay; } }

		public static void Initialize(IMessageHub messageHub, IKeyValueDisplay valueDisplay)
		{
			_messages = messageHub;
		    _valueDisplay = valueDisplay;
		}
	}
}