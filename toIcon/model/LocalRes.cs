using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toIcon.model {
	public class LocalRes {
		public static string rootPath() {
			return "/httpServer;component/resource/";
		}

		public static string icon16() {
			return "pack://application:,,,/toIcon;component/resource/icon16.png";
			//return rootPath() + "icon16.png";
		}
	}
}
