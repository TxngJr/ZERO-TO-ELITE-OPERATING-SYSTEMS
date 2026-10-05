# Lab 05 — Logical Race and Visibility

~~~bash
dotnet run --project 05-concurrency-II/examples/Chapter05.csproj -- check-then-act
dotnet run --project 05-concurrency-II/examples/Chapter05.csproj -- visibility
dotnet run --project 05-concurrency-II/examples/Chapter05.csproj -- background
~~~

## Exercise

แก้ check-then-act ด้วย:

~~~csharp
lock (lockObj)
{
    if (stock > 0)
    {
        stock--;
    }
}
~~~

แล้วพิสูจน์ว่า final stock ไม่ติดลบ
