namespace DataFuse.Integration.Helpers;

public static class ArrayUtil
{
    public static T[] EnsureAndResizeArray<T>(T[]? val, out int index)
    {
        return EnsureAndResizeArray(val, 1, out index);
    }

    public static T[] EnsureAndResizeArray<T>(T[]? val, int extendBy, out int index)
    {
        if (val is null)
        {
            val = new T[extendBy];
            index = 0;
        }
        else
        {
            var temp = val;
            index = temp.Length;
            Array.Resize(ref temp, temp.Length + extendBy);
            val = temp;
        }

        return val;
    }
}
