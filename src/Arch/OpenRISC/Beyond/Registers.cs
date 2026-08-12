using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.Arch.OpenRISC.Beyond;

public static class Registers
{
    public static RegisterStorage[] GpRegs { get; }
    public static RegisterBank All { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        GpRegs = factory.RangeOfReg32(32, "r{0}");

        All = new RegisterBank(factory.NamesToRegisters.Values);
    }

}
