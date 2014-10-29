using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MATHelperTest
{
	[TestClass]
	public class MATLoaderTest
	{
		[TestMethod]
		public void AlwaysPass()
		{
			Assert.AreEqual(true, true);
		}

		[TestMethod]
		public void AlwaysFail()
		{
			Assert.AreEqual(true, false);
		}
	}
}
