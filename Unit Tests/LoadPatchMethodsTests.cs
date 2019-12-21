namespace QMMTests
{
    using System.Reflection;
    using NUnit.Framework;
    using QModManager.Patching;

    using QModManager.API.ModLoading;

    [QModCore]
    public static class TestPatchClass
    {
        [QModPrePatch]
        public static void QPrePatch()
        {
        }

        [QModPatch]
        public static void QPatch()
        {
        }

        [QModPostPatch]
        public static void QPostPatch()
        {
        }
    }

    [TestFixture]
    public class LoadPatchMethodsTests
    {
        [Test]
        public void FetchPatchMethods_AttributesPresent_GetExpectedPatchMethod()
        {
            var qmod = new QModJson
            {
                LoadedAssembly = Assembly.GetExecutingAssembly()
            };

            var methodFinder = new PatchMethodFinder();

            methodFinder.LoadPatchMethods(qmod);

            Assert.AreEqual(3, qmod.PatchMethods.Count);
            Assert.AreEqual("QPrePatch", qmod.PatchMethods[PatchingOrder.PreInitialize].Method.Name);
            Assert.AreEqual("QPatch", qmod.PatchMethods[PatchingOrder.NormalInitialize].Method.Name);
            Assert.AreEqual("QPostPatch", qmod.PatchMethods[PatchingOrder.PostInitialize].Method.Name);
        }
    }
}
