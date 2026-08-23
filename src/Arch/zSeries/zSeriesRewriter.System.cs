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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.zSeries
{
    public partial class zSeriesRewriter
    {
        private void RewriteBpp()
        {
            m.SideEffect(m.Fn(
                intrinsics.bpp.MakeInstance(arch.WordWidth),
                Op(0, arch.WordWidth),
                Op(1, arch.WordWidth),
                Op(2, arch.WordWidth)));
        }


        private void RewriteBprp()
        {
            m.SideEffect(m.Fn(
                intrinsics.bprp.MakeInstance(arch.WordWidth),
                Op(0, arch.WordWidth),
                Op(1, arch.WordWidth),
                Op(2, arch.WordWidth)));
        }

        private void RewriteEar()
        {
            var r2 = (RegisterStorage) instr.Operands[1];
            var tmp = binder.CreateTemporary(PrimitiveType.Word32);
            m.Assign(tmp, m.Fn(intrinsics.ear, m.Word32(r2.Number)));
            var dst = Reg(0, PrimitiveType.Word64);
            m.Assign(dst, m.Dpb(dst, tmp, 0));
        }

        private void RewriteEx()
        {
            var op0 = Reg(0);
            SetCc(m.Fn(intrinsics.execute.MakeInstance(op0.DataType), op0, Op(1, arch.WordWidth)));
        }

        private void RewriteIpm()
        {
            var op = Reg(0);
            Assign(op, m.Fn(intrinsics.ipm, op));
        }


        private void RewriteLctl()
        {
            var op1 = Op(0, PrimitiveType.Word64);
            var op2 = Op(1, PrimitiveType.Word64);
            var ea = EffectiveAddress(2);
            m.SideEffect(m.Fn(intrinsics.lctl, op1, op2, ea));
        }

        private void RewriteLra()
        {
            var r = Reg(0);
            SetCc(m.Fn(
                intrinsics.lra.MakeInstance(ptrSize, r.DataType),
                EffectiveAddress(1),
                m.Out(r.DataType, r)));
        }

        private void RewritePr()
        {
            //$REVIEW: is this correct?
            m.SideEffect(m.Fn(intrinsics.pr));
            m.Return(0, 0);
        }

        private void RewriteStctl(PrimitiveType dt) {
            var op1 = Reg(0);
            var op2 = m.AddrOf(new PointerType(dt, arch.PointerType.BitSize), m.Mem(dt, EffectiveAddress(1)));
            m.SideEffect(m.Fn(
                intrinsics.stctl,
                op1,
                op2));
        }
    }
}
