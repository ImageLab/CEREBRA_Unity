using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using libsimple;

namespace UnitTests
{
	[TestClass]
	public class libsimpleTest
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
