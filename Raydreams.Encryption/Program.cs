using System;
using Raydreams.Encryption.IO;
using Raydreams.Encryption.Security;

namespace Raydreams.Encryption
{
    /// <summary></summary>
    class Program
    {
        /// <summary>Salt to use on the Key Maker. At least 8 bytes. Should remain the same.</summary>
        private static readonly byte[] salt = new byte[] { 0x21, 0xba, 0xdf, 0xd0, 0x8c, 0x89, 0x97, 0x0c, 0x2b, 0xd0, 0x71, 0x86, 0x4e, 0x32, 0x2e, 0x52 };

        static void Main( string[] args )
        {
            // use a string based Password to generate the actual key
            byte[] key = KeyMaker.MakeKey( "Password1", salt );

            // path to the test file
            string path = $"{RayXFile.DesktopPath}/PROS.jpeg";

            // encrypt file handler
            RayXFile fe = new RayXFile(key);

            // encrypt the file
            string ecPath = fe.EncryptFile( path );
            Console.WriteLine( $"File Encrypted to {ecPath}" );

            // decrypt the file
            string dePath = fe.DecryptFile( ecPath );
            Console.WriteLine( $"File Decrypted to {dePath}" );
        }
    }
}
