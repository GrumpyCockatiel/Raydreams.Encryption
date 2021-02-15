using System;

namespace Raydreams.Encryption.Security
{
	/// <summary>Struct to contain the message, key and init vector of a symmetric encryption when the key and IV are chosen randomly.</summary>
	public struct CipherMessage
	{
		/// <summary>The encrypted bytes</summary>
		public byte[] CipherBytes { get; set; }

		/// <summary>the original key bytes</summary>
		public byte[] Key { get; set; }

		/// <summary>the initialization vector</summary>
		public byte[] IV { get; set; }
	}
}
