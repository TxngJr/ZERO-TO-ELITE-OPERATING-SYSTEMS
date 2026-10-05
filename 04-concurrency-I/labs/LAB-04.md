# Lab 04 — Thread and Race Condition in C#

~~~bash
dotnet run --project 04-concurrency-I/examples/Chapter04.csproj
dotnet run --project 04-concurrency-I/examples/Chapter04.csproj -- race
dotnet run --project 04-concurrency-I/examples/Chapter04.csproj -- observe
~~~

## Exercise

ใช้รูปแบบเดียวกับ Activity ที่มี plus/minus:

- shared variable
- Thread P
- Thread M
- Stopwatch
- Start
- Join

ทำ 3 version:

1. sequential
2. threads without lock
3. threads with lock

เปรียบเทียบ correctness และเวลา
