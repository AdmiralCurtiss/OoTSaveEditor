using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Other.N64.OoTSaveEditor {
	class Program {
		[STAThread]
		public static int Main( string[] args ) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new OoTSaveEditForm() );
			return 0;
		}
	}
}
