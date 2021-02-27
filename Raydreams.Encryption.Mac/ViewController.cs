using System;
using System.IO;
using AppKit;
using Foundation;
using Raydreams.Encryption.IO;
using Raydreams.Encryption.Security;

namespace Raydreams.Encryption.Mac
{
    public partial class ViewController : NSViewController
    {
        public ViewController( IntPtr handle ) : base( handle )
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

		[Action( "openDocument:" )]
		public bool OpenFolder( NSMenuItem item )
		{
			NSOpenPanel dialog = new NSOpenPanel();
			dialog.Title = "Select File";
			dialog.CanChooseFiles = true;
			dialog.CanCreateDirectories = false;
			dialog.CanChooseDirectories = false;
			dialog.AllowsMultipleSelection = false;

			nint results = dialog.RunModal();

			Console.WriteLine( results );

			if ( results == 1 )
			{
				NSError err = null;
				//NSUrl[] dir = NSFileManager.DefaultManager.GetDirectoryContent( dialog.Url, null,
				//	NSDirectoryEnumerationOptions.SkipsHiddenFiles | NSDirectoryEnumerationOptions.SkipsSubdirectoryDescendants,
				//	out err );

				if ( dialog.Url.PathExtension.Equals( RayXFile.Extension, StringComparison.CurrentCultureIgnoreCase ) )
				{
					if ( !RayXFile.Sniff( dialog.Url.Path ) )
						return false;

					// use a string based Password to generate the actual key
					byte[] key = StrongKeyMaker.Make32BitKey( "Password1", AppDelegate.Salt );

					RayXFile fe = new RayXFile( key );
					FileInfo fi = fe.DecryptFile( dialog.Url.Path );
                }

				// only process the .xls, .csv, .txt or .xlsx files

				// field of the file being processed
				//this.fileField.StringValue = dialog.Url.AbsoluteString;

				// check the extension on the file to determine what to do
			}

			return true;
		}
	}
}
