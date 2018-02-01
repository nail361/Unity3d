
public class ProtectedUint{

    private string value;

    public string Value
    {
        set{ this.value = Protector.Encrypt(value); }
        get{ return value; }
    }

	public ProtectedUint() {
        value = Protector.Encrypt("0");
	}

    public ProtectedUint(string value)
    {
        this.value = Protector.Encrypt(value);
    }

    // ++ ; --

    public static ProtectedUint operator ++(ProtectedUint op1)
    {
        ProtectedUint result = new ProtectedUint(op1.GetConverted<string>());
        result += 1;
        return result;
    }

    public static ProtectedUint operator --(ProtectedUint op1)
    {
        ProtectedUint result = new ProtectedUint(op1.GetConverted<string>());

        if (result.GetConverted<uint>() == 0) return op1;
        else
        {
            result -= 1;
            return result;
        }
    }

    //--------------------

    // > ; <

    public static bool operator >(ProtectedUint op1, uint op2)
    {
        return op1.GetConverted<uint>() > op2;
    }

    public static bool operator <(ProtectedUint op1, uint op2)
    {
        return op1.GetConverted<uint>() < op2;
    }

    public static bool operator >(ProtectedUint op1, int op2)
    {
        return op1.GetConverted<uint>() > op2;
    }

    public static bool operator <(ProtectedUint op1, int op2)
    {
        return op1.GetConverted<uint>() < op2;
    }

    public static bool operator >(ProtectedUint op1, ProtectedUint op2)
    {
        return op1.GetConverted<uint>() > op2.GetConverted<uint>();
    }

    public static bool operator <(ProtectedUint op1, ProtectedUint op2)
    {
        return op1.GetConverted<uint>() < op2.GetConverted<uint>();
    }

    //----------------------


    public override bool Equals(object obj)
    {
        var item = obj as ProtectedUint;

        if (item == null)
        {
            return false;
        }

        return GetConverted<uint>().Equals(item.GetConverted<uint>());
    }

    public override int GetHashCode()
    {
        return GetConverted<uint>().GetHashCode();
    }

    public static bool operator ==(ProtectedUint op1, ProtectedUint op2)
    {
        return op1.GetConverted<uint>() == op2.GetConverted<uint>();
    }

    public static bool operator !=(ProtectedUint op1, ProtectedUint op2)
    {
        return op1.GetConverted<uint>() != op2.GetConverted<uint>();
    }

    public static bool operator >=(ProtectedUint op1, ProtectedUint op2)
    {
        return op1.GetConverted<uint>() >= op2.GetConverted<uint>();
    }

    public static bool operator <=(ProtectedUint op1, ProtectedUint op2)
    {
        return op1.GetConverted<uint>() <= op2.GetConverted<uint>();
    }

    public static bool operator >=(ProtectedUint op1, uint op2)
    {
        return op1.GetConverted<uint>() >= op2;
    }

    public static bool operator <=(ProtectedUint op1, uint op2)
    {
        return op1.GetConverted<uint>() <= op2;
    }

    public static bool operator >=(ProtectedUint op1, int op2)
    {
        return op1.GetConverted<uint>() >= op2;
    }

    public static bool operator <=(ProtectedUint op1, int op2)
    {
        return op1.GetConverted<uint>() <= op2;
    }

    public static bool operator ==(ProtectedUint op1, uint op2)
    {
        return op1.GetConverted<uint>() == op2;
    }

    public static bool operator !=(ProtectedUint op1, uint op2)
    {
        return op1.GetConverted<uint>() != op2;
    }

    public static bool operator ==(ProtectedUint op1, int op2)
    {
        return op1.GetConverted<uint>() == op2;
    }

    public static bool operator !=(ProtectedUint op1, int op2)
    {
        return op1.GetConverted<uint>() != op2;
    }

    public static ProtectedUint operator *(ProtectedUint op1, ProtectedUint op2)
    {
        ProtectedUint result = new ProtectedUint();

        result.Value = (op1.GetConverted<uint>() * op2.GetConverted<uint>()).ToString();
        return result;
    }

    public static ProtectedUint operator +(ProtectedUint op1, ProtectedUint op2)
    {
        ProtectedUint result = new ProtectedUint();

        result.Value = (op1.GetConverted<uint>() + op2.GetConverted<uint>()).ToString();
        return result;
    }

    public static ProtectedUint operator +(ProtectedUint op1, uint op2)
    {
        ProtectedUint result = new ProtectedUint();

        result.Value = (op1.GetConverted<uint>() + op2).ToString();
        return result;
    }

    public static ProtectedUint operator +(ProtectedUint op1, int op2)
    {
        return op1 + (uint)op2;
    }

    public static ProtectedUint operator -(ProtectedUint op1, ProtectedUint op2)
    {
        ProtectedUint result = new ProtectedUint();

        if (op2.GetConverted<uint>() > op1.GetConverted<uint>())
        {
            return result;
        }
        else {
            result.Value = (op1.GetConverted<uint>() - op2.GetConverted<uint>()).ToString();
            return result;
        }
    }

    public static ProtectedUint operator -(ProtectedUint op1, uint op2)
    {
        ProtectedUint result = new ProtectedUint();

        if (op2 > op1.GetConverted<uint>())
        {
            return result;
        }
        else
        {
            result.Value = (op1.GetConverted<uint>() - op2).ToString();
            return result;
        }
    }

    public static ProtectedUint operator -(ProtectedUint op1, int op2)
    {
        return op1 - (uint)op2;
    }

    public T GetConverted<T>()
    {
        return Protector.Decrypt<T>(value);
    }

    public void Copy(string value)
    {
        this.value = value;
    }
}
