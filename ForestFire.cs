using System;
using System.Collections.Generic;
using System.IO;

namespace forestfire;

public class ForestFire
{
    public int Count { get; private set; }
    public int ActiveCount { get; private set; }

    public ForestFire? ParentFire { get; private set; }

    public List<ForestFire> ChildFires { get; } = new();

    public void Report()
    {
        if (!(ActiveCount <= 0) || ChildFires.Count > 0) return;

        if (ParentFire != null)
        {
            ParentFire.Increment(Count);
            ParentFire.RemoveChildFire(this);
            ParentFire = null;
        }
        FireTracker.INSTANCE.Log(Count);

    }

    public bool IsRelated(ForestFire other)
    {
        ForestFire? current = this;
        while (current != null)
        {
            if (current == other) return true;
            current = current.ParentFire;
        }

        current = other;
        while (current != null)
        {
            if (current == this) return true;
            current = current.ParentFire;
        }

        return false;
    }

    public void Increment(int amount = 1)
    {
        Count += amount;
    }

    public void IncrementActive()
    {
        ActiveCount++;
    }

    public void DecrementActive()
    {
        ActiveCount--;
        Report();
    }

    public ForestFire? GetRootFire()
    {
        ForestFire? current = this;
        while (current?.ParentFire != null)
        {
            current = current.ParentFire;
        }
        return current;
    }

    public void setParentFire(ForestFire? parent)
    {
        if (ParentFire != null) return;
        ParentFire = parent;
    }

    public void AddChildFire(ForestFire child)
    {
        if (IsRelated(child)) return;
        child.setParentFire(this);
        ChildFires.Add(child);
    }

    public void RemoveChildFire(ForestFire child)
    {
        ChildFires.Remove(child);
        child.ParentFire = null;
        Report();
    }
}
