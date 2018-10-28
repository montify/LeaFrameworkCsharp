using LeaFramework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.Content
{
	public class ContentFactory
	{
		public static IContentLoader GetContentLoader(Type type)
		{
			IContentLoader contentLoader = null;

			if (type == typeof(LeaTexture2D))
				contentLoader = new BitMapLoader();
			else
				throw new Exception("NOT SUPPORTET");

			return contentLoader;
		}
	}
}
