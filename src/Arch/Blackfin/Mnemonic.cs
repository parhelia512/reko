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

namespace Reko.Arch.Blackfin
{
    public enum Mnemonic
    {
        invalid,

        ABORT,
        CALL,
        CLI,
        CSYNC,
        DIVQ,
        DIVS,
        EMUEXCEPT,
        EXCPT,
        IDLE,
        JUMP,
        JUMP_L,
        JUMP_S,
        LINK,
        MNOP,
        NOP,
        RAISE,
        RTE,
        RTI,
        RTN,
        RTS,
        RTX,
        SSYNC,
        STI,
        UNLINK,
        add,
        add3,
        add_sh1,
        add_sh2,
        and3,
        asr,
        asr3,
        bitclr,
        bitset,
        bittgl,
        flush,
        flushinv,
        if_cc_jump,
        if_cc_jump_bp,
        if_cc_mov,
        if_ncc_jump,
        if_ncc_jump_bp,
        if_ncc_mov,
        iflush,
        lsl,
        lsl3,
        lsr,
        lsr3,
        mov,
        mov_cc,
        mov_cc_bittest,
        mov_cc_eq,
        mov_cc_le,
        mov_cc_lt,
        mov_cc_n_bittest,
        mov_cc_ule,
        mov_cc_ult,
        mov_post,
        mov_pre,
        mov_r_cc,
        mov_x,
        mov_xb,
        mov_xl,
        mov_z,
        mov_zb,
        mov_zl,
        mul,
        neg,
        neg_cc,
        not,
        or3,
        prefetch,
        shift1add,
        shift2add,
        sub,
        sub3,
        xor3,
    }
}