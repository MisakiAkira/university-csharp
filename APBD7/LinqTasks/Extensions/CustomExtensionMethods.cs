using LinqTasks.Models;

namespace LinqTasks.Extensions;

public static class CustomExtensionMethods
{
    //Put your extension methods here
    public static IEnumerable<Emp> GetEmpsWithSubordinates(this IEnumerable<Emp> emps)
    {
        IEnumerable<Emp> result = emps.Select(x => x.Mgr).OfType<Emp>().Distinct().OrderByDescending(x => x.Salary).OrderBy(x => x.Ename);
        return result;
    }
}