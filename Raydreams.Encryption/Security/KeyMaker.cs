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
		/// <returns>A 256 bit key</returns>
        /// <remarks>The salt and iterations must always be the same to get the same results.</remarks>
		public static byte[] Make32BitKey( string pw, byte[] salt, int iterations = 1000 )
		{
			// valiadate
			if ( pw == null )
				pw = String.Empty;

			if ( iterations < 1 )
				iterations = 1;

			byte[] key = Encoding.UTF8.GetBytes( pw );
			Rfc2898DeriveBytes keyGen = new Rfc2898DeriveBytes( key, salt, iterations );

			// size of the key has to match the algorithm
			return keyGen.GetBytes( KeySize );
		}

	}
}
