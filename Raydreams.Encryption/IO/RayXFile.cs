using System;
using System.IO;
using System.Linq;
using System.Text;
using Raydreams.Encryption.Security;

namespace Raydreams.Encryption.IO
{
    public class RayXFile
    {
        #region [ Fields ]

        /// <summary>Path to the user's desktop folder</summary>
        public static readonly string DesktopPath = Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory );

        /// <summary>The file extension on an encrypted file. You can use whatever you like</summary>
        public static string Extension = "rayx";

        /// <summary>This is the magic number used at the beginning of the file for sniffing.</summary>
        public static byte[] Magic = new byte[] { 0x72, 0x61, 0x79, 0x00 };

        /// <summary>File format version</summary>
        public static byte[] Version = new byte[] { 0x01, 0x00, };

        /// <summary>Delimiter sequnce</summary>
        public static byte[] Delimiter = new byte[] { 0x00, 0x01 };

        #endregion [ Fields ]

        /// <summary></summary>
        public RayXFile(byte[] key)
        {
            // validate the key
            this.Key = key;
        }

        #region [ MyRegion ]

        /// <summary></summary>
        public byte[] Key { get; set; }

        #endregion [ MyRegion ]

        /// <summary></summary>
        public string EncryptFile(string path)
        {
            if ( String.IsNullOrWhiteSpace( path ) )
                return null;

            // get a handle to the file
            FileInfo fi = new FileInfo( path );

            if ( !fi.Exists )
            {
                return null;
            }

            // base file name
            string dir = fi.DirectoryName;
            string name = Path.GetFileNameWithoutExtension( fi.Name );
            string ext = fi.Extension.TrimStart( new char[] { '.' } );

            // 4.2 Gig limit or throws an exception
            byte[] data = File.ReadAllBytes( path );

            // encrypt
            AESEncryptor enc = new AESEncryptor();
            CipherMessage results = enc.Encrypt( data, this.Key );

            // write to file - never overwrite
            string outPath = $"{dir}/{name}.{Extension}";
            using FileStream fs = new FileStream( outPath, FileMode.CreateNew, FileAccess.Write );

            // 4 bytes - write a magic number - which is 'ray' followed by 0
            fs.Write( Magic );

            // 2 bytes - write the file format version which is 1.0
            fs.Write( Version );

            // 16 bytes - first write the IV out which is 16 bytes
            fs.Write( results.IV, 0, results.IV.Length );

            // 2 bytes - write a delimiator which is 01
            fs.Write( Delimiter );

            // write the original extension which is 1+len
            byte[] eb = Encoding.UTF8.GetBytes( ext );
            byte[] ebl = BitConverter.GetBytes( eb.Length );
            fs.WriteByte( ebl[0] );
            fs.Write( eb );

            // 2 bytes - write a delimiator which is 01
            fs.Write( Delimiter );

            // 1 byte - later add a metadata section of optional data but for now its just 1 byte of value 0
            fs.WriteByte( 0x00 );

            // write the encrypted data
            fs.Write( results.CipherBytes, 0, results.CipherBytes.Length );

            // flush and close
            fs.Flush();
            fs.Close();

            return outPath;
        }

        /// <summary></summary>
        /// <returns></returns>
        public string DecryptFile(string path)
        {
            if ( String.IsNullOrWhiteSpace( path ) )
                return null;

            // get a handle to the file
            FileInfo fi = new FileInfo( path );

            if ( !fi.Exists )
            {
                return null;
            }

            // base file name
            string dir = fi.DirectoryName;
            string name = Path.GetFileNameWithoutExtension( fi.Name );
            string fext = fi.Extension.TrimStart( new char[] { '.' } );

            // make sure valid file type
            if ( !fext.Equals( Extension, StringComparison.InvariantCultureIgnoreCase ) )
                return null;

            // write to file - never overwrite
            using FileStream fs = new FileStream( path, FileMode.Open, FileAccess.Read );

            // 4 bytes - write a magic number - which is 'ray' followed by 0
            byte[] magic = new byte[Magic.Length];
            fs.Read( magic );

            if ( !this.ArraysEqual(magic, Magic) )
                return null;

            // 2 bytes - write the file format version which is 1.0
            byte[] ver = new byte[Version.Length];
            fs.Read( ver );

            // 16 bytes - first write the IV out which is 16 bytes
            byte[] iv = new byte[16];
            fs.Read( iv );

            // 2 bytes - read a delimiator which is 01
            byte[] delim = new byte[Delimiter.Length];
            fs.Read( delim );

            // 1 byte - the length of the extension string
            byte[] ebl = new byte[1];
            fs.Read( ebl );
            int el = Convert.ToInt32( ebl[0] );

            // read N bytes the original extension
            byte[] eb = new byte[el];
            fs.Read( eb );
            string ext = Encoding.UTF8.GetString( eb );

            // 2 bytes - read a delimiator which is 01
            fs.Read( delim );

            // 1 byte for the metadata size
            byte[] mdl = new byte[1];
            fs.Read( mdl );

            // finally get the data itself
            int offset = 28 + el;
            byte[] data = new byte[fs.Length - offset];
            fs.Read( data );

            // decrypt
            AESEncryptor enc = new AESEncryptor();
            byte[] file = enc.Decrypt( data, this.Key, iv );

            string outPath = $"{dir}/{name}-copy.{ext}";
            File.WriteAllBytes( outPath, file );

            fs.Close();

            return outPath;
        }

        /// <summary>Test 2 byte arrays are exactly the same</summary>
        private bool ArraysEqual(byte[] b1, byte[] b2)
        {
            if ( b1 == null && b2 != null )
                return false;

            if ( b2 == null && b1 != null )
                return false;

            if ( b1.Length != b2.Length )
                return false;

            return b1.SequenceEqual( b2 );
        }

    }
}
