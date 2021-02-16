using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

namespace AssetUsageFinder {
	static class FLAGS {
		public const string DEBUG = "DEBUG1"; //todo rename in release
	}

	static class Asr {
#line hidden
		[Conditional(FLAGS.DEBUG)]
		public static void AreEqual(int a, int b) {
			Assert.AreEqual(a, b);
		}

		[Conditional(FLAGS.DEBUG)]
		public static void IsTrue(bool b, string format = null) {
			Assert.IsTrue(b, format);
		}

		[Conditional(FLAGS.DEBUG)]
		public static void IsFalse(bool b, string format = null) {
			Assert.IsFalse(b, format);
		}

		[Conditional(FLAGS.DEBUG)]
		public static void Fail(string format = null) {
			throw new AssertionException("Failed", format);
		}

		[Conditional(FLAGS.DEBUG)]
		public static void IsNotNull(Object target, string str) {
			Assert.IsNotNull(target, str);
		}
#line default
	}
}