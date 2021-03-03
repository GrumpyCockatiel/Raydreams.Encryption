using System;
using System.Security.Cryptography;
using System.Text;

namespace Raydreams.Encryption.Security
{
	/// <summary>Define the Key Maker as a delegate for different strengths</summary>
    /// <param name="pw"></param>
    /// <param name="salt"></param>
    /// <param name="iterations"></param>
    /// <returns></returns>
	public delegate byte[] MakeKey( string pw, byte[] salt, int iterations);

	/// <summary>A class that generates a 256 bit key from a text phrase</summary>
	public static class StrongKeyMaker
	{
		/// <summary>The physical key size.</summary>
		/// <remarks>While you can use 16 or 24 bits, we really want the strongest key.</remarks>
		public const int KeySize = 32;

		/// <summary>Generates a symmetric key to use from a string password</summary>
        /// <param name="pw">The plain text phrase to derive a key from. Spaces are NOT trimmed.</param>
        /// <param name="salt">The salt to use in the algorithm</param>
        /// <param name="iterations">How many interations. 1000 is the default.</param>
		/// <returns>A 256 bit key</returns>
        /// <remarks>The salt and iterations must always be the same to get the same results.</remarks>
		public static byte[] Make32BitKey( string pw, byte[] salt, int iterations = 1001 )
		{
			if ( salt == null || salt.Length < 8 )
				throw new System.ArgumentException( nameof( salt ), "You need some better salt." );

			// valiadate
			if ( pw == null )
				pw = String.Empty;

			if ( iterations < 1000 )
				iterations = 1000;

			byte[] key = Encoding.UTF8.GetBytes( pw );
			Rfc2898DeriveBytes keyGen = new Rfc2898DeriveBytes( key, salt, iterations );

			// size of the key has to match the algorithm
			return keyGen.GetBytes( KeySize );
		}

	}
}
