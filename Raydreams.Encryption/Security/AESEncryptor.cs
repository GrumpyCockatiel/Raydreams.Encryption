
using System;
using System.IO;
using System.Security.Cryptography;

namespace Raydreams.Encryption.Security
{
	/// <summary>Wraps the AES Symmetric Encryption</summary>
	/// <remarks>To replace SymmetricEncryptor because I only care about AES</remarks>
	public class AESEncryptor
    {
		#region [ Fields ]

		/// <summary>Algorithm being used</summary>
		private SymmetricAlgorithm _algo = null;

		#endregion [ Fields ]

		#region [ Constructors ]

		/// <summary>Constructor</summary>
		public AESEncryptor()
		{
			this._algo = new RijndaelManaged();
			this.Algorithm.Mode = CipherMode.CBC;
			// these are all defaults
			//this.Algorithm.KeySize = 128;
			//this.Algorithm.BlockSize = 128;
			//this.Algorithm.FeedbackSize = 128;
			this.Algorithm.Padding = PaddingMode.PKCS7;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>The encryptor</summary>
		protected SymmetricAlgorithm Algorithm
		{
			get { return this._algo; }
		}

		/// <summary></summary>
		public KeySizes[] KeySizes
		{
			get { return this._algo.LegalKeySizes; }
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary>Encrypt JUST plain bytes with some key</summary>
		/// <param name="data"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public CipherMessage Encrypt( byte[] data, byte[] key )
		{
			// validate arguments
			if ( data == null || data.Length < 1 )
				throw new System.ArgumentNullException( nameof( data ) );

			// need better validation of the key size
			if ( key == null || key.Length < 1 )
				throw new System.ArgumentNullException( nameof( key ) );

			// setup the algorithm
			this.Algorithm.GenerateIV();
			this.Algorithm.Key = key;

			CipherMessage results = new CipherMessage()
			{
				IV = this.Algorithm.IV,
			};

			try
			{
				// create an encryptor
				ICryptoTransform encryptor = this.Algorithm.CreateEncryptor( this.Algorithm.Key, this.Algorithm.IV );

				// decrypt
				results.CipherBytes = this.DoCrypto( data, encryptor );
			}
			catch ( System.Exception )
			{
                throw;
            }

			return results;
		}

		/// <summary>Decrypt cipter text using a symmetric key algorithm.</summary>
		/// <param name="cipherText">The encrypted message as a byte array.</param>
		/// <param name="key">The key used in the original encryption.</param>
		/// <param name="iv">The init vector used in the original encryption.</param>
		/// <returns></returns>
		public byte[] Decrypt( byte[] data, byte[] key, byte[] iv )
		{
			// validate input
			if ( data == null || data.Length < 1 )
				throw new ArgumentNullException( nameof( data ) );

			if ( key == null || key.Length <= 0 )
				throw new ArgumentNullException( nameof( key ) );

			if ( iv == null || iv.Length <= 0 )
				throw new ArgumentNullException( nameof( iv ) );

			// decrypted results
			byte[] results = null;

			this.Algorithm.Key = key;
			this.Algorithm.IV = iv;

			try
			{
				// create an decryptor
				ICryptoTransform decryptor = this.Algorithm.CreateDecryptor( this.Algorithm.Key, this.Algorithm.IV );

				// decrypt
				results = this.DoCrypto( data, decryptor );
			}
			catch ( System.Exception )
			{
				throw;
			}

			return results;
		}

		/// <summary>zero out memory</summary>
		public void Clear()
		{
			// Clear the RijndaelManaged object.
			if ( this.Algorithm != null )
				this.Algorithm.Clear();
		}

		/// <summary>Does the actual encrypt or decrypt</summary>
        /// <param name="data"></param>
        /// <param name="cryptoTransform"></param>
        /// <returns></returns>
		private byte[] DoCrypto( byte[] data, ICryptoTransform cryptor )
		{
			using ( var ms = new MemoryStream() )
			using ( var cs = new CryptoStream( ms, cryptor, CryptoStreamMode.Write ) )
			{
				cs.Write( data, 0, data.Length );
				cs.FlushFinalBlock();

				return ms.ToArray();
			}
		}

		#endregion [ Methods ]

	}
}
