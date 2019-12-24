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

        // This extra step is to prevent modders from abusing the new Pre/Post Patching methods
        [QModPrePatch("C75582AC97732BB95F76AD3755EBD0AB")]
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

        // If a modder doesn't want to go through the effort to understand what this is and why,
        // then they shouldn't be using this feature in the first place.
        [QModPostPatch("4F89B345B56898C514E89B65E4CC67DE")]
        public static void QPostPatch()
        {
            Invocations.Add(nameof(QPostPatch));
            PostPatchInvoked = true;
        }
    }
}
