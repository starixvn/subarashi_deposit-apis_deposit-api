using System.Reflection;

namespace Fanex.DepositAPI.Infrastructure;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}