using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ObservableInt
{
    public UnityEvent<int> OnChange { get; private set; } = new();
    private int value;
    public int Value
    {
        get => value; set
        {
            this.value = value;
            OnChange.Invoke(value);
        }
    }


    public ObservableInt(int value)
    {
        this.value = value;
        OnChange = new();
    }

    /*public static implicit operator ObservableInt(double value)
    {
        return value;
    }
    public static implicit operator ObservableInt(float value)
    {
        return value;
    }
    public static implicit operator ObservableInt(int value)
    {
        return value;
    }*/

    public override string ToString()
    {
        return value.ToString();
    }

    public static ObservableInt operator +(ObservableInt first, ObservableInt second)
    {
        ObservableInt result = new ObservableInt(first.value + second.value);
        return result;
    }

    public static ObservableInt operator -(ObservableInt first, ObservableInt second)
    {
        return new ObservableInt(first.value - second.value);
    }
    public static ObservableInt operator *(ObservableInt first, ObservableInt second)
    {
        ObservableInt result = new ObservableInt(first.value * second.value);
        return result;
    }

    public static ObservableInt operator /(ObservableInt first, ObservableInt second)
    {
        return new ObservableInt(first.value / second.value);
    }
    public static bool operator >(ObservableInt first, ObservableInt second)
    {
        return first.value > second.value;
    }
    public static bool operator <(ObservableInt first, ObservableInt second)
    {
        return first.value < second.value;
    }
    public static bool operator >=(ObservableInt first, ObservableInt second)
    {
        return first.value >= second.value;
    }
    public static bool operator <=(ObservableInt first, ObservableInt second)
    {
        return first.value <= second.value;
    }
    public static bool operator ==(ObservableInt obj1, ObservableInt obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;
        if (ReferenceEquals(obj1, null))
            return false;
        if (ReferenceEquals(obj2, null))
            return false;
        return obj1.Equals(obj2);
    }
    public static bool operator !=(ObservableInt obj1, ObservableInt obj2) => !(obj1 == obj2);
    public bool Equals(ObservableInt other)
    {
        if (ReferenceEquals(other, null))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return value.Equals(other.value);
    }
    //public override bool Equals(object obj) => Equals(obj as ObservableInt);

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = value.GetHashCode();
            return hashCode;
        }
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

}