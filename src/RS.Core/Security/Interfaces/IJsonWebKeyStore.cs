using RS.Core.Security.Model;
using System.Collections.ObjectModel;

namespace RS.Core.Security.Interfaces;

public interface IJsonWebKeyStore
{
	Task Store(KeyMaterial keyMaterial);
	Task<KeyMaterial> GetCurrent();
	Task Revoke(KeyMaterial keyMaterial, string reason = default);
	Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int quantity);
	Task<KeyMaterial> Get(string keyId);
	Task Clear();
}
