using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using RS.Identity.API.Security.Jwa;
using System.Security.Cryptography;

namespace RS.Identity.API.Security.Services;

internal static class CryptoService
{
	/// <summary>
	/// Creates a new RSA security key.
	/// Key size recommendations: https://www.keylength.com/en/compare/
	/// </summary>
	/// <returns></returns>
	public static RsaSecurityKey CreateRsaSecurityKey(int keySize = 3072)
	{
		return new RsaSecurityKey(RSA.Create(keySize))
		{
			KeyId = CreateUniqueId()
		};
	}

	internal static string CreateUniqueId(int length = 16)
	{
		return Base64UrlEncoder.Encode(CreateRandomKey(length));
	}

	/// <summary>
	/// Creates a new ECDSA security key.
	/// </summary>
	/// <param name="curve">The name of the curve as defined in
	/// https://tools.ietf.org/html/rfc7518#section-6.2.1.1.</param>
	/// <returns></returns>
	internal static ECDsaSecurityKey CreateECDsaSecurityKey(Algorithm algorithm)
	{
		var curve = algorithm.Curve;
		if (string.IsNullOrEmpty(algorithm.Curve))
			curve = GetCurveType(algorithm);

		return new ECDsaSecurityKey(ECDsa.Create(GetNamedECCurve(curve)))
		{
			KeyId = CreateUniqueId()
		};
	}

	internal static string GetCurveType(Algorithm algorithm)
	{
		return algorithm.Alg switch
		{
			SecurityAlgorithms.EcdsaSha256 => JsonWebKeyECTypes.P256,
			SecurityAlgorithms.EcdsaSha384 => JsonWebKeyECTypes.P384,
			SecurityAlgorithms.EcdsaSha512 => JsonWebKeyECTypes.P521,
			_ => throw new InvalidOperationException($"Unsupported curve type for {algorithm}")
		};
	}

	/// <summary>
	/// Creates a new HMAC security key.
	/// 
	/// Key size is selected based on NIST Special Publication 800-107 Revision 1 
	/// Recommendation for Applications Using Approved Hash Algorithms
	/// Section 5.3.4 Security Effect of the HMAC Key
	/// https://nvlpubs.nist.gov/nistpubs/Legacy/SP/nistspecialpublication800-107r1.pdf
	/// 
	/// Also RFC:
	/// https://www.rfc-editor.org/rfc/rfc4868#page-5
	/// </summary>
	internal static HMAC CreateHmacSecurityKey(Algorithm algorithm)
	{
		var hmac = algorithm.Alg switch
		{
			SecurityAlgorithms.HmacSha256 => (HMAC)new HMACSHA256(CreateRandomKey(64)),
			SecurityAlgorithms.HmacSha384 => new HMACSHA384(CreateRandomKey(128)),
			SecurityAlgorithms.HmacSha512 => new HMACSHA512(CreateRandomKey(128)),
			_ => throw new CryptographicException("Could not create HMAC key based on algorithm " + algorithm +
												  " (Could not parse expected SHA version)")
		};

		return hmac;
	}


	/// <summary>
	/// Creates a AES security key.
	/// </summary>
	internal static Aes CreateAESSecurityKey(Algorithm algorithm)
	{
		var aesKey = Aes.Create();
		var aesKeySize = algorithm.Alg switch
		{
			SecurityAlgorithms.Aes128KW => 128,
			SecurityAlgorithms.Aes256KW => 256,
			_ => throw new CryptographicException("Could not create AES key based on algorithm " + algorithm)
		};
		aesKey.KeySize = aesKeySize;
		aesKey.GenerateKey();
		return aesKey;
	}

	/// <summary>
	/// Returns the elliptic curve corresponding to the curve id.
	/// It matchs RFC 7518
	/// </summary>
	/// <param name="curveId">Represents ecdsa curve -P256, P384, P512</param>
	internal static ECCurve GetNamedECCurve(string curveId)
	{
		if (string.IsNullOrEmpty(curveId))
			throw LogHelper.LogArgumentNullException(nameof(curveId));

		// treat 512 as 521. It was a bug in .NET Release.
		return curveId switch
		{
			JsonWebKeyECTypes.P256 => ECCurve.NamedCurves.nistP256,
			JsonWebKeyECTypes.P384 => ECCurve.NamedCurves.nistP384,
			JsonWebKeyECTypes.P512 => ECCurve.NamedCurves.nistP521,
			JsonWebKeyECTypes.P521 => ECCurve.NamedCurves.nistP521,
			_ => throw LogHelper.LogExceptionMessage(new ArgumentException(curveId))
		};
	}

	/// <summary>Creates a random key byte array.</summary>
	/// <param name="length">The length.</param>
	/// <returns></returns>
	internal static byte[] CreateRandomKey(int length)
	{
		byte[] data = new byte[length];
		RandomNumberGenerator.Fill(data);
		return data;
	}
}
