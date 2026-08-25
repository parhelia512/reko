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
using Reko.Core.Types;

namespace Reko.Arch.Mips;

public partial class MipsRewriter
{
    private void AssignS(Expression dst, Expression src)
    {
        m.Assign(dst, m.MaybeExtendS(src, dst.DataType));
    }

    private void RewriteMf0(MipsInstruction instr)
    {
        AssignS(
            RewriteOperand(instr, 0),
            m.Fn(intrinsics.mf0));
    }

    private void RewritePcpyh(MipsInstruction instr)
    {
        m.Assign(
            RewriteOperand(instr, 0),
            m.Fn(
                intrinsics.pcpyh,
                RewriteOperand0(instr, 1)));
    }

    private void RewritePcpyld(MipsInstruction instr)
    {
        m.Assign(
            RewriteOperand(instr, 0),
            m.Fn(
                intrinsics.pcpyld,
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }

    private void RewritePcpyud(MipsInstruction instr)
    {
        m.Assign(
            RewriteOperand(instr, 0),
            m.Fn(
                intrinsics.pcpyud,
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }

    private void RewriteParallelBinary(MipsInstruction instr, IntrinsicProcedure fn)
    {
        m.Assign(
            RewriteOperand(instr, 0),
            m.Fn(
                fn,
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }

    private void RewriteSimd(
        MipsInstruction instr,
        IntrinsicProcedure simd, 
        PrimitiveType elementType)
    {
        m.Assign(
            RewriteOperand(instr, 0),
            m.Fn(
                simd.MakeInstance(elementType),
                RewriteOperand0(instr, 1),
                RewriteOperand0(instr, 2)));
    }
}
