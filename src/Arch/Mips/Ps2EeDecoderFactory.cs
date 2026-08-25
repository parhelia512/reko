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

using Reko.Core;
using Reko.Core.Machine;

namespace Reko.Arch.Mips;

partial class MipsDisassembler
{
    public class Ps2EeDecoderFactory : DecoderFactory
    {
        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1C()
        {
            var mmi0 = Mask(6, 5, "  PS2 - MMI0",
                Instr(Mnemonic.paddw, R3, R1, R2),
                Instr(Mnemonic.psubw, R3, R1, R2),
                Instr(Mnemonic.pcgtw, R3, R1, R2),
                Instr(Mnemonic.pmaxw, R3, R1, R2),

                Instr(Mnemonic.paddh, R3, R1, R2),
                Instr(Mnemonic.psubh, R3, R1, R2),
                Instr(Mnemonic.pcgth, R3, R1, R2),
                Instr(Mnemonic.pmaxh, R3, R1, R2),

                Instr(Mnemonic.paddb, R3, R1, R2),
                Instr(Mnemonic.psubb, R3, R1, R2),
                Instr(Mnemonic.pcgtb, R3, R1, R2),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.paddsw, R3, R1, R2),
                Instr(Mnemonic.psubsw, R3, R1, R2),
                Nyi("pextlw"),
                Nyi("ppacw"),

                Instr(Mnemonic.paddsh, R3, R1, R2),
                Instr(Mnemonic.psubsh, R3, R1, R2),
                Nyi("pextlh"),
                Nyi("ppach"),

                Instr(Mnemonic.paddsb, R3, R1, R2),
                Instr(Mnemonic.psubsb, R3, R1, R2),
                Nyi("pextlb"),
                Nyi("ppacb"),

                invalid,
                invalid,
                Nyi("pext5"),
                Nyi("ppac5"));

            var mmi1 = Mask(6, 5, "  PS2 - MMI1",
                invalid,
                Nyi("pabsw"),
                Instr(Mnemonic.pceqw, R3, R1, R2),
                Instr(Mnemonic.pminw, R3, R1, R2),

                Nyi("padsbh"),
                Nyi("pabsh"),
                Instr(Mnemonic.pceqh, R3, R1, R2),
                Instr(Mnemonic.pminh, R3, R1, R2),

                invalid,
                invalid,
                Instr(Mnemonic.pceqb, R3, R1, R2),
                invalid,

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.padduw, R3, R1, R2),
                Instr(Mnemonic.psubuw, R3, R1, R2),
                Nyi("pextuw"),
                invalid,

                Instr(Mnemonic.padduh, R3, R1, R2),
                Instr(Mnemonic.psubuh, R3, R1, R2),
                Nyi("pextuh"),
                invalid,

                Instr(Mnemonic.paddub, R3, R1, R2),
                Instr(Mnemonic.psubub, R3, R1, R2),
                Nyi("pextub"),
                Nyi("qfsrv"),

                invalid,
                invalid,
                invalid,
                invalid);

            var mmi2 = Mask(6, 5, "  PS2 - MMI2",
                Nyi("pmaddw"),
                invalid,
                Nyi("psllvw"),
                Nyi("psrlvw"),

                Nyi("pmsubw"),
                invalid,
                invalid,
                invalid,

                Nyi("pmfhi"),
                Nyi("pmflo"),
                Nyi("pinth"),
                invalid,

                Instr(Mnemonic.pmultw, R3, R1, R2),
                Instr(Mnemonic.pdivw, R3, R1, R2),
                Instr(Mnemonic.pcpyld, R3, R1, R2),
                invalid,

                Nyi("pmaddh"),
                Nyi("phmadh"),
                Instr(Mnemonic.pand, R3, R1, R2),
                Instr(Mnemonic.pxor, R3, R1, R2),

                Nyi("pmsubh"),
                Nyi("phmsbh"),
                invalid,
                invalid,

                invalid,
                invalid,
                Nyi("pexeh"),
                Nyi("prevh"),

                Instr(Mnemonic.pmulth, R3, R1, R2),
                Nyi("pdivbw"),
                Nyi("pexew"),
                Nyi("prot3w"));

            var mmi3 = Sparse(6, 5, "  PS2 - MMI3",
                invalid,
                (0b000_00, Nyi("pmadduw")),
                (0b000_11, Nyi("psravw")),
                (0b010_00, Nyi("pmthi")),
                (0b010_01, Nyi("pmtlo")),
                (0b010_10, Nyi("pinteh")),
                (0b011_00, Nyi("pmultuw")),
                (0b011_01, Instr(Mnemonic.pdivuw, R3, R1, R2)),
                (0b011_10, Instr(Mnemonic.pcpyud, R3, R1, R2)),
                (0b100_10, Instr(Mnemonic.por, R3, R1, R2)),
                (0b100_11, Instr(Mnemonic.pnor, R3, R1, R2)),
                (0b110_10, Nyi("pexch")),
                (0b110_11, Instr(Mnemonic.pcpyh, R3, R2)),
                (0b111_10, Nyi("pexcw")));

