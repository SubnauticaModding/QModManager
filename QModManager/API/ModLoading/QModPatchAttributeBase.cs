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
        /// <param name="mod">The mod.</param>
        /// <exception cref="FatalPatchingException">This modder has not read the documentation and should not be using prepatch/postpatch functions.</exception>
        internal void ValidateSecretPassword(MethodInfo method, QMod mod)
        {
            switch (this.PatchOrder)
            {
                case PatchingOrder.PreInitialize:
                case PatchingOrder.NormalInitialize:
                case PatchingOrder.PostInitialize:
                    return;
            }

            /*
            What is all this for?
            This is to make sure that modders don't start using elevated PrePatching and PostPatching in place of normal Pre/Post Patching without a second thought.
            Elevated Pre/Post patching should be reserved for mods that absolutely require it; Primarily, mods that interact with all other mods.
            Mods that only interact with one or two mods can likely do just fine with basic load order.

            Now, if you are convinced that your mod should be using these features, then this is what you do.
            */
            using (var md5 = MD5.Create())
            {
                string c1 = method.Name; // Step 1: Start with the name of your pre or post patching method.
                string c2 = mod.Id; // Step 2: Get the ID of your mod
                string c;

                // Step 3: Check if you are doing a PrePatch or PostPatch
                if (this.PatchOrder < 0)
                {
                    // Step 3 Option A: For a PrePatch, create a string by placing the prepatch method name before the assembly name.
                    c = c1 + c2;
                }
                else
                {
                    // Step 3 Option B: For a PostPatch, create a string by placing the postpatch method name after the assembly name.
                    c = c2 + c1;
                }

                // Step 4: Compute the MD5 Hash of that string
                // You don't need to write code for this. You can do it online: http://onlinemd5.com/
                byte[] inputBytes = Encoding.ASCII.GetBytes(c);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Step 5: Use the resulting string as the parameter for [QModPrePatch()] or [QModPostPatch()] depending on which one you built.
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
