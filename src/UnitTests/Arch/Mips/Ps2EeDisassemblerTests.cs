#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using NUnit.Framework;
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Mips;

public class Ps2EeDisassemblerTests : DisassemblerTestBase<MipsInstruction>
{
    private readonly MipsArchitecture arch;
    private readonly Address addrBase;

    public Ps2EeDisassemblerTests()
    {
        var options = new Dictionary<string, object>
        {
           { ProcessorOption.InstructionSet, "ps2ee"  }
        };
        this.arch = new MipsLe32Architecture(CreateServiceContainer(), "mips-32-le", options);
        this.addrBase = Address.Ptr32(0x0010_0000);
    }

    public override IProcessorArchitecture Architecture => this.arch;
    public override Address LoadAddress => addrBase;

    private void AssertCode(string expected, string hexBytes)
    {
        var instr = DisassembleHexBytes(hexBytes);
        Assert.That(instr.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void MipsDis_add_s()
    {
        AssertCode("add.s\tf2,f2,f2", "80100246");
    }

    [Test]
    public void MipsDis_cvt_s_w()
    {
        AssertCode("cvt.s.w\tf0,f0", "20008046");
    }

    [Test]
    public void MipsDis_cvt_w_s()
    {
        AssertCode("cvt.w.s\tf2,f0", "A4000046");
    }

    [Test]
    public void MipsDis_di()
    {
        AssertCode("di", "39000042");
    }

    [Test]
    public void MipsDis_div1()
    {
        AssertCode("div1\tr0,r7,r3", "1A00E370");
    }

    [Test]
    public void MipsDis_ei()
    {
        AssertCode("ei", "38000042");
    }

    [Test]
    public void MipsDis_lq()
    {
        AssertCode("lq\tr3,0000(r4)", "00008378");
    }

    [Test]
    public void MipsDis_mfhi1()
    {
        AssertCode("mfhi1\tr2", "10100070");
    }

    [Test]
    public void MipsDis_mf0()
    {
        AssertCode("mf0\tr2", "00E00240");
    }

    [Test]
    public void MipsDis_mflo1()
    {
        AssertCode("mflo1\tr7", "12380070");
    }

    [Test]
    public void MipsDis_mtlo1()
    {
        AssertCode("mtlo1\tr0", "13004070");
    }


    [Test]
    public void MipsDis_mov_s()
    {
        AssertCode("mov.s\tf15,f0", "C6030046");
    }

    [Test]
    public void MipsDis_mul_s()
    {
        AssertCode("mul.s\tf0,f12,f0", "02600046");
    }

    [Test]
    public void MipsDis_multu1()
    {
        AssertCode("multu1\tr0,r3,r5", "19006570");
    }

    [Test]
    public void MipsDis_pand()
    {
        AssertCode("pand\tr2,r2,r3", "89144370");
    }

    [Test]
    public void MipsDis_pcpyh()
    {
        AssertCode("pcpyh\tr3,r8", "E91E0870");
    }

    [Test]
    public void MipsDis_pcpyld()
    {
        AssertCode("pcpyld\tr8,r3,r3", "89436370");
    }

    [Test]
    public void MipsDis_pcpyud()
    {
        AssertCode("pcpyud\tr10,r8,r7", "A9530771");
    }

    [Test]
    public void MipsDis_pnor()
    {
        AssertCode("pnor\tr3,r0,r3", "E91C0370");
    }

    [Test]
    public void MipsDis_psubb()
    {
        AssertCode("psubb\tr2,r3,r8", "48126870");
    }

    [Test]
    public void MipsDis_psubw()
    {
        AssertCode("psubw\tr7,r2,r3", "48384370");
    }

    [Test]
    public void MipsDis_pxor()
    {
        AssertCode("pxor\tr8,r2,r3", "C9444370");
    }

    [Test]
    public void MipsDis_sq()
    {
        AssertCode("sq\tr3,0000(r4)", "0000837C");
    }
}
