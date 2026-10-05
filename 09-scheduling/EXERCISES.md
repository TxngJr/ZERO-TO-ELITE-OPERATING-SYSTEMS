# Scheduling Exercises — C# Edition

## Exercise 1

แก้ Workload ใน Program.cs:

~~~text
A AT=0 BT=7 PR=2
B AT=2 BT=4 PR=1
C AT=4 BT=1 PR=3
D AT=5 BT=4 PR=2
~~~

ทำ FCFS และคำนวณ CT/TAT/WT/RT

## Exercise 2

เขียน C# function:

~~~csharp
static double AverageWaitingTime(IEnumerable<State> states)
~~~

## Exercise 3

เพิ่ม enum:

~~~csharp
enum Algorithm
{
    Fcfs,
    Sjf,
    Srtf,
    Priority,
    RoundRobin,
    Mlfq
}
~~~

แล้วแทน string switch

## Exercise 4

เพิ่ม context switch cost = 1 time unit แล้วเปรียบเทียบ RR q=1 กับ q=4

## Exercise 5

เพิ่ม aging ใน Priority Scheduling ด้วย C#
