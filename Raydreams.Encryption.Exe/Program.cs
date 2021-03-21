using System;
using System.IO;
using Raydreams.Encryption.IO;
using Raydreams.Encryption.Security;

namespace Raydreams.Encryption.Exe
{
    /// <summary>Main</summary>
    class Program
    {
        /// <summary>Salt to use on the Key Maker. At least 8 bytes. Should remain the same.</summary>
        private static readonly byte[] salt = new byte[] { 0x21, 0xba, 0xdf, 0xd0, 0x8c, 0x89, 0x97, 0x0c, 0x2b, 0xd0, 0x71, 0x86, 0x4e, 0x32, 0x2e, 0x52 };

        static void Main( string[] args )
        {
            Program app = new Program();
            app.Run();

            Console.WriteLine("Done");
        }

        /// <summary></summary>
        public void Run()
        {
            string path = $"{RayXFile.DesktopPath}/noimage.png";
            string base64 = this.GetBase64File(path);
        }

        /// <summary></summary>
        private void EncryptDecrypt()
        {
            // use a string based Password to generate the actual key
            byte[] key = StrongKeyMaker.Make32BitKey( "Password1", salt, 9999 );

            // path to the test file
            string path = $"{RayXFile.DesktopPath}/PROS.jpeg";

            // encrypt file handler
            RayXFile fe = new RayXFile( key );

            // encrypt the file
            FileInfo ecPath = fe.EncryptFile( path );
            Console.WriteLine( $"File Encrypted to {ecPath}" );

            // decrypt the file
            FileInfo dePath = fe.DecryptFile( ecPath.FullName );
            Console.WriteLine( $"File Decrypted to {dePath}" );
        }

        /// <summary></summary>
        private string GetBase64File(string path)
        {
            if ( String.IsNullOrWhiteSpace( path ) )
                return null;

            // get a handle to the file
            FileInfo fi = new FileInfo( path );

            if ( !fi.Exists )
                return null;

            byte[] data = File.ReadAllBytes( path );

            return Convert.ToBase64String( data, Base64FormattingOptions.None );
        }

    }
}
