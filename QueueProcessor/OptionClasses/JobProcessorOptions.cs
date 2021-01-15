using System;
using System.Collections.Generic;
using System.Text;

namespace MTController2.OptionClasses
{
    public class JobProcessorOptions : Options
    {
		private int _someInt;

		public JobProcessorOptions(int someInt)
		{
			_someInt = someInt;
		}

		public int SomeInt
		{
			get { return _someInt; }
			set { _someInt = value; }
		}

		
	}
}
