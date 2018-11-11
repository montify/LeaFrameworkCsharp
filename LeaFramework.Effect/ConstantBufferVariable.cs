// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

namespace LeaFramework.Effect
{

	public struct ConstantBufferVariable
	{
		public string Name;
		public int Size;
		public int Offset;
		

		public ConstantBufferVariable(string name, int size, int offset)
		{
			Name = name;
			Offset = offset;

			Size = 0;
			Size = CalculateAlignement(size, 16);
		
		}

		// return true when the size is divided with N = 0
		private bool IsAlign(int size, int N)
		{
			return size % N == 0;
		}

		// Determine the next number who is divided by 0 with the alignement and return it
		private int CalculateAlignement(int value, int alignement)
		{
			if (IsAlign(value, alignement))
				return value;
			
			int retVal = value;

			while (true)
			{
				if (IsAlign(retVal, alignement))
					break;

				retVal++;
			}

			return retVal;
		}
	}
}