            var mmi = Sparse(0, 6, "  PS2 - MMI",
                invalid,
                (0b000_000, Nyi("madd")),
                (0b000_001, Nyi("maddu")),
                (0b000_100, Nyi("plzcw")),
                (0b001_000, mmi0),
                (0b001_001, mmi2),
                (0b010_000, Instr(Mnemonic.mfhi1, R3)),
                (0b010_001, Instr(Mnemonic.mthi1, R3)),
                (0b010_010, Instr(Mnemonic.mflo1, R3)),
                (0b010_011, Instr(Mnemonic.mtlo1, R3)),
                (0b011_000, Instr(Mnemonic.mult1, R3,R1,R2)),
                (0b011_001, Instr(Mnemonic.multu1, R3,R1,R2)),
                (0b011_010, Instr(Mnemonic.div1, R3,R1,R2)),
                (0b011_011, Instr(Mnemonic.divu1, R3,R1,R2)),
                (0b100_000, Nyi("madd1")),
                (0b100_001, Nyi("maddu1")),
                (0b101_000, mmi1),
                (0b101_001, mmi3),
                (0b110_000, Nyi("pmfhl")),
                (0b110_001, Nyi("pmthl")),
                (0b110_100, Nyi("psllh")),
                (0b110_110, Nyi("psrlh")),
                (0b110_111, Nyi("psrah")),
                (0b111_100, Nyi("psllw")),
                (0b111_110, Nyi("psrlw")),
                (0b111_111, Nyi("psraw")));
            return mmi;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1E()
        {
            return Instr(Mnemonic.lq, R2, Eq);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode1F()
        {
            return Instr(Mnemonic.sq, R2, Eq);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop0()
        {
            var c0 = Sparse(0, 6, "  PS2 - C0",
                invalid,
                (0b000_001, Nyi("tlbr")),
                (0b000_010, Nyi("tlbwi")),
                (0b000_110, Nyi("tlbwr")),
                (0b001_000, Nyi("tlbp")),
                (0b011_000, Nyi("eret")),
                (0b111_000, Instr(Mnemonic.ei)),
                (0b111_001, Instr(Mnemonic.di)));
            var cop0 = Sparse(21, 5, "  PS2 - COP0",
                invalid,
                (0b00_000, Instr(Mnemonic.mf0, R2)),
                (0b00_100, Nyi("mt0")),
                (0b01_000, Nyi("bc0")),
                (0b10_000, c0));
            return cop0;
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeCop1_S()
        {
            return Sparse(0, 6, "  S class",
                invalid,
                (0b000_000, Instr(Mnemonic.add_s, F4, F3, F2)),
                (0b000_001, Instr(Mnemonic.sub_s, F4, F3, F2)),
                (0b000_010, Instr(Mnemonic.mul_s, F4, F3, F2)),
                (0b000_011, Instr(Mnemonic.div_s, F4, F3, F2)),
                (0b000_100, Instr(Mnemonic.sqrt_s, F4, F3)),
                (0b000_101, Instr(Mnemonic.abs_s, F4, F3)),
                (0b000_110, Instr(Mnemonic.mov_s, F4, F3)),
                (0b000_111, Instr(Mnemonic.neg_s, F4, F3)),

                (0b010_110, Nyi("rsqrt_s")),

                (0b011_000, Nyi("adda_s")),
                (0b011_001, Nyi("suba_s")),
                (0b011_010, Nyi("mula_s")),
                (0b011_100, Nyi("madd_s")),
                (0b011_101, Nyi("msub_s")),
                (0b011_110, Nyi("madda_s")),
                (0b011_111, Nyi("msuba_s")),

                (0b100_100, Instr(Mnemonic.cvt_w_s, F4, F3)),

                (0b101_000, Nyi("max_s")),
                (0b101_001, Nyi("min_s")),

                (0b110_000, Nyi("c_f_s")),
                (0b110_010, Nyi("c_eq_s")),
                (0b110_100, Nyi("c_lt_s")),
                (0b110_110, Nyi("c_le_s")));
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeWclass()
        {
            return Sparse(0, 6, " PS2 - WClass",
                invalid,
                (0b100_000, Instr(Mnemonic.cvt_s_w, F4, F3)));
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode36()
        {
            return Nyi("LQC2");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Decode3E()
        {
            return Nyi("SQC2");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeRegimm18()
        {
            return Nyi("mtsab");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeRegimm19()
        {
            return Nyi("mtsah");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial28()
        {
            return Nyi("mfsa");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial29()
        {
            return Nyi("mtsa)");
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2C()
        {
            return Instr(Mnemonic.dadd, R3, R1, R2);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2D()
        {
            return Instr(Mnemonic.daddu, R3, R1, R2);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2E()
        {
            return Instr(Mnemonic.dsub, R3, R1, R2);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> DecodeSpecial2F()
        {
            return Instr(Mnemonic.dsubu, R3, R1, R2);
        }

        protected override Decoder<MipsDisassembler, Mnemonic, MipsInstruction> Instr64(Mnemonic mnemonic, params Mutator<MipsDisassembler>[] mutators)
        {
            return Instr(mnemonic, mutators);
        }
    }
}
