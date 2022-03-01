namespace SMLHelper.V2.Options.Attributes
{
    using Json;
    using System.Collections.Generic;

    internal class ModOptionAttributeMetadata<T> where T : ConfigFile, new()
    {
        public ModOptionAttribute ModOptionAttribute;
        public MemberInfoMetadata<T> MemberInfoMetadata;
        public IEnumerable<MemberInfoMetadata<T>> OnChangeMetadata;
        public IEnumerable<MemberInfoMetadata<T>> OnGameObjectCreatedMetadata;
    }
}
