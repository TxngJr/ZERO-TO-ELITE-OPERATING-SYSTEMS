# Lab 03 — C# Child Process Lifecycle

~~~bash
dotnet run --project 03-process-context-II/examples/Chapter03.csproj
~~~

Trace:

~~~bash
strace -f -e trace=process dotnet run --project 03-process-context-II/examples/Chapter03.csproj
~~~

## C# Exercise

แก้ Program.cs ให้:

- parent สร้าง child 5 ตัว
- child Sleep ไม่เท่ากัน
- parent print completion order
- ใช้ WaitForExit
- ห้ามให้ parent ใช้ Thread.Sleep เพื่อเดาว่า child ตัวไหนจบ
