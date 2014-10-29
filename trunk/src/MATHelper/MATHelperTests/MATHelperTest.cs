using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MATHelper;

namespace MATHelperTest
{
	[TestClass]
	public class MATHelperTest
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
