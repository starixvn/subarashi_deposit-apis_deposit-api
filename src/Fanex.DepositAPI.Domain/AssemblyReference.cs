using System.Reflection;

namespace Fanex.DepositAPI.Domain;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}