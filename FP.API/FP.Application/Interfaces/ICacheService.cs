namespace FP.Application.Interfaces
{
	public interface ICacheService
	{
		Task Set(string key, object value, int minutes);
		Task Reset(string key);
		Task<T> Get<T>(string key);
	}
}
