using Reko.Core;
using System.Collections.Generic;

namespace Reko.Arch.OpenRISC.Beyond;

public static class Registers
{
    public static RegisterStorage[] GpRegs { get; }
    public static Dictionary<string, RegisterStorage> ByName { get; }
    public static Dictionary<StorageDomain, RegisterStorage> ByDomain { get; }

    static Registers()
    {
        var factory = new StorageFactory();
        GpRegs = factory.RangeOfReg32(32, "r{0}");

        ByName = factory.NamesToRegisters;
        ByDomain = factory.DomainsToRegisters;
    }

}
