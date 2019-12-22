namespace QMMTests
{
    using System.Collections.Generic;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class TestPatchClass
    {
        internal static bool PrePatchInvoked { get; private set; }
        internal static bool PatchInvoked { get; private set; }
        internal static bool PostPatchInvoked { get; private set; }
        internal static List<string> Invocations { get; } = new List<string>();

        internal static void Reset()
        {
            PrePatchInvoked = false;
            PatchInvoked = false;
            PostPatchInvoked = false;
            Invocations.Clear();
        }

        [QModPrePatch]
        public static void QPrePatch()
        {
            Invocations.Add(nameof(QPrePatch));
            PrePatchInvoked = true;
        }

        [QModPatch]
        public static void QPatch()
        {
            Invocations.Add(nameof(QPatch));
            PatchInvoked = true;
        }

        [QModPostPatch]
        public static void QPostPatch()
        {
            Invocations.Add(nameof(QPostPatch));
            PostPatchInvoked = true;
        }
    }
}
