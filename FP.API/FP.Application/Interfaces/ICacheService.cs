namespace FP.Application.Interfaces
{
	public interface ICacheService
	{
		Task Set(string key, object value, int minutes);
		Task<T?> Get<T>(string key);
	}
}
