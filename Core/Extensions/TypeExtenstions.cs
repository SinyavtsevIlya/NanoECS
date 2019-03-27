using System.Collections.Generic;
using System;
using System.Linq;

public static class TypeExtenstions
{
    public static List<Type> GetChildTypes(this Type parentType)
    {
        return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where parentType.IsAssignableFrom(assemblyType)
                select assemblyType).ToList().ToList();
    }
}
