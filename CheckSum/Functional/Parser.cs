using System.Collections.Generic;
using System.Windows.Documents;

namespace CheckSumServer.Functional
{
    public static class Parser<T,N>
    {
        public static string GetListForMessage(List<T> lst)
        {
            string str = "{ ";
            foreach (var cq in lst)
            {
                str += cq.ToString() + ",";
            }
            str = str.Remove(str.Length - 1);
            str += " }";
            return str;
        }

        public static string GetListForMessage(Dictionary<T,N> dcn)
        {
            string str = "{ ";
            foreach (var cq in dcn)
            {
                str += cq.Key.ToString()+":"+cq.ToString() + " ; ";
            }
            str = str.Remove(str.Length - 1);
            str += " }";
            return str;
        }
    }
}
