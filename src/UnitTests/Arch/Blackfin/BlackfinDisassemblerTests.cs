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
using Reko.Arch.Blackfin;
using Reko.Core;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Blackfin;

[TestFixture]
public class BlackfinDisassemblerTests : DisassemblerTestBase<BlackfinInstruction>
{
    private readonly BlackfinArchitecture arch;

    public BlackfinDisassemblerTests()
    {
        this.arch = new BlackfinArchitecture(new ServiceContainer(), "blackfin", []);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress { get; } = Address.Ptr32(0x00100000);

    private void AssertCode(string sExpected, string hexBytes)
    {
        var instr = DisassembleHexBytes(hexBytes);
        Assert.AreEqual(sExpected, instr.ToString());
    }

    [Test]
    public void BlackfinDasm_add_shift()
    {
        AssertCode("R3 = R3 + R0 << 1;", "0341");
    }

    [Test]
    public void BlackfinDasm_add_sh2_p()
    {
        AssertCode("P1 = P1 + P2 << 2;", "D145");
    }


    [Test]
    public void BlackfinDasm_and3()
    {
        AssertCode("R1 = R1 & R2;", "5154");
    }

    [Test]
    public void BlackfinDasm_asr3_long()
    {
        var instr = DisassembleHexBytes("82C69203");
        Assert.AreEqual("R1 = R2 >>> 0E;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_Jump_indirect()
    {
        AssertCode("JUMP [P3];", "5300");
    }

    [Test]
    public void BlackfinDasm_Jump_pc_indexed()
    {
        AssertCode("JUMP [PC + P4];", "8400");
    }

    [Test]
    public void BlackfinDasm_jump_s()
    {
        AssertCode("JUMP.S 000FFFFE;", "FF2F");
    }

    [Test]
    public void BlackfinDasm_load_lo_imm()
    {
        AssertCode("SP.L = 0FEC;", "0EE1EC0F ");
    }

    [Test]
    public void BlackfinDasm_load_hi_imm()
    {
        AssertCode("SP.H = 0FEC;", "4EE1EC0F ");
    }

    [Test]
    public void BlackfinDasm_load_x_imm()
    {
        AssertCode("SP = 0FEC (X);", "2EE1EC0F ");
    }

    [Test]
    public void BlackfinDasm_lsl_imm()
    {
        AssertCode("P4 = P1 << 02;", "4C44");
    }

    [Test]
    public void BlackfinDasm_shl_2()
    {
        AssertCode("P0 = P5 << 02;", "6844");
    }

    [Test]
    public void BlackfinDasm_store_mem_off()
    {
        AssertCode("[P4 + 0x17000] = R1;", "21E6005C");
    }

    [Test]
    public void BlackfinDasm_st_p2_r0()
    {
        AssertCode("W[P2] = R0;", "1097");
    }

    [Test]
    public void BlackfinDasm_RTS()
    {
        AssertCode("RTS;", "1000");
    }

    [Test]
    public void BlackfinDasm_RTI()
    {
        AssertCode("RTI;", "1100");
    }

    [Test]
    public void BlackfinDasm_RTX()
    {
        AssertCode("RTX;", "1200");
    }

    [Test]
    public void BlackfinDasm_RTN()
    {
        AssertCode("RTN;", "1300");
    }

    [Test]
    public void BlackfinDasm_mov_dreg_negimm()
    {
        AssertCode("R0 = FFFFFFFF;", "F863");
    }

    [Test]
    public void BlackfinDasm_push_i3()
    {
        AssertCode("[--SP] = I3;", "5301");
    }

    [Test]
    public void BlackfinDasm_push_a0_w()
    {
        AssertCode("[--SP] = A0.W;", "6101");
    }

    [Test]
    public void BlackfinDasm_push_lt0()
    {
        AssertCode("[--SP] = LT0;", "7101");
    }

    [Test]
    public void BlackfinDasm_iflush()
    {
        AssertCode("iflush [P1];", "5902");
    }

    [Test]
    public void BlackfinDasm_flushinv_post()
    {
        AssertCode("flushinv [P0++];", "6802");
    }

    [Test]
    public void BlackfinDasm_flush_postinc()
    {
        AssertCode("flush [P0++];", "7002");
    }

    [Test]
    public void BlackfinDasm_mov_cc_le_d_d()
    {
        AssertCode("CC = R0 <= R7;", "070A");
    }

    [Test]
    public void BlackfinDasm_mov_cc_eq_d_imm()
    {
        AssertCode("CC = R0 == 00000000;", "000C");
    }

    [Test]
    public void BlackfinDasm_store_uimm()
    {
        AssertCode("[P5 + 0x0004] = R6;", "6EB0");
    }

    [Test]
    public void BlackfinDasm_bitclr()
    {
        AssertCode("BITCLR(R6,00);", "064C");
    }

    [Test]
    public void BlackfinDasm_pop_r0()
    {
        AssertCode("R0 = [SP++];", "3090");
    }

    [Test]
    public void BlackfinDasm_pop_a1_w()
    {
        AssertCode("A1.W = [SP++];", "2301");
    }

    [Test]
    public void BlackfinDasm_push_rete()
    {
        var instr = DisassembleHexBytes("7E01");
        Assert.AreEqual("[--SP] = RETE;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_csync()
    {
        var instr = DisassembleHexBytes("2300");
        Assert.AreEqual("CSYNC;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_divq()
    {
        var instr = DisassembleHexBytes("0842");
        Assert.AreEqual("DIVQ ( R0,R1 );", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ssync()
    {
        var instr = DisassembleHexBytes("2400");
        Assert.AreEqual("SSYNC;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_load_dreg_shortoffset()
    {
        var instr = DisassembleHexBytes("A0A0");
        Assert.AreEqual("R0 = [P4 + 0x0008];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_sti()
    {
        var instr = DisassembleHexBytes("4000");
        Assert.AreEqual("STI R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_nop()
    {
        var instr = DisassembleHexBytes("0000");
        Assert.AreEqual("NOP;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_call()
    {
        var instr = DisassembleHexBytes("00E37805");
        Assert.AreEqual("CALL 00100AF0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_pop_b1()
    {
        var instr = DisassembleHexBytes("1901");
        Assert.AreEqual("B1 = [SP++];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_if_ncc_jump()
    {
        var instr = DisassembleHexBytes("0310");
        Assert.AreEqual("IF !CC JUMP 00100006;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_iflush_postinc()
    {
        var instr = DisassembleHexBytes("7802");
        Assert.AreEqual("iflush [P0++];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_if_ncc_mov_d_p()
    {
        var instr = DisassembleHexBytes("5206");
        Assert.AreEqual("IF !CC R2 = P2;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_if_ncc_d_d()
    {
        var instr = DisassembleHexBytes("0807");
        Assert.AreEqual("IF !CC R1 = R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_if_ncc_p_p()
    {
        var instr = DisassembleHexBytes("F407");
        Assert.AreEqual("IF !CC SP = P4;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_bitset()
    {
        var instr = DisassembleHexBytes("C04A");
        Assert.AreEqual("BITSET(R0,18);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_if_n_cc_mov_p_p()
    {
        var instr = DisassembleHexBytes("F907");
        Assert.AreEqual("IF !CC FP = P1;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_st_p()
    {
        var instr = DisassembleHexBytes("ECBD");
        Assert.AreEqual("[P5 + 0x001C] = P4;", instr.ToString());
    }


    [Test]
    public void BlackfinDasm_pop_regs()
    {
        var instr = DisassembleHexBytes("2805");
        Assert.AreEqual("(FP:P0) = [SP++];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_push_regs()
    {
        var instr = DisassembleHexBytes("EC05");
        Assert.AreEqual("[--SP] = (FP:P4);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_cli()
    {
        var instr = DisassembleHexBytes("3000");
        Assert.AreEqual("CLI R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_not()
    {
        var instr = DisassembleHexBytes("C143");
        Assert.AreEqual("R1 = ~R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_asr_dreg()
    {
        var instr = DisassembleHexBytes("1340");
        Assert.AreEqual("R3 >>>= R2;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mul_dreg()
    {
        var instr = DisassembleHexBytes("C740");
        Assert.AreEqual("R7 *= R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_lsr()
    {
        var instr = DisassembleHexBytes("884E");
        Assert.AreEqual("R0 >>= 11;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_add3()
    {
        var instr = DisassembleHexBytes("C751");
        Assert.AreEqual("R7 = R7 + R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_add3_long()
    {
        AssertCode("R2.H = R2.L + R2.H;", "22C41144");
    }

    [Test]
    public void BlackfinDasm_mul()
    {
        var instr = DisassembleHexBytes("F740");
        Assert.AreEqual("R7 *= R6;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mov_cc_lt_d_d()
    {
        var instr = DisassembleHexBytes("9009");
        Assert.AreEqual("CC = R2 < R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mov_n_bittest()
    {
        var instr = DisassembleHexBytes("0848");
        Assert.AreEqual("CC = !BITTEST(R0,01);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mov_cc_eq()
    {
        var instr = DisassembleHexBytes("0802");
        Assert.AreEqual("CC = R0 == R1;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_store_indirect()
    {
        var instr = DisassembleHexBytes("1293");
        Assert.AreEqual("[P2] = R2;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_sub3()
    {
        var instr = DisassembleHexBytes("5152");
        Assert.AreEqual("R1 = R1 - R2;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_add_imm()
    {
        var instr = DisassembleHexBytes("6764");
        Assert.AreEqual("R7 += 0000000C;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_add_simm()
    {
        var instr = DisassembleHexBytes("A064");
        Assert.AreEqual("R0 += 00000014;", instr.ToString());
    }



    [Test]
    public void BlackfinDasm_mov_reg_simm()
    {
        var instr = DisassembleHexBytes("0960");
        Assert.AreEqual("R1 = 00000001;", instr.ToString());
    }




    [Test]
    public void BlackfinDasm_load_z_byte()
    {
        var instr = DisassembleHexBytes("2899");
        Assert.AreEqual("R0 = B[P5] (Z);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_if_cc_jump_bp()
    {
        var instr = DisassembleHexBytes("021C");
        Assert.AreEqual("IF CC JUMP 00100004 (BP);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mov_emudat()
    {
        var instr = DisassembleHexBytes("3D3E");
        Assert.AreEqual("EMUDAT = R5;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mov_x_byte_postinc()
    {
        var instr = DisassembleHexBytes("5098");
        Assert.AreEqual("R0 = B[P2++] (X);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mov_cc_eq_d_d()
    {
        var instr = DisassembleHexBytes("1808");
        Assert.AreEqual("CC = R3 == R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_store_byte_postinc()
    {
        var instr = DisassembleHexBytes("109A");
        Assert.AreEqual("B[P2++] = R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_if_ncc_jump_bp()
    {
        var instr = DisassembleHexBytes("0214");
        Assert.AreEqual("IF !CC JUMP 00100004 (BP);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_if_cc_jump()
    {
        var instr = DisassembleHexBytes("F61F");
        Assert.AreEqual("IF CC JUMP 000FFFEC (BP);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mov_b_z()
    {
        var instr = DisassembleHexBytes("4043");
        Assert.AreEqual("R0 = R0.B (Z);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_dlo_idx()
    {
        AssertCode("R6.L = W[I1];", "2E9D");
    }

    [Test]
    public void BlackfinDasm_ld_zext_postinc()
    {
        var instr = DisassembleHexBytes("8894");
        Assert.AreEqual("R0 = W[P1--] (Z);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_hi()
    {
        AssertCode("R0.H = W[I0];", "409D");
    }

    [Test]
    public void BlackfinDasm_ld_long_bx()
    {
        var instr = DisassembleHexBytes("A8E50100");
        Assert.AreEqual("R0 = B[P5 + 0x0001] (X);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_lsl()
    {
        var instr = DisassembleHexBytes("404F");
        Assert.AreEqual("R0 <<= 08;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_or3()
    {
        var instr = DisassembleHexBytes("0856");
        Assert.AreEqual("R0 = R0 | R1;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_jump_l()
    {
        var instr = DisassembleHexBytes("FFE295FF");
        Assert.AreEqual("JUMP.L 000FFF2A;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_store_byte_uimm()
    {
        var instr = DisassembleHexBytes("B0E61000");
        Assert.AreEqual("B[SP + 0x0010] = R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_push_p5()
    {
        var instr = DisassembleHexBytes("C504");
        Assert.AreEqual("[--SP] = (P5:P5);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_load_ptr()
    {
        var instr = DisassembleHexBytes("5591");
        Assert.AreEqual("P5 = [P2];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_add3_p()
    {
        var instr = DisassembleHexBytes("555B");
        Assert.AreEqual("P5 = P5 + P2;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_pop_p5()
    {
        var instr = DisassembleHexBytes("8504");
        Assert.AreEqual("(P5:P5) = [SP++];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mov_cc_eq_p_imm()
    {
        var instr = DisassembleHexBytes("420C");
        Assert.AreEqual("CC = P2 == 00000002;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_call_indir()
    {
        var instr = DisassembleHexBytes("6200");
        Assert.AreEqual("CALL [P2];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_excpt()
    {
        var instr = DisassembleHexBytes("A100");
        Assert.AreEqual("EXCPT 01;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_st_p_long()
    {
        var instr = DisassembleHexBytes("29E70F08");
        Assert.AreEqual("[P5 + 0x203C] = P1;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_lsr3()
    {
        var instr = DisassembleHexBytes("82C6C281");
        Assert.AreEqual("R0 = R2 >> 08;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_lsl_d_d()
    {
        var instr = DisassembleHexBytes("8140");
        Assert.AreEqual("R1 <<= R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_idx()
    {
        AssertCode("R0.L = W[I0];", "209D");
    }


    [Test]
    public void BlackfinDasm_ld_sp()
    {
        var instr = DisassembleHexBytes("8EAC");
        Assert.AreEqual("SP = [P1 + 0x0008];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_raise()
    {
        var instr = DisassembleHexBytes("9500");
        Assert.AreEqual("RAISE 05;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_link()
    {
        var instr = DisassembleHexBytes("00E82900");
        Assert.AreEqual("LINK +00000290;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_long_offs()
    {
        var instr = DisassembleHexBytes("28E42D00");
        Assert.AreEqual("R0 = [P5 + 0x00B4];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_idx_postinc()
    {
        var instr = DisassembleHexBytes("0B9C");
        Assert.AreEqual("R3 = [I1++];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mnop()
    {
        var instr = DisassembleHexBytes("03C80018");
        Assert.AreEqual("MNOP;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_lsr3_i()
    {
        var instr = DisassembleHexBytes("D144");
        Assert.AreEqual("P1 = P2 >> 01;", instr.ToString());
    }


    [Test]
    public void BlackfinDasm_ld_postdec()
    {
        var instr = DisassembleHexBytes("C998");
        Assert.AreEqual("R1 = W[P1--].B (X);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_st_dec_b()
    {
        var instr = DisassembleHexBytes("819A");
        Assert.AreEqual("B[P0--] = R1;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_postdec_zb()
    {
        var instr = DisassembleHexBytes("9998");
        Assert.AreEqual("R1 = W[P3--].B (Z);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_add3_lo()
    {
        var instr = DisassembleHexBytes("02C41104");
        Assert.AreEqual("R2.L = R2.L + R1.L;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_p()
    {
        var instr = DisassembleHexBytes("41AC");
        Assert.AreEqual("P1 = [P0 + 0x0004];", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_sub_p_p()
    {
        var instr = DisassembleHexBytes("2944");
        Assert.AreEqual("P1 -= P5;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_st_p_d_w()
    {
        var instr = DisassembleHexBytes("1797");
        Assert.AreEqual("W[P2] = R7;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_mov_d_xl()
    {
        var instr = DisassembleHexBytes("8242");
        Assert.AreEqual("R2 = R0.L (X);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_lsr_d_d()
    {
        var instr = DisassembleHexBytes("4740");
        Assert.AreEqual("R7 >>= R0;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_p_zl()
    {
        var instr = DisassembleHexBytes("1095");
        Assert.AreEqual("R0 = W[P2] (Z);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_sp_wx()
    {
        var instr = DisassembleHexBytes("30AA");
        Assert.AreEqual("R0 = W[SP + 0x0010] (X);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_shift1add()
    {
        var instr = DisassembleHexBytes("955C");
        Assert.That(instr.ToString(), Is.EqualTo("P2 = P5 + (P2 << 1);"));
    }

    [Test]
    public void BlackfinDasm_shift2add()
    {
        var instr = DisassembleHexBytes("955E");
        Assert.That(instr.ToString(), Is.EqualTo("P2 = P5 + (P2 << 2);"));
    }

    [Test]
    public void BlackfinDasm_sra()
    {
        var instr = DisassembleHexBytes("82C66101");
        Assert.AreEqual("R0 = R1 >>> F4;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_neg_cc()
    {
        var instr = DisassembleHexBytes("1802");
        Assert.AreEqual("CC = !CC;", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_wz()
    {
        var instr = DisassembleHexBytes("A0A4");
        Assert.AreEqual("R0 = W[P4 + 0x0004] (Z);", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_ld_dlo()
    {
        var instr = DisassembleHexBytes("229C");
        Assert.AreEqual("R2.L = W[I0++];", instr.ToString());
    }



#if BORED

// This file contains unit tests automatically generated by Reko decompiler.
// Please copy the contents of this file and report it on GitHub, using the 
// following URL: https://github.com/uxmal/reko/issues

// Reko: a decoder for the instruction B047 at address 00000018 has not been implemented. (0b0100............)
// Reko: a decoder for the instruction 0081 at address 00082A1A has not been implemented. (0b1000............)
[Test]
public void BlackfinDasm_0281()
{
AssertCode("R4 = [P2 ++ P0];", "0281");
}
// Reko: a decoder for the instruction 567C at address 00082A20 has not been implemented. (0b0111............)

// Reko: a decoder for the instruction 40EBD002 at address 00082A22 has not been implemented.




    [Test]
    public void BlackfinDasm_E0B21003()
    {
        var instr = DisassembleHexBytes("B2E00310");
        Assert.AreEqual("@@@", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_E0B21002()
    {
        var instr = DisassembleHexBytes("B2E00210");
        Assert.AreEqual("@@@", instr.ToString());
    }


    [Test]
    public void BlackfinDasm_C0801808()
    {
        var instr = DisassembleHexBytes("80C00818");
        Assert.AreEqual("@@@", instr.ToString());
    }



    // Reko: a decoder for Blackfin instruction E0A21007 at address 07F41316 has not been implemented. (0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0A21007()
    {
        var instr = DisassembleHexBytes("A2E00710");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction E0A22006 at address 07F4132C has not been implemented. (0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0A22006()
    {
        var instr = DisassembleHexBytes("A2E00620");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction E0A32004 at address 07F41376 has not been implemented. (0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0A32004()
    {
        var instr = DisassembleHexBytes("A3E00420");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction E0A52005 at address 07F41384 has not been implemented. (0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0A52005()
    {
        var instr = DisassembleHexBytes("A5E00520");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction E0A22003 at address 07F413BC has not been implemented. (0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0A22003()
    {
        var instr = DisassembleHexBytes("A2E00320");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction E0A21002 at address 07F413FA has not been implemented. (0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0A21002()
    {
        var instr = DisassembleHexBytes("A2E00210");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction E0A22002 at address 07F41474 has not been implemented. (0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0A22002()
    {
        var instr = DisassembleHexBytes("A2E00220");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction E0B3002B at address 07F4172A has not been implemented. (2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0B3002B()
    {
        var instr = DisassembleHexBytes("B3E02B00");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction E0B81011 at address 07F41774 has not been implemented. (1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0B81011()
    {
        var instr = DisassembleHexBytes("B8E01110");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction C28C0183 at address 07F422E4 has not been implemented. (83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_C28C0183()
    {
        var instr = DisassembleHexBytes("8CC28301");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction 9710 at address 07F4254C has not been implemented. (10970000 - 0b1001............)
 

    // Reko: a decoder for Blackfin instruction 9D00 at address 07F4553E has not been implemented. (009D0000 - 10970000 - 0b1001............)




    [Test]
    public void BlackfinDasm_E0B22009()
    {
        var instr = DisassembleHexBytes("B2E00920");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction 45C2 at address 07F4901E has not been implemented. (C2450000 - 0b0100............)


    // Reko: a decoder for Blackfin instruction 45FA at address 07F4902C has not been implemented. (FA450000 - C2450000 - 0b0100............)


    // Reko: a decoder for Blackfin instruction E0B22010 at address 07F561C8 has not been implemented. (1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_E0B22010()
    {
        var instr = DisassembleHexBytes("B2E01020");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction 9552 at address 07F57486 has not been implemented. (52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
    [Test]
    public void BlackfinDasm_9552()
    {
        AssertCode("R2 = W[P2] (X);", "5295");
    }





    // Reko: a decoder for Blackfin instruction C28C01B8 at address 07F5A508 has not been implemented. (B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_C28C01B8()
    {
        var instr = DisassembleHexBytes("8CC2B801");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction C2882001 at address 07F5AA0E has not been implemented. (012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_C2882001()
    {
        var instr = DisassembleHexBytes("88C20120");
        Assert.AreEqual("@@@", instr.ToString());
    }
    
    // Reko: a decoder for Blackfin instruction C28C0031 at address 07F5AC7C has not been implemented. (31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_C28C0031()
    {
        var instr = DisassembleHexBytes("8CC23100");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction C28C0008 at address 07F5B0B8 has not been implemented. (08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_C28C0008()
    {
        var instr = DisassembleHexBytes("8CC20800");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction C2882097 at address 07F5B340 has not been implemented. (972088C2 - 012088C2 - 0820B2E0 - 08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_C2882097()
    {
        var instr = DisassembleHexBytes("88C29720");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction 4103 at address 07F5D5FC has not been implemented. (03410000 - 51440000 - C1450000 - 55440000 - 4A440000 - 4A440000 - 4A440000 - 52440000 - 4D440000 - 4D440000 - 4D440000 - 78410000 - 78410000 - 4A440000 - 69440000 - FA450000 - C2450000 - 0b0100............)


    // Reko: a decoder for Blackfin instruction 4468 at address 07F5DA5E has not been implemented. (68440000 - 52440000 - 52440000 - 52440000 - 50440000 - 03410000 - 51440000 - C1450000 - 55440000 - 4A440000 - 4A440000 - 4A440000 - 52440000 - 4D440000 - 4D440000 - 4D440000 - 78410000 - 78410000 - 4A440000 - 69440000 - FA450000 - C2450000 - 0b0100............)





    // Reko: a decoder for Blackfin instruction 9E61 at address 07F61632 has not been implemented. (619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)


    // Reko: a decoder for Blackfin instruction 9E6F at address 07F6167E has not been implemented. (6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)
 

    // Reko: a decoder for Blackfin instruction 9E62 at address 07F61696 has not been implemented. (629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)




    // Reko: a decoder for Blackfin instruction 9488 at address 07F61A06 has not been implemented. (88940000 - 28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)


    // Reko: a decoder for Blackfin instruction 9E65 at address 07F61B96 has not been implemented. (659E0000 - 13970000 - 609E0000 - 88940000 - 28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)

    // Reko: a decoder for Blackfin instruction 9D2E at address 07F61B9A has not been implemented. (2E9D0000 - 659E0000 - 13970000 - 609E0000 - 88940000 - 28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)


    // Reko: a decoder for Blackfin instruction 9E67 at address 07F61F76 has not been implemented. (679E0000 - 609E0000 - 619E0000 - 629E0000 - 609E0000 - 619E0000 - 629E0000 - 0E970000 - 2E9D0000 - 659E0000 - 13970000 - 609E0000 - 88940000 - 28960000 - 619E0000 - 08960000 - 08960000 - 629E0000 - 6F9E0000 - 619E0000 - 639E0000 - 38970000 - 10970000 - 11970000 - 10970000 - 389F0000 - 639E0000 - 10970000 - 10970000 - 10970000 - 11970000 - 50950000 - 52950000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 10970000 - 50950000 - 50950000 - 52950000 - 609E0000 - 10970000 - 08970000 - 10970000 - 28970000 - 10970000 - 11970000 - 11970000 - 11970000 - 10970000 - 10970000 - 08970000 - 17970000 - 08970000 - 17970000 - 10970000 - 10970000 - 10970000 - 2F970000 - 10970000 - 2F970000 - 28970000 - 10970000 - 10970000 - 10970000 - 08970000 - 10970000 - 10970000 - 15970000 - 10970000 - 28970000 - 20970000 - 11970000 - 209D0000 - 009D0000 - 009D0000 - 10970000 - 0b1001............)



    [Test]
    public void BlackfinDasm_mov_r_cc()
    {
        var instr = DisassembleHexBytes("8603");
        Assert.AreEqual("AQ = CC;", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction 4208 at address 07F6726A has not been implemented. (08420000 - 4A440000 - 48410000 - 50410000 - 51440000 - D1450000 - 48410000 - 48410000 - 70410000 - 48410000 - 48410000 - 60410000 - 48410000 - 50410000 - 60410000 - 60410000 - 41410000 - 45410000 - 68410000 - 50410000 - 68410000 - 68410000 - 78410000 - 48410000 - 52440000 - 69440000 - 4C440000 - E9450000 - 4C440000 - 68440000 - 52440000 - 52440000 - 52440000 - 50440000 - 03410000 - 51440000 - C1450000 - 55440000 - 4A440000 - 4A440000 - 4A440000 - 52440000 - 4D440000 - 4D440000 - 4D440000 - 78410000 - 78410000 - 4A440000 - 69440000 - FA450000 - C2450000 - 0b0100............)


    // Reko: a decoder for Blackfin instruction C0814608 at address 07F673A0 has not been implemented. (084681C0 - 081880C0 - 081880C0 - 0520A2E0 - 0220A2E0 - 0220A2E0 - 0520A2E0 - 0220A2E0 - 0520B3E0 - 0520B3E0 - 0520B3E0 - 0510B3E0 - 0B20B3E0 - 0620B3E0 - 5D00B2E0 - 0610B2E0 - 0520B2E0 - 0920B2E0 - 0220B2E0 - 0D20B2E0 - 450092E0 - 2910B2E0 - 0620B3E0 - 0500B3E0 - 0420B3E0 - 0A10B2E0 - 1D10B3E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 0420B2E0 - 0420B2E0 - 0420B2E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 1710B2E0 - 1710B2E0 - 1810B2E0 - 1810B2E0 - 1710B2E0 - 0F20B2E0 - 0510B2E0 - 0320B2E0 - 0220B2E0 - 972088C2 - 012088C2 - 0820B2E0 - 08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_C0814608()
    {
        var instr = DisassembleHexBytes("81C00846");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction C0815801 at address 07F673A4 has not been implemented. (015881C0 - 084681C0 - 081880C0 - 081880C0 - 0520A2E0 - 0220A2E0 - 0220A2E0 - 0520A2E0 - 0220A2E0 - 0520B3E0 - 0520B3E0 - 0520B3E0 - 0510B3E0 - 0B20B3E0 - 0620B3E0 - 5D00B2E0 - 0610B2E0 - 0520B2E0 - 0920B2E0 - 0220B2E0 - 0D20B2E0 - 450092E0 - 2910B2E0 - 0620B3E0 - 0500B3E0 - 0420B3E0 - 0A10B2E0 - 1D10B3E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 0420B2E0 - 0420B2E0 - 0420B2E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 1710B2E0 - 1710B2E0 - 1810B2E0 - 1810B2E0 - 1710B2E0 - 0F20B2E0 - 0510B2E0 - 0320B2E0 - 0220B2E0 - 972088C2 - 012088C2 - 0820B2E0 - 08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_C0815801()
    {
        var instr = DisassembleHexBytes("81C00158");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // Reko: a decoder for Blackfin instruction C08B3800 at address 07F673B0 has not been implemented. (00388BC0 - 015881C0 - 084681C0 - 081880C0 - 081880C0 - 0520A2E0 - 0220A2E0 - 0220A2E0 - 0520A2E0 - 0220A2E0 - 0520B3E0 - 0520B3E0 - 0520B3E0 - 0510B3E0 - 0B20B3E0 - 0620B3E0 - 5D00B2E0 - 0610B2E0 - 0520B2E0 - 0920B2E0 - 0220B2E0 - 0D20B2E0 - 450092E0 - 2910B2E0 - 0620B3E0 - 0500B3E0 - 0420B3E0 - 0A10B2E0 - 1D10B3E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 0420B2E0 - 0420B2E0 - 0420B2E0 - 0220B2E0 - 0220B2E0 - 0220B2E0 - 1710B2E0 - 1710B2E0 - 1810B2E0 - 1810B2E0 - 1710B2E0 - 0F20B2E0 - 0510B2E0 - 0320B2E0 - 0220B2E0 - 972088C2 - 012088C2 - 0820B2E0 - 08008CC2 - 0820B2E0 - 31008CC2 - 0920B2E0 - 0C00B2E0 - 0920B2E0 - 0C00B2E0 - 012088C2 - B8018CC2 - B8018CC2 - 0520B2E0 - 0F20B2E0 - 1020B2E0 - 0F20B2E0 - 1320B2E0 - 0F20B2E0 - 1020B2E0 - 0620B2E0 - 9D10B3E0 - 0220B2E0 - 0420B2E0 - 1110B3E0 - 0920B2E0 - 1A10B3E0 - 1810B3E0 - 0320B2E0 - 0F10B2E0 - 0C10B2E0 - 0C10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 0E10B2E0 - 83018CC2 - 1110B8E0 - 2B00B3E0 - 0220A2E0 - 0210A2E0 - 0320A2E0 - 0320A2E0 - 0210A2E0 - 0320A2E0 - 0420A3E0 - 0520A5E0 - 0420A3E0 - 0620A2E0 - 0710A2E0 - 0210B2E0 - 0210B2E0 - 0310B2E0 - 0310B2E0 - 081880C0 - 081880C0)
    [Test]
    public void BlackfinDasm_C08B3800()
    {
        var instr = DisassembleHexBytes("8BC00038");
        Assert.AreEqual("@@@", instr.ToString());
    }

    [Test]
    public void BlackfinDasm_C0883800()
    {
        var instr = DisassembleHexBytes("88C0A5E5");
        Assert.AreEqual("@@@", instr.ToString());
    }

    // This file contains unit tests automatically generated by Reko decompiler.
    // Please copy the contents of this file and report it on GitHub, using the 
    // following URL: https://github.com/uxmal/reko/issues

    // Reko: a decoder for the instruction B3E00520 at address 07F64946 has not been implemented.
    [Test]
public void BlackfinDasm_E0B32005()
{
AssertCode("@@@", "B3E00520");
}
// Reko: a decoder for the instruction B2E00220 at address 07F619C0 has not been implemented.
[Test]
public void BlackfinDasm_E0B22002()
{
AssertCode("@@@", "B2E00220");
}
// Reko: a decoder for the instruction 619E at address 07F61632 has not been implemented. (0b1001............)
// Reko: a decoder for the instruction 6F9E at address 07F6167E has not been implemented. (0b1001............)



    // Reko: a decoder for the instruction B2E00D20 at address 07F61576 has not been implemented.
    [Test]
public void BlackfinDasm_E0B2200D()
{
AssertCode("@@@", "B2E00D20");
}
// Reko: a decoder for the instruction B2E00F20 at address 07F56AC0 has not been implemented.
[Test]
public void BlackfinDasm_E0B2200F()
{
AssertCode("@@@", "B2E00F20");
}
// Reko: a decoder for the instruction B2E01320 at address 07F567E0 has not been implemented.
[Test]
public void BlackfinDasm_E0B22013()
{
AssertCode("@@@", "B2E01320");
}
// Reko: a decoder for the instruction B3E01110 at address 07F4C9FA has not been implemented.
[Test]
public void BlackfinDasm_E0B31011()
{
AssertCode("@@@", "B3E01110");
}
// Reko: a decoder for the instruction B2E00E10 at address 07F449F6 has not been implemented.
[Test]
public void BlackfinDasm_E0B2100E()
{
AssertCode("@@@", "B2E00E10");
}
// Reko: a decoder for the instruction B2E00C10 at address 07F44C9E has not been implemented.
[Test]
public void BlackfinDasm_E0B2100C()
{
AssertCode("@@@", "B2E00C10");
}
// Reko: a decoder for the instruction B2E00F10 at address 07F44FFC has not been implemented.
[Test]
public void BlackfinDasm_E0B2100F()
{
AssertCode("@@@", "B2E00F10");
}
// Reko: a decoder for the instruction 009D at address 07F4553E has not been implemented. (0b1001............)
// Reko: a decoder for the instruction B2E00320 at address 07F45836 has not been implemented.
[Test]
public void BlackfinDasm_E0B22003()
{
AssertCode("@@@", "B2E00320");
}


    // Reko: a decoder for the instruction 209D at address 07F46616 has not been implemented. (0b1001............)




 
    // Reko: a decoder for the instruction B3E01810 at address 07F47B50 has not been implemented.
    [Test]
public void BlackfinDasm_E0B31018()
{
AssertCode("@@@", "B3E01810");
}
// Reko: a decoder for the instruction B3E01A10 at address 07F47BD4 has not been implemented.
[Test]
public void BlackfinDasm_E0B3101A()
{
AssertCode("@@@", "B3E01A10");
}
// Reko: a decoder for the instruction B2E00420 at address 07F4D846 has not been implemented.
[Test]
public void BlackfinDasm_E0B22004()
{
AssertCode("@@@", "B2E00420");
}
// Reko: a decoder for the instruction B3E09D10 at address 07F51AF4 has not been implemented.
[Test]
public void BlackfinDasm_E0B3109D()
{
AssertCode("@@@", "B3E09D10");
}
// Reko: a decoder for the instruction B2E00620 at address 07F538B6 has not been implemented.
[Test]
public void BlackfinDasm_E0B22006()
{
AssertCode("@@@", "B2E00620");
}
// Reko: a decoder for the instruction 609E at address 07F57142 has not been implemented. (0b1001............)

// Reko: a decoder for the instruction B2E00520 at address 07F57994 has not been implemented.
[Test]
public void BlackfinDasm_E0B22005()
{
AssertCode("@@@", "B2E00520");
}
// Reko: a decoder for the instruction B2E00C00 at address 07F5AA62 has not been implemented.
[Test]
public void BlackfinDasm_E0B2000C()
{
AssertCode("@@@", "B2E00C00");
}
// Reko: a decoder for the instruction B2E00820 at address 07F5ADC0 has not been implemented.
[Test]
public void BlackfinDasm_E0B22008()
{
AssertCode("@@@", "B2E00820");
}
// Reko: a decoder for the instruction B2E00510 at address 07F5C34E has not been implemented.
[Test]
public void BlackfinDasm_E0B21005()
{
AssertCode("@@@", "B2E00510");
}
// Reko: a decoder for the instruction B2E01710 at address 07F5D874 has not been implemented.
[Test]
public void BlackfinDasm_E0B21017()
{
AssertCode("@@@", "B2E01710");
}
// Reko: a decoder for the instruction B2E01810 at address 07F5D8B8 has not been implemented.
[Test]
public void BlackfinDasm_E0B21018()
{
AssertCode("@@@", "B2E01810");
}
// Reko: a decoder for the instruction B3E01D10 at address 07F5E1DC has not been implemented.
[Test]
public void BlackfinDasm_E0B3101D()
{
AssertCode("@@@", "B3E01D10");
}
// Reko: a decoder for the instruction B2E00A10 at address 07F5E22C has not been implemented.
[Test]
public void BlackfinDasm_E0B2100A()
{
AssertCode("@@@", "B2E00A10");
}
// Reko: a decoder for the instruction B3E00420 at address 07F5FA0C has not been implemented.
[Test]
public void BlackfinDasm_E0B32004()
{
AssertCode("@@@", "B3E00420");
}
// Reko: a decoder for the instruction B3E00500 at address 07F5FA38 has not been implemented.
[Test]
public void BlackfinDasm_E0B30005()
{
AssertCode("@@@", "B3E00500");
}
// Reko: a decoder for the instruction 629E at address 07F61696 has not been implemented. (0b1001............)

// Reko: a decoder for the instruction B2E00610 at address 07F61A8C has not been implemented.
[Test]
public void BlackfinDasm_E0B21006()
{
AssertCode("@@@", "B2E00610");
}
// Reko: a decoder for the instruction 659E at address 07F61B96 has not been implemented. (0b1001............)

// Reko: a decoder for the instruction 2E9D at address 07F61B9A has not been implemented. (0b1001............)

// Reko: a decoder for the instruction B2E05D00 at address 07F61E14 has not been implemented.
[Test]
public void BlackfinDasm_E0B2005D()
{
AssertCode("@@@", "B2E05D00");
}
// Reko: a decoder for the instruction 679E at address 07F61F76 has not been implemented. (0b1001............)

// Reko: a decoder for the instruction 639E at address 07F61F92 has not been implemented. (0b1001............)

// Reko: a decoder for the instruction B3E00620 at address 07F6200E has not been implemented.
[Test]
public void BlackfinDasm_E0B32006()
{
AssertCode("@@@", "B3E00620");
}
// Reko: a decoder for the instruction B3E00B20 at address 07F6288A has not been implemented.
[Test]
public void BlackfinDasm_E0B3200B()
{
AssertCode("@@@", "B3E00B20");
}
// Reko: a decoder for the instruction B3E00510 at address 07F638D2 has not been implemented.
[Test]
public void BlackfinDasm_E0B31005()
{
AssertCode("@@@", "B3E00510");
}
// Reko: a decoder for the instruction A2E00520 at address 07F672A2 has not been implemented.
[Test]
public void BlackfinDasm_E0A22005()
{
AssertCode("@@@", "A2E00520");
}
// Reko: a decoder for the instruction 81C00846 at address 07F673A0 has not been implemented.

#endif
}
