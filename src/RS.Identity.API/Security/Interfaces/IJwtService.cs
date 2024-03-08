using Microsoft.IdentityModel.Tokens;
using RS.Identity.API.Security.Model;
using System.Collections.ObjectModel;

namespace RS.Identity.API.Security.Interfaces;

public interface IJwtService
{
	/// <summary>
	/// By default it will use JWS options to create a Key if doesn't exist
	/// If you want to use JWE, you must select RSA. Or use `CryptographicKey` class
	/// </summary>
	/// <returns></returns>
	Task<SecurityKey> GenerateKey();
	Task<SecurityKey> GetCurrentSecurityKey();
	Task<SigningCredentials> GetCurrentSigningCredentials();
	Task<EncryptingCredentials> GetCurrentEncryptingCredentials();
	Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int? i = null);
	Task RevokeKey(string keyId, string reason = null);
	Task<SecurityKey> GenerateNewKey();
}
