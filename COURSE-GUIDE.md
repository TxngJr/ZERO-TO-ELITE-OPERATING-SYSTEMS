# Course Guide — C# Edition

## เป้าหมาย

เรียน Operating Systems โดยใช้ C# เป็นภาษาปฏิบัติหลักบน Fedora

ผู้เรียนต้องเชื่อมได้ว่า:

~~~text
C# code
→ .NET runtime
→ Linux process/thread
→ system call
→ kernel
→ CPU / memory
~~~

## Teaching Pattern

~~~text
Why
→ Concept
→ OS Mental Model
→ C# Mental Model
→ C# Code
→ Run
→ Observe Linux
→ Debug
→ Exercise
→ Explain-It-Back
~~~

## C# Programming Standard

ใช้ Thread, lock, Monitor, Interlocked, Volatile, Mutex, SemaphoreSlim, ReaderWriterLockSlim, Process และ MemoryMappedFile

P/Invoke ใช้เฉพาะเมื่อ .NET ไม่มี abstraction ที่ทำให้เห็น OS concept นั้นตรงพอ

## Important Rule

อย่าเหมารวมว่า C# API = Linux syscall ชื่อเดียวกัน

ตัวอย่าง:

~~~text
Console.WriteLine ≠ write(2) ตรง ๆ หนึ่งครั้งเสมอ
Process.Start ≠ fork() + execve() ที่เราควบคุมเองเสมอ
Thread ≠ CPU core
~~~

ให้ใช้ strace, ps, /proc และ Linux tools เพื่อพิสูจน์ runtime behavior

## Exercise Levels

ทุกบทมี:

1. Recall
2. Explain
3. Trace
4. C# Code Reading
5. C# Debugging
6. C# Implementation
7. OS Observation
8. Design

## Midterm

ประมาณ Chapters 01–05

## Core Completion

Chapters 01–11 เป็น Core Scope

หลังจากนั้นต่อ Advanced Track ได้ เช่น Signals, IPC, Filesystems, Containers, Kernel Modules และ OS Development
