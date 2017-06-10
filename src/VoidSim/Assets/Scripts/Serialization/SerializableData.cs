namespace Assets.Scripts.Serialization
{
	public interface ISerializeData<out T>
	{
		T GetData();
	}
}