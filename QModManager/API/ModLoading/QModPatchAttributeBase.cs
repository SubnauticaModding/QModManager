namespace QModManager.API
{
    using System;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using QModManager.Patching;

    /// <summary>
    /// Base class to all attributes that identify QMod patch methods.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class QModPatchAttributeBase : Attribute
    {
        internal PatchingOrder PatchOrder { get; }

        private readonly string _secretPasword;

        /// <summary>
        /// Initializes a new instance of the <see cref="QModPatchAttributeBase"/> class.
        /// </summary>
        /// <param name="patchOrder">The patch order.</param>
        internal QModPatchAttributeBase(PatchingOrder patchOrder)
        {
            this.PatchOrder = patchOrder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QModPatchAttributeBase" /> class.
        /// </summary>
        /// <param name="patchOrder">The patch order.</param>
        /// <param name="secretPasword">The secret pasword.</param>
        /// <exception cref="FatalPatchingException">This modder has not read the documentation and should not be using prepatch/postpatch functions.</exception>
        internal QModPatchAttributeBase(PatchingOrder patchOrder, string secretPasword) : this(patchOrder)
        {
            if (string.IsNullOrEmpty(secretPasword))
                throw new FatalPatchingException("This modder has not read the documentation and should not be using prepatch/postpatch functions.");

            _secretPasword = secretPasword;
        }

        /// <summary>
        /// Validates the that modder has read the documentation.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <exception cref="FatalPatchingException">This modder has not read the documentation and should not be using prepatch/postpatch functions.</exception>
        internal void ValidateThatModderHasReadTheDocumentation(MethodInfo method)
        {
            if (this.PatchOrder == 0)
                return;
            /*
            What is all this for?
            This is to make sure that modders don't start using PrePatch and PostPatch without a second thought.
            Pre/Post patching should be reserved for mods that absolutely require it; Primarily, mods that interact with other mods.
            Mods that act as mod libraries, or that otherwise deal with other mods, are candidates for Pre/Post patching.

            Pre/Post Patching should not be present in in common, standalone mods.
            Pre/Post Patching should absolutely NOT be used to resolve load order conflicts with a single mod: use LoadBefore/LoadAfter instead.

            Now, if you are convinced that you mod should be using these features, then this is what you do.            
            */
            using (var md5 = MD5.Create())
            {
                string c1 = method.Name; // Step 1: Start with the name of your pre or post patching method.
                string c2 = method.DeclaringType.Assembly.GetName().Name; // Step 2: Get the simple name of your assembly.
                string c;

                // Step 3: Check if you are doing a PrePatch or PostPatch
                if (this.PatchOrder < 0)
                {
                    // Step 3a: For a PrePatch, prefix the assembly name with the method name.
                    c = c1 + c2;
                }
                else
                {
                    // Step 3b: For a PostPatch, prefix the method name with the assembly name.
                    c = c2 + c1;
                }

                // Step 4: Compute the MD5 Hash of that string
                // You don't need to write code for this. You can do it online: http://onlinemd5.com/
                byte[] inputBytes = Encoding.ASCII.GetBytes(c);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Step 5: Use the resulting string as the parameter for [QModPrePatch] or [QModPostPatch] depending on which one you built.
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                if (sb.ToString() != _secretPasword)
                    throw new FatalPatchingException("This modder has not read the documentation and should not be using prepatch/postpatch functions.");
            }
        }
    }
}
