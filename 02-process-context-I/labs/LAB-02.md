# Lab 02 — Inspect a C# Process

Run:

~~~bash
dotnet run --project 02-process-context-I/examples/Chapter02.csproj
~~~

ระหว่าง sleep เปิด terminal ใหม่:

~~~bash
ps -o pid,ppid,stat,comm -p PID
cat /proc/PID/status
cat /proc/PID/maps
ls -l /proc/PID/fd
pmap -x PID
~~~

## C# Challenge

สร้าง ProcInspector.cs ที่รับ PID จาก args แล้วอ่าน status/maps ด้วย File API
