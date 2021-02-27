using System.Globalization;
using AppKit;
using Foundation;

namespace Raydreams.Encryption.Mac
{
    [Register( "AppDelegate" )]
    public class AppDelegate : NSApplicationDelegate
    {
        /// <summary>Salt to use on the Key Maker. At least 8 bytes. Should remain the same.</summary>
        public static readonly byte[] Salt = new byte[] { 0x21, 0xba, 0xdf, 0xd0, 0x8c, 0x89, 0x97, 0x0c, 0x2b, 0xd0, 0x71, 0x86, 0x4e, 0x32, 0x2e, 0x52 };

        public AppDelegate()
        {
            CultureInfo.CurrentCulture = new CultureInfo( "en-US", false );
            CultureInfo.CurrentUICulture = new CultureInfo( "en-US", false );
        }

        public override void DidFinishLaunching( NSNotification notification )
        {
            // Insert code here to initialize your application
        }

        public override void WillTerminate( NSNotification notification )
        {
            // Insert code here to tear down your application
        }
    }
}
