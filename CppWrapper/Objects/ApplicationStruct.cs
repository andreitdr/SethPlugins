using System.Runtime.InteropServices;

namespace CppWrapper.Objects
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)] // can use ANSI also
    public struct ApplicationStruct
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 19)]
        public string ServerId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string Prefix;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 71)]
        public string Token;

        public ApplicationStruct(string serverId, string prefix, string token)
        {
            ServerId = serverId;
            Prefix = prefix;
            Token = token;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ComplexObject
    {
        public int Integer;
        public double DoubleValue;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string strValue;

        public ComplexObject(int integer, double doubleValue, string strValue)
        {
            Integer = integer;
            DoubleValue = doubleValue;
            this.strValue = strValue;
        }
    }
}
