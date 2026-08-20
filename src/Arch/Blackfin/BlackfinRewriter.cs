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
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Arch.Blackfin
{
    public class BlackfinRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly BlackfinArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly IEnumerator<BlackfinInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private InstrClass iclass;
        private BlackfinInstruction instr;

        public BlackfinRewriter(BlackfinArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            this.dasm = new BlackfinDisassembler(arch, rdr).GetEnumerator();
            this.rtls = [];
            this.m = new RtlEmitter(rtls);
            this.instr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest(instr);
                    iclass = InstrClass.Invalid;
                    goto case Mnemonic.invalid;
                case Mnemonic.invalid:
                    this.iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.add: RewriteArithmetic(Operator.IAdd); break;
                case Mnemonic.add3: RewriteArithmetic3(Operator.IAdd); break;
                case Mnemonic.add_sh1: RewriteAddShift(1); break;
                case Mnemonic.add_sh2: RewriteAddShift(2); break;
                case Mnemonic.and3: RewriteLogical3(Operator.And); break;
                case Mnemonic.asr: RewriteShift(Operator.Sar); break;
                case Mnemonic.asr3: RewriteShift3(Operator.Sar); break;
                case Mnemonic.bitclr: RewriteBitclrset(CommonOps.ClearBit); break;
                case Mnemonic.bitset: RewriteBitclrset(CommonOps.SetBit); break;
                case Mnemonic.bittgl: RewriteBitclrset(CommonOps.InvertBit); break;
                case Mnemonic.CALL: RewriteCall(); break;
                case Mnemonic.CLI: RewriteCli(); break;
                case Mnemonic.CSYNC: RewriteSync(csync_intrinsic); break;
                case Mnemonic.DIVQ: RewriteDivq(); break;
                case Mnemonic.EXCPT: RewriteExcpt(); break;
                case Mnemonic.if_cc_jump: RewriteIf(); break;
                case Mnemonic.if_cc_jump_bp: RewriteIf(); break;
                case Mnemonic.if_cc_mov: RewriteMoveIf(ConditionCode.EQ); break;
                case Mnemonic.if_ncc_jump: RewriteIfNot(); break;
                case Mnemonic.if_ncc_jump_bp: RewriteIfNot(); break;
                case Mnemonic.if_ncc_mov: RewriteMoveIf(ConditionCode.NE); break;
                case Mnemonic.JUMP: RewriteJump(); break;
                case Mnemonic.JUMP_L: RewriteJump(); break;
                case Mnemonic.JUMP_S: RewriteJump(); break;
                case Mnemonic.LINK: RewriteLink(); break;
                case Mnemonic.lsl: RewriteShift(Operator.Shl); break;
                case Mnemonic.lsl3: RewriteShift3(Operator.Shl); break;
                case Mnemonic.lsr: RewriteShift(Operator.Shr); break;
                case Mnemonic.lsr3: RewriteShift3(Operator.Shr); break;
                case Mnemonic.mov: RewriteMov(); break;
                case Mnemonic.mov_cc_bittest: RewriteMovBittest(false); break; 
                case Mnemonic.mov_cc_eq: RewriteCompareDataRegister(Operator.Eq); break;
                case Mnemonic.mov_cc_le: RewriteCompareDataRegister(Operator.Le); break;
                case Mnemonic.mov_cc_lt: RewriteCompareDataRegister(Operator.Lt); break;
                case Mnemonic.mov_cc_ule: RewriteCompareDataRegister(Operator.Ule); break;
                case Mnemonic.mov_cc_ult: RewriteCompareDataRegister(Operator.Ult); break;
                case Mnemonic.mov_cc_n_bittest: RewriteMovBittest(true); break; 
                case Mnemonic.mov_post: RewriteMovPost(); break;
                case Mnemonic.mov_pre: RewriteMovPre(); break;
                case Mnemonic.mov_x: RewriteMovx(); break;
                case Mnemonic.mov_xb: RewriteMovxb(); break;
                case Mnemonic.mov_xl: RewriteMovxl(); break;
                case Mnemonic.mov_z: RewriteMovz(); break;
                case Mnemonic.mov_zb: RewriteMovz(PrimitiveType.Byte); break;
                case Mnemonic.mov_zl: RewriteMovz(PrimitiveType.Word16); break;
                case Mnemonic.mul: RewriteMul(); break;
                case Mnemonic.neg: RewriteNeg(); break;
                case Mnemonic.neg_cc: RewriteNegCc(); break;
                case Mnemonic.not: RewriteNot(); break;
                case Mnemonic.or3: RewriteLogical3(Operator.Or); break;
                case Mnemonic.NOP: m.Nop(); break;
                case Mnemonic.RAISE: RewriteRaise(); break;
                case Mnemonic.RTI: RewriteRti(); break;
                case Mnemonic.RTN: RewriteRtn(); break;
                case Mnemonic.RTS: RewriteRts(); break;
                case Mnemonic.RTX: RewriteRtx(); break;
                case Mnemonic.shift1add: RewriteShiftAdd(1); break;
                case Mnemonic.shift2add: RewriteShiftAdd(2); break;
                case Mnemonic.SSYNC: RewriteSync(ssync_intrinsic); break;
                case Mnemonic.STI: RewriteSti(); break;
                case Mnemonic.sub: RewriteArithmetic(Operator.ISub); break;
                case Mnemonic.sub3: RewriteArithmetic3(Operator.ISub); break;
                case Mnemonic.UNLINK: RewriteUnlink(); break;
                case Mnemonic.xor3: RewriteLogical3(Operator.Xor); break;
                }
                yield return m.MakeCluster(instr.Address, instr.Length, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EmitUnitTest(BlackfinInstruction instr)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingRewriter("BlackfinRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Address Addr(int iOperand)
        {
            return (Address) instr.Operands[iOperand];
        }

        private void EmitCc(FlagGroupStorage grf, Expression e)
        {
            m.Assign(binder.EnsureFlagGroup(grf), e);
        }

        private void EmitCc(FlagGroupStorage grf, int n)
        {
            var cc = binder.EnsureFlagGroup(grf);
            var value = m.Const(cc.DataType, n);
            m.Assign(cc, value);
        }

        private Expression SrcOperand(int iOperand)
        {
            switch (instr.Operands[iOperand])
            {
            case RegisterStorage rop:
                return binder.EnsureRegister(rop);
            case Constant imm:
                return imm;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                return m.Mem(mem.DataType, ea);
            case RegisterRange range:
                return ExtendedRegister(range);
            default:
                throw new NotImplementedException($"Operand type {instr.Operands[iOperand].GetType().Name}.");
            }
        }

        private Expression DstOperand(int iOperand, Expression src)
        {
            switch (instr.Operands[iOperand])
            {
            case RegisterStorage rop:
                var dst = binder.EnsureRegister(rop);
                m.Assign(dst, m.MaybeSlice(src, dst.DataType));
                return dst;
            case Constant imm:
                return imm;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                m.Assign(m.Mem(mem.DataType, ea), m.MaybeSlice(src, mem.DataType));
                return src;
            case RegisterRange range:
                var extReg = ExtendedRegister(range);
                m.Assign(extReg, src);
                return extReg;
            default:
                throw new NotImplementedException($"Operand type {instr.Operands[iOperand].GetType().Name}.");
            }
        }

        private Expression EffectiveAddress(MemoryOperand mem)
        {
            Expression ea;
            if (mem.Base is not null)
            {
                ea = binder.EnsureRegister(mem.Base);
                if (mem.Index is not null)
                    throw new NotImplementedException();
                if (mem.Offset != 0)
                {
                    ea = m.AddSubSignedInt(ea, mem.Offset);
                }
                if (mem.PostDecrement || mem.PostIncrement)
                {
                    var tmp = binder.CreateTemporary(ea.DataType);
                    m.Assign(tmp, ea);
                    var size = mem.DataType.Size;
                    m.Assign(ea, m.AddSubSignedInt(ea, mem.PostIncrement
                        ? size
                        : -size));
                    ea = tmp;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return ea;
        }

        private Identifier ExtendedRegister(RegisterRange range)
        {
            int nRegs = range.MaxRegister - range.MinRegister + 1;
            var regSequence = new Storage[nRegs];
            long bitsize = 0;
            for (int i = 0; i < nRegs; ++i)
            {
                var reg = range.Registers[range.MaxRegister - i];
                regSequence[i] = reg;
                bitsize += reg.DataType.BitSize; 
            }
            var dt = PrimitiveType.CreateWord(bitsize);
            return binder.EnsureSequence(dt, regSequence);
        }

        private Identifier Reg(int iOperand)
        {
            return binder.EnsureRegister((RegisterStorage) instr.Operands[iOperand]);
        }

        private void RewriteCall()
        {
            m.Call(SrcOperand(0), 0);
        }

        private void RewriteCli()
        {
            m.SideEffect(m.Fn(cli_intrinsic));
        }

        private void RewriteSync(IntrinsicProcedure intrinsic)
        {
            m.SideEffect(m.Fn(intrinsic));
        }

        private void RewriteSti()
        {
            m.SideEffect(m.Fn(sti_intrinsic, SrcOperand(0)));
        }

        private void RewriteDivq()
        {
            var dividend = this.SrcOperand(0);
            var divisor = this.SrcOperand(1);
            var aq = binder.EnsureFlagGroup(Registers.AQ);
            m.Assign(aq, m.Test(ConditionCode.NE, m.Fn(
                divq_intrinsic,
                aq,
                dividend,
                divisor,
                m.Out(dividend.DataType, dividend))));
        }

        private void RewriteExcpt()
        {
            m.SideEffect(m.Fn(excpt_intrinsic, SrcOperand(0)));
        }

        private void RewriteIf()
        {
            var cond = m.Test(ConditionCode.NE, binder.EnsureFlagGroup(Registers.CC));
            var dst = SrcOperand(0);
            m.Branch(cond, dst);
        }

        private void RewriteIfNot()
        {
            var cond = m.Test(ConditionCode.EQ, binder.EnsureFlagGroup(Registers.CC));
            var dst = SrcOperand(0);
            m.Branch(cond, dst);
        }

        private void RewriteJump()
        {
            var addrDst = SrcOperand(0);
            m.Goto(addrDst);
        }

        private void RewriteAddShift(int shift)
        {
            var src1 = SrcOperand(1);
            var src2 = SrcOperand(2);
            var tmp = binder.CreateTemporary(src2.DataType);
            m.Assign(tmp, m.IAdd(src1, src2));
            var dst = DstOperand(0, m.Shl(tmp, shift));
            EmitCc(Registers.NZV, m.Cond(Registers.NZV.DataType, dst));
            EmitCc(Registers.VS, 0);
        }

        private void RewriteArithmetic(BinaryOperator op)
        {
            var src1 = SrcOperand(0);
            var src2 = SrcOperand(1);
            var dst = DstOperand(0, m.Bin(op, src1, src2));
            EmitCc(Registers.NZVC, m.Cond(Registers.NZVC.DataType, dst));
        }

        private void RewriteArithmetic3(BinaryOperator op)
        {
            var src1 = SrcOperand(1);
            var src2 = SrcOperand(2);
            var dst = DstOperand(0, m.Bin(op, src1, src2));
            EmitCc(Registers.NZVC, m.Cond(Registers.NZVC.DataType, dst));
        }

        private void RewriteBitclrset(IntrinsicProcedure intrinsic)
        {
            var left = SrcOperand(0);
            var right = SrcOperand(1);
            m.Assign(left, m.Fn(
                intrinsic.MakeInstance(left.DataType, right.DataType),
                left, right));
            EmitCc(Registers.AN, m.Cond(Registers.AN.DataType, left));
            EmitCc(Registers.AZ, 0);
            EmitCc(Registers.V, 0);
            EmitCc(Registers.AC0, 0);
        }

        private void RewriteCompareDataRegister(BinaryOperator op)
        {
            var src1 = SrcOperand(0);
            var src2 = SrcOperand(1);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.Assign(cc, m.Cond(cc.DataType, m.Bin(op, src1, src2)));
            EmitCc(Registers.NZVC, m.Cond(Registers.NZVC.DataType, m.Bin(op, src1, src2)));
        }

        private void RewriteLink()
        {
            var localVars = ((Constant) instr.Operands[0]).ToInt32();
            var sp = binder.EnsureRegister(Registers.SP);
            var fp = binder.EnsureRegister(Registers.FP);
            // Allocate slots for RETS and old FP.
            //$TODO: actually save rets and use a special %cont continuation register.
            m.Assign(sp, m.ISubS(sp, 8));
            m.Assign(m.Mem32(sp), fp);
            if (localVars != 0)
            {
                m.Assign(sp, m.ISubS(sp, localVars));
            }
        }

        private void RewriteLogical3(BinaryOperator op)
        {
            var src1 = SrcOperand(1);
            var src2 = SrcOperand(2);
            var dst = DstOperand(0, m.Bin(op, src1, src2));
            EmitCc(Registers.NZ, m.Cond(Registers.NZ.DataType, dst));
            EmitCc(Registers.V, 0);
            EmitCc(Registers.AC0, 0);
        }

        private void RewriteMov()
        {
            var src = SrcOperand(1);
            DstOperand(0, src);
        }

        private void RewriteMovBittest(bool invert)
        {
            var src1 = SrcOperand(0);
            var src2 = SrcOperand(1);
            var cc = binder.EnsureFlagGroup(Registers.CC);
            Expression test = m.Fn(CommonOps.Bit, src1, src2);
            if (invert)
                test = m.Not(test);
            m.Assign(cc, m.Cond(cc.DataType, test));
            EmitCc(Registers.NZVC, m.Cond(cc.DataType, test));
        }

        private void RewriteMoveIf(ConditionCode ccode)
        {
            var cc = binder.EnsureFlagGroup(Registers.CC);
            m.BranchInMiddleOfInstruction(m.Test(ccode, cc),
                instr.Address + instr.Length,
                InstrClass.CondJump);
            DstOperand(0, SrcOperand(1));
        }

        private void RewriteMovPost()
        {
            var regs = (RegisterRange) instr.Operands[0];
            var mem = (MemoryOperand) instr.Operands[1];
            Debug.Assert(mem.Base is not null);
            var sp = binder.EnsureRegister(mem.Base);
            foreach (var reg in regs.Registers.Reverse())
            {
                var id = binder.EnsureRegister(reg);
                m.Assign(id, m.Mem32(sp));
                m.Assign(sp, m.IAddS(sp, 4));
            }
        }

        private void RewriteMovPre()
        {
            var mem = (MemoryOperand) instr.Operands[0];
            var regs = (RegisterRange) instr.Operands[1];
            Debug.Assert(mem.Base is not null);
            var sp = binder.EnsureRegister(mem.Base);
            foreach (var reg in regs.Registers.Reverse())
            {
                var id = binder.EnsureRegister(reg);
                m.Assign(sp, m.ISubS(sp, 4));
                m.Assign(m.Mem32(sp), id);
            }
        }


        private void RewriteMovx()
        {
            var src = SrcOperand(1);
            var from = PrimitiveType.Create(Domain.SignedInt, src.DataType.BitSize);
            m.Assign(Reg(0), m.Convert(src, from, PrimitiveType.Int32));
        }

        private void RewriteMovxb()
        {
            var src = SrcOperand(1);
            m.Assign(Reg(0), m.Convert(m.Slice(src, PrimitiveType.SByte), PrimitiveType.SByte, PrimitiveType.Int32));
        }

        private void RewriteMovxl()
        {
            var src = SrcOperand(1);
            m.Assign(Reg(0), m.Convert(m.MaybeSlice(src, PrimitiveType.Word16), PrimitiveType.Word16, PrimitiveType.Int32));
        }

        private void RewriteMovz()
        {
            var src = SrcOperand(1);
            m.Assign(Reg(0), m.Convert(src, src.DataType, PrimitiveType.Word32));
        }

        private void RewriteMovz(PrimitiveType dt)
        {
            var src = SrcOperand(1);
            var tmp = binder.CreateTemporary(dt);
            m.Assign(tmp, m.Slice(src, dt));
            m.Assign(Reg(0), m.Convert(tmp, dt, PrimitiveType.Word32));
        }

        private void RewriteMul()
        {
            Debug.Assert(instr.Operands.Length == 2);
            var dst = Reg(0);
            var src = Reg(1);
            m.Assign(dst, m.IMul(dst, src));
        }

        private void RewriteNeg()
        {
            var src = SrcOperand(1);
            var dst = DstOperand(0, m.Neg(src));
            EmitCc(Registers.NZV, m.Cond(Registers.NZV.DataType, dst));
            EmitCc(Registers.AC0, m.Eq0(dst));
        }

        private void RewriteNegCc()
        {
            var cc = binder.EnsureFlagGroup(Registers.CC);
            var src = m.Comp(cc);
            m.Assign(cc, src);
        }

        private void RewriteNot()
        {
            var src = SrcOperand(1);
            var dst = DstOperand(0, m.Comp(src));
            EmitCc(Registers.NZV, m.Cond(Registers.NZV.DataType, dst));
            EmitCc(Registers.AC0, m.Eq0(dst));
        }

        private void RewriteRaise()
        {
            m.SideEffect(m.Fn(raise_intrinsic, SrcOperand(0)));
        }

        private void RewriteRti()
        {
            // A more accurate rewriter would assign PC = RETI
            m.SideEffect(m.Fn(rti_intrinsic));
            m.Return(0, 0);
        }


        private void RewriteRtn()
        {
            // A more accurate rewriter would assign PC = RETN
            m.Return(0, 0);
        }

        private void RewriteRts()
        {
            // A more accurate rewriter would assign PC = RETS
            m.Return(0, 0);
        }


        private void RewriteRtx()
        {
            // A more accurate rewriter would assign PC = RETX
            m.SideEffect(m.Fn(rtx_intrinsic));
            m.Return(0, 0);
        }
        private void RewriteShift(BinaryOperator op)
        {
            var src1 = SrcOperand(0);
            var src2 = SrcOperand(1);
            var dst = DstOperand(0, m.Bin(op, src1, src2));
            EmitCc(Registers.NZV, m.Cond(Registers.NZV.DataType, dst));
        }

        private void RewriteShift3(BinaryOperator op)
        {
            var src1 = SrcOperand(1);
            var src2 = SrcOperand(2);
            var dst = DstOperand(0, m.Bin(op, src1, src2));
            EmitCc(Registers.NZV, m.Cond(Registers.NZV.DataType, dst));
        }

        private void RewriteShiftAdd(int shift)
        {
            var src1 = SrcOperand(1);
            var src2 = SrcOperand(2);
            var tmp = binder.CreateTemporary(src2.DataType);
            m.Assign(tmp, m.Shl(src2, shift));
            var dst = DstOperand(0, m.IAdd(src1, tmp));
            EmitCc(Registers.NZV, m.Cond(Registers.NZV.DataType, dst));
            EmitCc(Registers.VS, 0);
        }


        private void RewriteUnlink()
        {
            var sp = binder.EnsureRegister(Registers.SP);
            var fp = binder.EnsureRegister(Registers.FP);
            // Restore slots for RETS and old FP.
            //$TODO: actually use RETS and a special %cont continuation register.
            m.Assign(sp, fp);
            m.Assign(fp, m.Mem32(sp));
            m.Assign(sp, m.IAddS(sp, 8));
        }

        private static readonly IntrinsicProcedure cli_intrinsic = new IntrinsicBuilder("__cli", true)
            .Void();
        private static readonly IntrinsicProcedure csync_intrinsic = new IntrinsicBuilder("__core_synchronize", true)
            .Void();
        private static readonly IntrinsicProcedure divq_intrinsic = IntrinsicBuilder.SideEffect("__divq_step")
            .Param(PrimitiveType.Bool)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word16)
            .OutParam(PrimitiveType.Word32)
            .Returns(PrimitiveType.Bool);

        private static readonly IntrinsicProcedure excpt_intrinsic = new IntrinsicBuilder("__force_exception", true)
            .Param(PrimitiveType.Byte)
            .Void();
        private static readonly IntrinsicProcedure raise_intrinsic = IntrinsicBuilder.SideEffect("__raise_interrupt")
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure rti_intrinsic = IntrinsicBuilder.SideEffect("__return_from_interrupt")
            .Void();
        private static readonly IntrinsicProcedure rtx_intrinsic = IntrinsicBuilder.SideEffect("__return_from_exception")
            .Void();
        private static readonly IntrinsicProcedure ssync_intrinsic = new IntrinsicBuilder("__system_synchronize", true)
            .Void();
        private static readonly IntrinsicProcedure sti_intrinsic = IntrinsicBuilder.SideEffect("__enable_interrupts")
            .Param(PrimitiveType.Word32)
            .Void();
    }
}