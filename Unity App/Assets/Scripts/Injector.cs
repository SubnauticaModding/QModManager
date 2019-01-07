using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public static class Injector
{
    public static bool IsInjected(string mainFilename)
    {
        try
        {
            AssemblyDefinition game = AssemblyDefinition.ReadAssembly(mainFilename);

            TypeDefinition type = game.MainModule.GetType("GameInput");
            MethodDefinition method = type.Methods.First(x => x.Name == "Awake");

            foreach (Instruction instruction in method.Body.Instructions)
            {
                if (instruction.OpCode.Equals(OpCodes.Call) && instruction.Operand.ToString().Equals("System.Void QModInstaller.QModPatcher::Patch()"))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}