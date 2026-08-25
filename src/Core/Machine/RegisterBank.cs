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

using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Core.Machine;

/// <summary>
/// Represents the registers of a machine architecture. This class is used to 
/// query registers based on their name or their <see cref="StorageDomain"/>s.
/// </summary>
public class RegisterBank
{
    private readonly IDictionary<StorageDomain, List<RegisterStorage>> byDomain;
    private readonly IDictionary<string, RegisterStorage> byName;

    /// <summary>
    /// Constructs a <see cref="RegisterBank"/> from the given collection of registers.
    /// </summary>
    /// <param name="registers">Registers to add to the register bank.</param>
    public RegisterBank(IEnumerable<RegisterStorage?> registers)
    {
        var dups = registers
            .Where(r => r is not null)
            .GroupBy(r => r!.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();
        if (dups.Length > 0)
            throw new ArgumentException($"Duplicate register names: {string.Join(", ", dups)}");

        this.byName = registers.Where(r => r is not null).ToDictionary(r => r!.Name, r => r!, StringComparer.OrdinalIgnoreCase);
        this.byDomain = GroupByDomain(registers);
    }

    /// <summary>
    /// Constructs a <see cref="RegisterBank"/> from the given collection of registers,
    /// along with aliases for the registers (like e.g. MIPS, Risc-V).
    /// </summary>
    /// <param name="registers">Registers to add to the register bank.</param>
    /// <param name="aliases">Aliases for the registers.</param>
    public RegisterBank(
        IEnumerable<RegisterStorage?> registers, 
        Dictionary<string, RegisterStorage> aliases)
        : this(registers)
    {
        foreach (var (name, reg) in aliases)
        {
            if (name is null)
                continue;
            if (!byName.ContainsKey(name))
                byName.Add(name, reg);
        }
    }

    private static Dictionary<StorageDomain, List<RegisterStorage>> GroupByDomain(IEnumerable<RegisterStorage?> registers)
    {
        // Group the registers by their storage domain,
        // and within each domain maintain a list of registers within that domain,
        // order by increasing size. Lookups will iterate throug the list
        // until a register of sufficient size is found.
        var byDomain =
            from h in registers.Where(r => r is not null) 
            group h by h.Domain into g
            select KeyValuePair.Create(
                g.Key,
                g
                    .Where(r => r is not null)
                    .OrderBy(r => r.BitSize)
                    .ThenBy(r => r.BitAddress)
                    .ToList());
        return byDomain.ToDictionary(k => k.Key, v => v.Value);
    }

    /// <summary>
    /// Given a <see cref="StorageDomain"/>, find the larget register that covers
    /// the given <see cref="BitRange"/>. If no register covers the range, return null.
    /// </summary>
    /// <param name="domain">Storage domain the register belongs to.</param>
    /// <param name="range">The bit range that must be covered by the register.</param>
    /// <returns>The largest register that covers the specified bit range;
    /// or null if no such register exists.</returns>
    public RegisterStorage? GetRegister(StorageDomain domain, BitRange range)
    {
        if (range.IsEmpty)
            return null;
        if (!this.byDomain.TryGetValue(domain, out var regs))
            return null;
        foreach (var reg in regs)
        {
            if (reg.Covers(range))
            {
                return reg;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the register whose name is <paramref name="name"/>. The register
    /// is assumed to exist; if it does not, an exception is thrown.
    /// </summary>
    /// <remarks>Use the <see cref="TryGetRegister"/> method if there is a chance 
    /// the register might not exist (e.g. user input).
    /// </remarks>
    /// <param name="name">The name of the register</param>
    /// <returns>A <see cref="RegisterStorage"/> representing the register.
    /// </returns>
    public RegisterStorage GetRegister(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        return this.byName.TryGetValue(name, out var reg)
            ? reg
            : throw new KeyNotFoundException($"Register {name} not found.");
    }

    /// <summary>
    /// Returns all registers of this architecture in no particular order. 
    /// The returned array is a copy of the internal register list.
    /// </summary>
    /// <returns>An unordered array of <see cref="RegisterStorage"/>s.</returns>
    public RegisterStorage[] GetRegisters()
    {
        return this.byName.Values.ToArray();
    }

    /// <summary>
    /// Attempts to find a register with name <paramref>name</paramref>
    /// </summary>
    /// <param name="name">The name of the register</param>
    /// <param name="reg">The register, if found; otherwise null.</param>
    /// <returns>True if the register was found; otherwise false.</returns>
    public bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        return this.byName.TryGetValue(name, out reg);
    }

    /// <summary>
    /// Retrieves the registers whose domains span the half-open interval
    /// <paramref name="startDomain"/> to <paramref name="endDomain"/>.
    /// </summary>
    /// <param name="startDomain">The domain of the first register to retrieve.</param>
    /// <param name="endDomain">The domain of the first register to not retrieve.</param>
    /// <returns>An enumerable of <see cref="RegisterStorage"/>, sorted by storage domain.
    /// </returns>
    public IEnumerable<RegisterStorage> GetRegistersByDomain(int startDomain, int endDomain)
    {
        return this.byDomain
            .Where(r => r.Value.Count > 0 &&
                        startDomain <= (int) r.Key &&
                        (int) r.Key < endDomain)
            .OrderBy(r => r.Key)
            .Select(r => r.Value[0]);
    }

    /// <summary>
    /// Retrieves the widest register whose domain is <paramref name="storageDomain"/>.
    /// </summary>
    /// <remarks>
    /// Assumes the domain exists and contains at least one register. If not,
    /// an exception is thrown.
    /// </remarks>
    /// <param name="storageDomain">The storage domain of the register.</param>
    /// <returns>The widest register in the specified domain.</returns>
    public RegisterStorage GetRegisterByDomain(StorageDomain storageDomain)
    {
        if (!this.byDomain.TryGetValue(storageDomain, out var regs) || regs.Count == 0)
            throw new KeyNotFoundException($"No registers found for domain {storageDomain}.");
        return regs[^1];
    }
}
