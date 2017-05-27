using ObjMapping.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjMapping.consts
{
    internal static class Consts
    {
        private static readonly Type[] SMPL_TYPES = new Type[]{
            typeof(string),

            typeof(sbyte),typeof(sbyte?),
            typeof(short),typeof(short?),
            typeof(int),typeof(int?),
            typeof(long),typeof(long?),
            typeof(byte),typeof(byte?),
            typeof(ushort),typeof(ushort?),
            typeof(uint),typeof(uint?),
            typeof(ulong),typeof(ulong?),

            typeof(float),typeof(float?),
            typeof(double),typeof(double?),
            typeof(decimal),typeof(decimal?),

            typeof(bool),typeof(bool?),
            typeof(char),typeof(char?),

            typeof(DateTime),typeof(DateTime?),
            typeof(Guid),typeof(Guid?),
        };

        public static MemberType Convert2MemberType(Type type)
        {
            if (type != null)
            {
                if (type.IsEnum || SMPL_TYPES.Any(t => t == type)) return MemberType.Simple;
                if (type.IsClass && type.IsGenericType) return MemberType.Generic;
                if (type.IsArray) return MemberType.Array;
                return MemberType.Complex;
            }
            return MemberType.Unknown;
        }
    }
}
