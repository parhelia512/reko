using Reko.Arch.Arm.AArch32;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Reko.Environments.RiscOS;

// https://paolozaino.wordpress.com/2020/08/07/risc-os-introduction-to-the-arm-aif-object-file-format/
// https://www.riscosopen.org/wiki/documentation/show/OS_Exit

public class AifLoader : ProgramImageLoader
{
    public AifLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) 
        : base(services, imageLocation, imgRaw)
    {
        this.PreferredBaseAddress = Address.Ptr32(0x8000);
    }

    public override Address PreferredBaseAddress { get; set; }
    public override Program LoadProgram(Address? address, string? platformOverride)
    {
        var cfgSvc = Services.RequireService<IConfigurationService>();
        var arch = cfgSvc.GetArchitecture("arm");
        Debug.Assert(arch is not null);
        var platform = cfgSvc.GetEnvironment(platformOverride ?? "riscOS")
            .Load(Services, arch);

        var rdr = new LeImageReader(new ByteMemoryArea(PreferredBaseAddress, RawImage), 0);
        var instrs = arch.CreateDisassembler(rdr).Take(5).ToArray();
        if (instrs.Length < 5)
            throw new BadImageFormatException("Unable to load AIF header.");
        var Decompress = instrs[0];           // Jump to decompression code section OR No Operation if the AIF is not compressed.
        var SelfRelocCode = instrs[1];        // Jump to subroutine for self relocation OR No Operation if the image is not self - relocating
        var ZeroInitCodeInstr = instrs[2];    // Jump to ZeroInit code subroutine OR No Operation if the image has none
        var ImageEntryPoint = instrs[3];      // Jump to EntryPoint for Executable AIF OR EntryPoint offset for Non - Executable AIF.BL is used to make the header addressable via R14(ARM32 Link Register) in a position independent to ensure the header is position - independent
        var ProgramExit = instrs[4];          // Instructions to exit the program as last attempt, in RISC OS this is an OS_Exit SWI

        if (!rdr.TryReadUInt32(out var ImageReadOnlySize) ||    // Size of the ReadOnly section, it includes the size of the Header only in the case the AIF is Executable
            !rdr.TryReadUInt32(out var ImageReadWriteSize) ||   // Exact size of the ReadWrite section in multiple of 4 bytes
            !rdr.TryReadUInt32(out var ImageDebugSize) ||       // Exact size of the Debug section in multiple of 4 bytes.Includes high and low level debug size.Bits 0 - 3 hold the type, bits 4 - 31 hold the low - level debug size
            !rdr.TryReadUInt32(out var ImageZeroInitSize) ||    // Exact size of the ZeroInit section in multiple of 4 bytes
            !rdr.TryReadUInt32(out var ImageDebugType) ||       // Valid values are 0 = No debugging data present,1 = Low - level debugging data present,2 = Src - Level debugging data present,3 = 1 and 2 present
            !rdr.TryReadUInt32(out var ImageBase) ||            // Address where the code was linked
            !rdr.TryReadUInt32(out var WorkSpace) ||            // Work Space  this was obsoleted in the ’90s
            !rdr.TryReadUInt32(out var AddressMode) ||          // Address mode    this word contains either 0, 26 or 32 in its last significant byte to indicates if the binary image is linked for 26bit, 32bit or, if it’s 0 then that indicate the binary is in an old 26bit header
            !rdr.TryReadUInt32(out var DataBase) ||             // Data base   address where the image data was linked
            !rdr.TryReadUInt32(out var Reserved1) ||            // Two reserved words This is for Extended AIF
            !rdr.TryReadUInt32(out var Reserved2) ||            // Two reserved words This is for Extended AIF
            !rdr.TryReadUInt32(out var DBGInit) ||              // DBGInit | NOP   Debug Initialisation Instruction OR No Operation if DBGInit is unused
            !rdr.TryReadUInt32(out var ZeroInitCode))           // ZeroInit code 15 words
            throw new BadImageFormatException("Unable to read AIF header.");

        var segments = new SegmentMap(
            new ImageSegment(
                "code",
                new ByteMemoryArea(PreferredBaseAddress, RawImage),
                AccessMode.ReadExecute));
        var memory = new ProgramMemory(segments);
        var program = new Program(memory, arch, platform);
        AddSymbol(arch, program, ImageEntryPoint, "_entry");
        AddSymbol(arch, program, ZeroInitCodeInstr, "_zero_init");
        return program;
    }

    private static void AddSymbol(
        IProcessorArchitecture arch,
        Program program, 
        MachineInstruction instr,
        string name)
    {
        if (instr.MnemonicAsInteger == (int) Mnemonic.bl &&
            instr.Operands[0] is Address e)
        {
            var entry = ImageSymbol.Procedure(arch, e, name);
            program.EntryPoints.Add(entry.Address, entry);
        }
    }
}
