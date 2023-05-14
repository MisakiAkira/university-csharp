using LinqTasks.Extensions;
using LinqTasks.Models;
using System.Linq;

namespace LinqTasks;

public static partial class Tasks
{
    public static IEnumerable<Emp> Emps { get; set; }
    public static IEnumerable<Dept> Depts { get; set; }

    static Tasks()
    {
        Depts = LoadDepts();
        Emps = LoadEmps();
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Job = "Backend programmer";
    /// </summary>
    public static IEnumerable<Emp> Task1()
    {
        IEnumerable<Emp> results = Emps.Where(x => x.Job == "Backend programmer");
        return results;
    }

    /// <summary>
    ///     SELECT * FROM Emps Job = "Frontend programmer" AND Salary>1000 ORDER BY Ename DESC;
    /// </summary>
    public static IEnumerable<Emp> Task2()
    {
        IEnumerable<Emp> results = Emps.Where(x => x.Job == "Frontend programmer" && x.Salary > 1000).OrderByDescending(x => x.Ename);
        return results;
    }


    /// <summary>
    ///     SELECT MAX(Salary) FROM Emps;
    /// </summary>
    public static int Task3()
    {
        int result = Emps.Max(x => x.Salary);
        return result;
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Salary=(SELECT MAX(Salary) FROM Emps);
    /// </summary>
    public static IEnumerable<Emp> Task4()
    {
        IEnumerable<Emp> results = Emps.Where(x => x.Salary == Task3());
        return results;
    }

    /// <summary>
    ///    SELECT ename AS Nazwisko, job AS Praca FROM Emps;
    /// </summary>
    public static IEnumerable<object> Task5()
    {
        IEnumerable<object> results = Emps.Select(emp => new {Nazwisko=emp.Ename,Praca=emp.Job});
        return results;
    }

    /// <summary>
    ///     SELECT Emps.Ename, Emps.Job, Depts.Dname FROM Emps
    ///     INNER JOIN Depts ON Emps.Deptno=Depts.Deptno
    ///     Rezultat: Złączenie kolekcji Emps i Depts.
    /// </summary>
    public static IEnumerable<object> Task6()
    {
        IEnumerable<object> results = Emps.Join(Depts, emp => emp.Deptno, dept => dept.Deptno, (emp, dept) => new { emp.Ename, emp.Job, dept.Dname });
        return results;
    }

    /// <summary>
    ///     SELECT Job AS Praca, COUNT(1) LiczbaPracownikow FROM Emps GROUP BY Job;
    /// </summary>
    public static IEnumerable<object> Task7()
    {
        IEnumerable<object> results = Emps.GroupBy(emp => emp.Job).Select(emp => new { Praca = emp.Key, LiczbaPracownikow = emp.Count() });
        return results;
    }

    /// <summary>
    ///     Zwróć wartość "true" jeśli choć jeden
    ///     z elementów kolekcji pracuje jako "Backend programmer".
    /// </summary>
    public static bool Task8()
    {
        bool result = Emps.Count(emp => emp.Job.Equals("Backend programmer")) > 0;
        return result;
    }

    /// <summary>
    ///     SELECT TOP 1 * FROM Emp WHERE Job="Frontend programmer"
    ///     ORDER BY HireDate DESC;
    /// </summary>
    public static Emp Task9()
    {
        IEnumerable<Emp> tmp = Emps.Where(emp=>emp.Job.Equals("Frontend programmer")).OrderByDescending(emp=>emp.HireDate);
        return tmp.ElementAt(0);
    }

    /// <summary>
    ///     SELECT Ename, Job, Hiredate FROM Emps
    ///     UNION
    ///     SELECT "Brak wartości", null, null;
    /// </summary>
    public static IEnumerable<object> Task10()
    {
        IEnumerable<object> results = Emps.Select(emp => new {emp.Ename,emp.Job,emp.HireDate}).Union(
                Emps.Select(emp => new {Ename="Brak wartości",Job=(string)null,HireDate=(DateTime?)null})
            );
        return results;
    }

    /// <summary>
    ///     Wykorzystując LINQ pobierz pracowników podzielony na departamenty pamiętając, że:
    ///     1. Interesują nas tylko departamenty z liczbą pracowników powyżej 1
    ///     2. Chcemy zwrócić listę obiektów o następującej srukturze:
    ///     [
    ///     {name: "RESEARCH", numOfEmployees: 3},
    ///     {name: "SALES", numOfEmployees: 5},
    ///     ...
    ///     ]
    ///     3. Wykorzystaj typy anonimowe
    /// </summary>
    public static IEnumerable<object> Task11()
    {
        //IEnumerable<object> results = Depts.Select(x => new { name = x.Dname, numOfEmployees = x });
        IEnumerable<object> results = Depts.Join(Emps, dept => dept.Deptno, emp => emp.Deptno, (dept, emp) => new { dept.Dname, emp })
            .GroupBy(dept => dept.Dname).Select(x => new { name = x.Key, numOfEmployees = x.Count() });
        return results;
    }

    /// <summary>
    ///     Napisz własną metodę rozszerzeń, która pozwoli skompilować się poniższemu fragmentowi kodu.
    ///     Metodę dodaj do klasy CustomExtensionMethods, która zdefiniowana jest poniżej.
    ///     Metoda powinna zwrócić tylko tych pracowników, którzy mają min. 1 bezpośredniego podwładnego.
    ///     Pracownicy powinny w ramach kolekcji być posortowani po nazwisku (rosnąco) i pensji (malejąco).
    /// </summary>
    public static IEnumerable<Emp> Task12()
    {
        IEnumerable<Emp> result = Emps.GetEmpsWithSubordinates();
        
        return result;
    }

    /// <summary>
    ///     Poniższa metoda powinna zwracać pojedyczną liczbę int.
    ///     Na wejściu przyjmujemy listę liczb całkowitych.
    ///     Spróbuj z pomocą LINQ'a odnaleźć tę liczbę, które występuja w tablicy int'ów nieparzystą liczbę razy.
    ///     Zakładamy, że zawsze będzie jedna taka liczba.
    ///     Np: {1,1,1,1,1,1,10,1,1,1,1} => 10
    /// </summary>
    public static int Task13(int[] arr)
    {
        IEnumerable<int> result = arr.GroupBy(x => x).Select(x=> new {x.Key, Value = x.Count() }).Where(x => x.Value % 2 != 0).Select(x=>x.Key);
        return result.ElementAt(0);
    }

    /// <summary>
    ///     Zwróć tylko te departamenty, które mają 5 pracowników lub nie mają pracowników w ogóle.
    ///     Posortuj rezultat po nazwie departament rosnąco.
    /// </summary>
    public static IEnumerable<Dept> Task14()
    {
        IEnumerable<Dept> result = Depts
            .Join(Emps, dept => dept.Deptno, emp => emp.Deptno, (dept, emp) => new {dept, emp})
            .GroupBy(x=>x.dept).Select(x => new { x.Key, value = x.Count()}).Where(x=>x.value != 5 || x.value != 0).Select(x=>x.Key).OrderBy(x=>x.Dname);
        return Depts.Except(result);
    }
}