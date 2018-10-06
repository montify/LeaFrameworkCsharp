
using LeaFramework.PlayGround;

namespace PlayGround
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var g = new Game01())
			{
				g.Run();
			}
		}
	}
}
