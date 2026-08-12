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
using Reko.Core;
using Reko.Core.Machine;
using System.Linq;

namespace Reko.UnitTests.Core.Machine;

[TestFixture]
public class RegisterBankTests
{
    private readonly RegisterBank bank;

    public RegisterBankTests()
    {
        RegisterStorage[] regs = [
            RegisterStorage.Reg64("rax", 0),
            RegisterStorage.Reg64("rcx", 1),
            RegisterStorage.Reg64("rdx", 2),
            RegisterStorage.Reg64("rbx", 3),
            // Intentionally leave a gap for rbp and rsp
            RegisterStorage.Reg64("rsi", 6),
            RegisterStorage.Reg64("rdi", 7),
        ];

        RegisterStorage[] regs32 = [
            RegisterStorage.Reg32("eax", 0),
            RegisterStorage.Reg32("ecx", 1),
            RegisterStorage.Reg32("edx", 2),
            RegisterStorage.Reg32("ebx", 3),
            // Intentionally leave a gap for ebp and esp
            RegisterStorage.Reg32("esi", 6),
            RegisterStorage.Reg32("edi", 7),
        ];

        RegisterStorage[] reg16 = [
            RegisterStorage.Reg16("ax", 0),
            RegisterStorage.Reg16("cx", 1),
            RegisterStorage.Reg16("dx", 2),
            RegisterStorage.Reg16("bx", 3),
            // Intentionally leave a gap for bp and sp
            RegisterStorage.Reg16("si", 6),
            RegisterStorage.Reg16("di", 7),
        ];

        RegisterStorage[] reg8 = [
            RegisterStorage.Reg8("al", 0),
            RegisterStorage.Reg8("cl", 1),
            RegisterStorage.Reg8("dl", 2),
            RegisterStorage.Reg8("bl", 3),
            RegisterStorage.Reg8("ah", 0, 8),
            RegisterStorage.Reg8("ch", 1, 8),
            RegisterStorage.Reg8("dh", 2, 8),
            RegisterStorage.Reg8("bh", 3, 8),
        ];

        this.bank = new RegisterBank(
            regs.Concat(regs32).Concat(reg16).Concat(reg8).ToArray());
    }

    [Test]
    public void Rb_MissingRegister()
    {
        var reg = bank.GetRegister((StorageDomain) 14, new(0, 8));
        Assert.That(reg, Is.Null);
    }

    [Test]
    public void Rb_InvalidBitrange()
    {
        var reg = bank.GetRegister((StorageDomain) 0, new(64, 72));
        Assert.That(reg, Is.Null);
    }


    [Test]
    public void Rb_Loworder8Bits()
    {
        var reg = bank.GetRegister((StorageDomain) 0, new(0, 6));
        Assert.That(reg.Name, Is.EqualTo("al"));
    }

    [Test]
    public void Rb_Highorder8Bits()
    {
        var reg = bank.GetRegister((StorageDomain) 0, new(9, 14));
        Assert.That(reg.Name, Is.EqualTo("ah"));
    }

    [Test]
    public void Rb_16bits()
    {
        var reg = bank.GetRegister((StorageDomain) 3, new(2, 14));
        Assert.That(reg.Name, Is.EqualTo("bx"));
    }
}
