﻿using System;
using System.Diagnostics.CodeAnalysis;
using Cpp2IL.Core.Exceptions;
using HarmonyLib;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cpp2IL.Core
{
    /// <summary>
    /// Yes, I'm harmony-patching Cecil, instead of putting in more useful error messages upstream.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class Cpp2IlHarmonyPatches
    {
        internal static void Install()
        {
            Logger.InfoNewline("Patching Cecil for better error messages...", "Harmony");
            
            Logger.VerboseNewline("\tInitializing harmony instance 'dev.samboy.cpp2il'...", "Harmony");
            var harmony = new Harmony("dev.samboy.cpp2il");
            
            Logger.VerboseNewline("\tAdding finalizer to Mono.Cecil.Cil.CodeWriter:WriteMethodBody...", "Harmony");
            harmony.Patch(AccessTools.Method("Mono.Cecil.Cil.CodeWriter:WriteMethodBody"), finalizer: new(typeof(Cpp2IlHarmonyPatches), nameof(FinalizeWriteMethodBody)));
            
            Logger.VerboseNewline("\tAdding finalizer to Mono.Cecil.Cil.CodeWriter:WriteOperand...", "Harmony");
            harmony.Patch(AccessTools.Method("Mono.Cecil.Cil.CodeWriter:WriteOperand"), finalizer: new(typeof(Cpp2IlHarmonyPatches), nameof(FinalizeWriteOperand)));
            
            Logger.VerboseNewline("\tDone", "Harmony");
        }

        public static Exception? FinalizeWriteMethodBody(MethodDefinition method, Exception? __exception)
        {
            if(__exception != null)
                return new MethodWriteFailedException(method, __exception);

            return null;
        }
        
        public static Exception? FinalizeWriteOperand(Instruction instruction, Exception? __exception)
        {
            if(__exception != null)
                return new InstructionWriteFailedException(instruction, __exception);

            return null;
        }
    }
}