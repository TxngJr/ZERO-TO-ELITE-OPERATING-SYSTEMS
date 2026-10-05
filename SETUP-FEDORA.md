# Fedora Setup — C#/.NET

## 1. ตรวจระบบ

~~~bash
cat /etc/os-release
uname -a
lscpu
free -h
~~~

## 2. ติดตั้ง .NET SDK

บน Fedora 45:

~~~bash
sudo dnf install dotnet-sdk-10.0
~~~

ตรวจ:

~~~bash
dotnet --info
dotnet --version
~~~

## 3. ติดตั้ง OS Observation Tools

~~~bash
sudo dnf install strace gdb procps-ng psmisc perf time
~~~

## 4. Clone Course

~~~bash
git clone https://github.com/TxngJr/ZERO-TO-ELITE-OPERATING-SYSTEMS.git
cd ZERO-TO-ELITE-OPERATING-SYSTEMS
~~~

## 5. Build

~~~bash
./build.sh
~~~

## 6. Run

~~~bash
dotnet run --project 01-os-introduction/examples/Chapter01.csproj
~~~

## 7. C# Style Used in This Course

~~~csharp
static readonly object LockObj = new object();

lock (LockObj)
{
    while (!condition)
    {
        Monitor.Wait(LockObj);
    }

    Monitor.PulseAll(LockObj);
}
~~~

และ:

~~~csharp
Thread worker = new Thread(Work);
worker.Start();
worker.Join();
~~~

## 8. Safety

ไม่ต้องปิด SELinux, ไม่ต้องรัน lab เป็น root, และไม่ต้องแก้ kernel

## 9. Study Rule

~~~text
Predict
→ Run
→ Observe
→ Explain
~~~

PID, scheduling, timing และ memory addresses เปลี่ยนได้ จึงห้ามจำ output เป็นค่าตายตัว
