using System;

public class AttackService
{
    /// <summary>
    /// Attacker takes away Target health 
    /// </summary>
    /// <param name="attacker">attacking</param>
    /// <param name="target">getting attack</param>
    public void Attack(Character attacker, Character target)
    {
        int dmg = Math.Max(0, attacker.Attack.Value - target.Defense.Value);
        target.ApplyDamage(dmg);
    }

    public void Heal(Character healer)
    {
        int healed = healer.HP.Value + 15;
        if (healed > healer.MaxHP.Value) healed = healer.MaxHP.Value;
        healer.HP.Value = healed;
    }
}           

// int remainingHealth = target.HP.Value - dmg;
// if (remainingHealth <= 0)
// {
//     target.HP.Value = 0;
//     OnTargetDead?.Invoke(target);
//     return;
// }
//
// target.HP.Value = remainingHealth;
// OnTargetTakeDamage?.Invoke(target);