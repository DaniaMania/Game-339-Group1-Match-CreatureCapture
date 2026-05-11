using System;
using Game339.Shared.DependencyInjection;

public class AttackService
{
    /// <summary>
    /// One hit of an attack from attacker to target.
    /// Damage = attacker.Attack.Value, with Weakness on the attacker reducing it by 25%.
    /// Damage then flows through DealDamage (vulnerability + block + HP).
    /// Multihit is the caller's job — call this method N times for N hits.
    /// </summary>
    public void Attack(ICharacter attacker, ICharacter target)
    {
        int dmg = attacker.Attack.Value;
        if (attacker.WeaknessDuration.Value > 0)
        {
            dmg = (int)(dmg * 0.75f); // -25%
        }
        DealDamage(target, dmg);
    }

    /// <summary>
    /// Deal damage to target, respecting Vulnerability (+50%) and Block (absorbs first).
    /// Use this directly for non-attack damage sources — environmental damage, the storm,
    /// damaging passives like Thorns — that should respect target-side modifiers but don't
    /// involve an attacker (so Weakness doesn't apply).
    /// </summary>
    public void DealDamage(ICharacter target, int rawDamage)
    {
        if (rawDamage < 0) rawDamage = 0;

        int dmg = rawDamage;
        if (target.VulnerabilityDuration.Value > 0)
        {
            dmg = (int)(dmg * 1.5f); // +50%
        }

        int blocked = Math.Min(target.Block.Value, dmg);
        target.Block.Value -= blocked;
        int actualDamage = dmg - blocked;

        target.ApplyDamage(actualDamage);
    }

    public void Heal(ICharacter healer)
    {
        int healed = healer.HP.Value + 15;
        if (healed > healer.MaxHP.Value) healed = healer.MaxHP.Value;
        healer.HP.Value = healed;
    }
}