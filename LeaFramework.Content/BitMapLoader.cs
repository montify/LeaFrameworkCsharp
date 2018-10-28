using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.Content
{
	public class BitMapLoader : IContentLoader
	{
		public object Load(string path)
		{
			return new Bitmap(path);
		}
	}
}


