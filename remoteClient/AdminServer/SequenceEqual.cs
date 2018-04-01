using System.Collections.Generic;

namespace AdminServer
{
    public static class SequenceEqual
    {
        public static bool ListEquals<T>(this List<T> target, List<T> source) where T : struct
        {
            if (target.Count != source.Count)
                return false;
            else
            {
                for (int i = 0; i < target.Count; i++)
                {
                    if (!target[i].Equals(source[i]))
                        return false;
                }
                return true;
            }
        }
    }
}
