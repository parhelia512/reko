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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Elf;

/*
public enum ElfMipsFlags : uint
{
    EF_MIPS_NOREORDER = 0x00000001,
    EF_MIPS_PIC = 0x00000002,
    EF_MIPS_CPIC = 0x00000004,
    EF_MIPS_UCODE = 0x00000008,
    EF_MIPS_ABI2 = 0x00000020,
    EF_MIPS_OPTIONS_FIRST = 0x00000080,
    EF_MIPS_32BITMODE = 0x00000100,
    EF_MIPS_FP64 = 0x00000200,
    EF_MIPS_NAN2008 = 0x00000400,
    EF_MIPS_ABI_ON32 = 0x00000800,

    EF_MIPS_ABI = 0x0000F000,
    EF_MIPS_ABI_O32 = 0x00001000,
    EF_MIPS_ABI_O64 = 0x00002000,
    EF_MIPS_ABI_EABI32 = 0x00003000,
    EF_MIPS_ABI_EABI64 = 0x00004000,

    EF_MIPS_ARCH_ASE_MDMX = 0x00001000,
    EF_MIPS_ARCH_ASE_MICROMIPS = 0x00002000,
    EF_MIPS_ARCH_ASE_DSP = 0x00004000,
    EF_MIPS_ARCH_ASE_DSP2 = 0x00008000,

    EF_MIPS_ARCH = 0xF0000000,
    EF_MIPS_ARCH_64 = 0x60000000,
    EF_MIPS_ARCH_64R2 = 0x80000000,
    EF_MIPS_ARCH_64R6 = 0xA0000000,

    EF_MIPS_MACH =      0x00FF0000,
    EF_MIPS_MACH_5900 = 0x00920000
}
*/
