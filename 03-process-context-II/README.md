# Chapter 03 — Process & Context Switch Part II with C#

## Goals

เข้าใจ Context Switch, mode transition, process creation, fork/exec/wait concept, child lifecycle, exit status และ zombie/reaping concept

## Context Switch

~~~text
Task A running
↓
kernel saves required context
↓
scheduler chooses B
↓
restore/switch state
↓
Task B running
~~~

System call transition ไม่ได้แปลว่าต้อง switch ไป process อื่นเสมอ

## fork / exec / wait Concept

ใน POSIX model มักสอน:

~~~text
fork
→ child
→ exec
→ wait
~~~

แต่ C# ใช้ high-level runtime abstraction:

~~~csharp
Process.Start(...)
Process.WaitForExit()
Process.ExitCode
~~~

เราไม่บังคับให้ C# P/Invoke fork เพื่อเลียนแบบ C เพราะ managed runtime มี GC, runtime threads และ internal locks ของตัวเอง

## Process.Start

~~~csharp
using Process child = Process.Start(info)!;
~~~

บน Unix implementation ภายในอาจเปลี่ยนตาม runtime/version ดังนั้นห้ามจำว่า Process.Start เท่ากับ fork() เสมอ

พิสูจน์ด้วย:

~~~bash
strace -f -e trace=process dotnet run --project 03-process-context-II/examples/Chapter03.csproj
~~~

## Wait

~~~csharp
child.WaitForExit();
Console.WriteLine(child.ExitCode);
~~~

นี่เชื่อมกับแนวคิด parent รอ child completion แต่ runtime จัด native details ให้

## Zombie

Zombie คือ child ที่ terminate แล้วแต่ termination information ยังไม่ถูก reap

C# Process API อาจจัด native child lifecycle ผ่าน runtime จึงไม่ใช้ demo ที่พยายามบังคับ zombie แบบไม่ portable

แต่ต้องอ่าน Linux state Z และอธิบาย lifecycle ได้

## C# Exercises

1. ให้ child รับ exit code จาก args
2. parent สร้าง child 3 ตัวและ WaitForExit ทุกตัว
3. ใช้ Stopwatch วัด completion times
4. ใช้ ProcessStartInfo เรียก /usr/bin/sleep 2
5. ใช้ strace -f ตรวจ process-related syscalls
6. อธิบาย Process.Start ต่างจาก fork/exec ใน abstraction level อย่างไร

## Quiz

- Context switch vs mode switch?
- Process.Start เป็น high-level API หรือ syscall?
- ควร assume ว่า native implementation ใช้ fork เสมอหรือไม่?
- WaitForExit ทำอะไร?
- Zombie คืออะไร?

## Explain-It-Back

~~~text
C# parent
→ Process.Start
→ runtime/platform process creation
→ child OS process
→ child exits
→ WaitForExit
~~~
