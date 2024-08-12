using System.Reflection;

namespace Fanex.DepositAPI.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}