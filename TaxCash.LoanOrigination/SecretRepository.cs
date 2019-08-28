using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System.Collections.Generic;
using System.IdentityModel.Tokens;

namespace TaxCash.LoanOrigination
{
	public class SecretRepository
	{
		public enum Key
		{
			Jwt
		}
		private static readonly IDictionary<Key, string> keyDictionary = new Dictionary<Key, string>
				{
						{ Key.Jwt, NLSJWTKey }
				};

		private static readonly string NLSUserNameKey = "https://taxcash.vault.azure.net/secrets/NLSWebServiceUserName/e3ab502fa56246ea942542c0c482ca4d";
		private static readonly string NLSPasswordKey = "https://taxcash.vault.azure.net/secrets/NLSWebServicePassword/84620d196ecc45fa9627770d01fc3c87";
		private static readonly string NLSJWTKey = "https://taxcash.vault.azure.net/keys/NLSJWTKey/7d7a81e0dc974157a690cc8f4fce2970";
		public SecretRepository(AzureServiceTokenProvider tokenProvider)
		{
			TokenProvider = tokenProvider;
		}
		public string GetNLSUserName()
		{
			return @"NLS\Tcws8680";
		}

		public string GetNLSPassword()
		{
			return "35#gZ%729";
		}

		/*
		public RsaSecurityKey GetKey(Key key)
		{
			return GetKey(keyDictionary[key]);
		}
		*/
		public AzureServiceTokenProvider TokenProvider { get; }

		private string GetSecret(string key)
		{
			var keyVaultClient = new KeyVaultClient(
							new KeyVaultClient.AuthenticationCallback(TokenProvider.KeyVaultTokenCallback));

			var secretBundle = keyVaultClient.GetSecretAsync(key).ConfigureAwait(false).GetAwaiter().GetResult();
			return secretBundle.Value;
		}

		private RsaSecurityKey GetKey(string key)
		{
			var keyVaultClient = new KeyVaultClient(
							new KeyVaultClient.AuthenticationCallback(TokenProvider.KeyVaultTokenCallback));

			var keyBundle = keyVaultClient.GetKeyAsync(key).ConfigureAwait(false).GetAwaiter().GetResult();
			return new RsaSecurityKey(keyBundle.Key.ToRSA(true));
		}
	}
}
