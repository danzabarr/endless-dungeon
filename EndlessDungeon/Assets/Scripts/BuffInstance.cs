using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffInstance 
{
    private Buff buff;
    private ParticleSystem particles;
    private float tickTimer, tickInterval;
    public float[] floatVariables;
    public int[] intVariables;

    public Unit Caster { get; private set; }
    public Unit Target { get; private set; }
    public float SecondsElapsed { get; private set; }
    public int Stacks { get; private set; }
    public bool Finished { get; private set; }
    public bool Permanent => buff.Permanent;
    public float SecondsMax => buff.Duration;
    public float SecondsRemaining => buff.Permanent ? 0 : buff.Duration - SecondsElapsed;
    public float FractionElapsed => SecondsElapsed / buff.Duration;
    public Texture2D Icon => buff.Icon;
    public Buff Buff => buff;
    
    public BuffInstance(Unit caster, Unit target, Buff buff, int stacks = 1)
    {
        this.buff = buff;
        Caster = caster;
        Target = target;
        Stacks = stacks;

        //TODO figure out the timing here based on the buff and the caster haste?
        if (buff.Particles)
            particles = Object.Instantiate(buff.Particles, target.GetGameObject().transform);
        tickInterval = buff.TickInterval;
    }
    public bool IsMatchToOverwrite(Unit caster, Unit target, Buff buff)
    {
        switch (buff.AllowDuplicates)
        {
            case Buff.Duplicates.AnyAmount:
                return false;

            case Buff.Duplicates.OnePerTarget:
                return Target == target && this.buff == buff;

            case Buff.Duplicates.OnePerCaster:
                return Caster == caster && Target == target && this.buff == buff;

            case Buff.Duplicates.OnePerCasterPerTarget:
                return Caster == caster && Target == target && this.buff == buff;

            default:
                return false;
        }
    }

    /// <summary>
    /// Resets the timer and adds a stack up to a max.
    /// </summary>
    public void AddStack(int stacks = 1)
    {
        if (Finished)
            return;

        if (buff.ResetTimerOnAddStack)
            Refresh();

        int stacksToAdd = Mathf.Clamp(Stacks + stacks, 1, buff.MaxStacks) - Stacks;

        if (stacksToAdd <= 0)
            return;

        Stacks += stacksToAdd;

        buff.OnAddStack(Caster, Target, Stacks, stacksToAdd, SecondsElapsed);
    }
    /// <summary>
    /// Remove one stack of the buff. If it is the last stack, the buff is removed completely. Does not reset the timer.
    /// </summary>
    /// <param name="remover"></param>
    public void RemoveStack(Unit remover, int stacks = 1)
    {
        if (Finished)
            return;

        int stacksToRemove = Stacks - Mathf.Clamp(Stacks - stacks, 1, buff.MaxStacks) ;
        if (stacksToRemove <= 0)
            return;

        Stacks -= stacksToRemove;
        
        buff.OnRemoveStack(remover, Caster, Target, Stacks, stacksToRemove, SecondsElapsed);
        if (Stacks <= 0)
        {
            if (particles) Object.Destroy(particles.gameObject);
            Finished = true;
        }
    }
    /// <summary>
    /// Resets the timer without adding additional stacks.
    /// </summary>
    public void Refresh()
    {
        if (Finished)
            return;
        SecondsElapsed = 0;
        buff.OnRefresh(Caster, Target, Stacks, SecondsElapsed);
    }
    /// <summary>
    /// Removes the buff completely.
    /// </summary>
    /// <param name="dispeller"></param>
    public void Dispel(Unit dispeller)
    {
        if (Finished)
            return;
        buff.OnDispel(dispeller, Caster, Target, Stacks, SecondsElapsed);
        if (particles) particles.Stop();
        Finished = true;
    }
    private void Expire()
    {
        if (Finished)
            return;
        buff.OnExpire(Caster, Target, Stacks, SecondsElapsed);
        if (particles) particles.Stop();
        Finished = true;
    }

    private void Tick()
    {
        if (Finished)
            return;
        buff.OnTick(Caster, Target, Stacks, SecondsElapsed);
    }

    public void UpdateTimer()
    {
        if (Finished)
            return;

        if (particles)
            particles.transform.position = Target.GetCenterPosition();

        SecondsElapsed += Time.deltaTime;

        if (tickInterval > 0)
        {
            tickTimer += Time.deltaTime;
            while (tickTimer >= tickInterval)
            {
                tickTimer -= tickInterval;
                Tick();
            }
        }

        if (SecondsElapsed >= buff.Duration)
        {
            if (buff.RemoveStackOnExpire && Stacks > 1)
            {
                RemoveStack(null, 1);
                SecondsElapsed = 0;
            }
            else
                Expire();
            return;
        }
    }
}
