# Chapter 02 — Process & Context Switch Part I with C#

## Goals

เข้าใจ Program vs Process, PID/PPID, Process State, PCB abstraction, CPU Context, address space และ managed/unmanaged memory

## Program vs Process

C# source หรือ compiled assembly ยังไม่ใช่ running process

เมื่อ .NET runtime เริ่ม execute จะเกิด OS process ที่มี PID จริง

~~~csharp
Console.WriteLine(Environment.ProcessId);
~~~

## C# Process View

~~~csharp
using Process p = Process.GetCurrentProcess();

Console.WriteLine(p.Id);
Console.WriteLine(p.Threads.Count);
Console.WriteLine(p.WorkingSet64);
~~~

Process เป็น .NET wrapper สำหรับข้อมูล process ของ OS

## Managed vs Unmanaged Memory

C# object ทั่วไปอยู่ภายใต้ Garbage Collector จึงไม่ควร assume ว่า address คงที่

Lab ใช้:

~~~csharp
Marshal.AllocHGlobal(4096);
~~~

เพื่อสร้าง unmanaged allocation ที่เหมาะกับการสังเกต memory mapping มากกว่า

## Simplified Process Address Space

~~~text
Code / runtime mappings
Managed heap
Native allocations
Shared libraries
Thread stacks
Other mmap regions
~~~

ดูจริงด้วย:

~~~bash
cat /proc/PID/maps
pmap -x PID
~~~

## PID / PPID

C#:

~~~csharp
Environment.ProcessId
~~~

Linux PPID:

~~~bash
ps -o pid,ppid,stat,comm -p PID
~~~

## PCB

PCB เป็น abstraction ของข้อมูลที่ OS ต้องเก็บ เช่น:

- identity
- state
- CPU context
- scheduling
- memory mappings
- open resources

C# ไม่มี class PCB ที่แทน kernel structure

## CPU Context

concept สำคัญ:

- instruction pointer
- stack pointer
- registers
- flags

ใช้เพื่อ resume execution หลังถูกสลับออกจาก CPU

## C# Exercises

1. เขียน program แสดง ProcessName, Id, Thread count และ WorkingSet
2. อ่าน /proc/self/status ด้วย File.ReadAllLines
3. parse VmRSS และ Threads
4. Allocate unmanaged memory 1 MiB แล้ว inspect /proc/self/maps
5. อธิบาย managed heap ต่างจาก unmanaged allocation อย่างไร
6. เขียน C# program print PID แล้ว Sleep 30 วินาทีเพื่อให้ ps inspect

## Quiz

- Program vs Process?
- Environment.ProcessId คืออะไร?
- C# object address ควร assume ว่าคงที่หรือไม่?
- PCB เป็น C# class หรือ textbook abstraction?
- /proc/PID/maps แสดงอะไร?

## Explain-It-Back

อธิบาย C# application → .NET runtime → Linux process
