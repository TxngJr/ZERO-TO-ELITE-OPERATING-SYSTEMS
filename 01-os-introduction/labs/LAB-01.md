# Lab 01 — C# to Linux Kernel

~~~bash
dotnet build 01-os-introduction/examples/Chapter01.csproj
dotnet run --project 01-os-introduction/examples/Chapter01.csproj
dotnet run --project 01-os-introduction/examples/Chapter01.csproj -- raw-write
~~~

Trace:

~~~bash
strace -e trace=write dotnet run --project 01-os-introduction/examples/Chapter01.csproj
~~~

## Exercise

แก้ Program.cs ให้มี mode read-proc ที่อ่าน /proc/self/status ด้วย File.ReadAllLines()

ใช้ C# เท่านั้น
