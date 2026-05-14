using System;
using Game339.Shared.DependencyInjection;

public class AttackService
{
    /// <summary>
    /// One hit of an attack from attacker to target.
    /// Damage = attacker.Attack.Value, with Weakness on attacker reducing it by 25%.
    /// Damage flows through DealDamage (vulnerability + block + HP).
    /// If target had Thorns, attacker takes thorns damage via DealUnscaledDamage
    /// (block still absorbs, but vulnerability/weakness do not scale it).
    /// </summary>
    public void Attack(ICharacter attacker, ICharacter target)
    {
        int dmg = attacker.Attack.Value;
        if (attacker.WeaknessDuration.Value > 0)
        {
            dmg = (int)(dmg * 0.75f);
        }
        DealDamage(target, dmg);

        if (target.Thorns.Value > 0)
        {
            DealUnscaledDamage(attacker, target.Thorns.Value);
        }
    }

    /// <summary>
    /// Deal damage to target, respecting Vulnerability (+50%) and Block (absorbs first).
    /// Use for non-attack damage sources that should still respect target-side scaling —
    /// e.g. the storm tick, environmental hazards. For damage that should ignore scaling
    /// (like thorns retaliation), use DealUnscaledDamage instead.
    /// </summary>
    public void DealDamage(ICharacter target, int rawDamage)
    {
        if (rawDamage < 0) rawDamage = 0;

        int dmg = rawDamage;
        if (target.VulnerabilityDuration.Value > 0)
        {
            dmg = (int)(dmg * 1.5f);
        }

        ApplyWithBlock(target, dmg);
    }

    /// <summary>
    /// Deal damage to target, respecting Block but ignoring Vulnerability scaling.
    /// Used for thorns retaliation — it's a fixed reflection, not an attack, so it shouldn't
    /// be amplified by vulnerability on the receiver or reduced by weakness on the source.
    /// </summary>
    public void DealUnscaledDamage(ICharacter target, int rawDamage)
    {
        if (rawDamage < 0) rawDamage = 0;
        ApplyWithBlock(target, rawDamage);
    }

    private static void ApplyWithBlock(ICharacter target, int dmg)
    {
        int blocked = Math.Min(target.Block.Value, dmg);
        target.Block.Value -= blocked;
        target.ApplyDamage(dmg - blocked);
    }

    public void Heal(ICharacter healer)
    {
        int healed = healer.HP.Value + 15;
        if (healed > healer.MaxHP.Value) healed = healer.MaxHP.Value;
        healer.HP.Value = healed;
    }
}