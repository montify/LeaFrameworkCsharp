using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.Content
{
	public interface IContentLoader
	{
		object Load(string path);
	}
}
