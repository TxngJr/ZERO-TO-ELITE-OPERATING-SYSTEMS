# Fedora 45 Setup — C#/.NET 10

## 1. ตรวจระบบ

~~~bash
cat /etc/fedora-release
uname -r
lscpu
free -h
~~~

## 2. ติดตั้ง .NET 10 SDK

~~~bash
sudo dnf install dotnet-sdk-10.0
~~~

ตรวจ:

~~~bash
dotnet --info
dotnet --version
~~~

repository มี global.json เพื่อกำหนด .NET 10 SDK feature baseline และอนุญาต roll-forward ภายใน .NET 10 feature line ตาม config

---

## 3. ติดตั้ง OS Tools

~~~bash
sudo dnf install   git   strace   procps-ng   psmisc   perf   time
~~~

---

## 4. Clone

~~~bash
git clone https://github.com/TxngJr/ZERO-TO-ELITE-OPERATING-SYSTEMS.git
cd ZERO-TO-ELITE-OPERATING-SYSTEMS
~~~

---

## 5. Build ทั้งคอร์ส

~~~bash
./build.sh
~~~

---

## 6. Correctness Verification

~~~bash
./verify.sh
~~~

ต้องผ่านทั้งสองคำสั่ง

---

## 7. Run Examples

~~~bash
dotnet run --project 04-concurrency-I/examples/Chapter04.csproj -- race

dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- producer-consumer

dotnet run --project 09-scheduling/examples/Chapter09.csproj -- mlfq

dotnet run --project 11-virtual-memory/examples/Chapter11.csproj -- protection
~~~

---

## 8. Observe Linux

~~~bash
ps -ef
ps -L -p PID
cat /proc/PID/status
cat /proc/PID/maps
ls /proc/PID/task
pmap -x PID
strace -f COMMAND
vmstat 1 5
~~~

---

## 9. Safety / Lab Boundaries

ไม่ต้อง:

- ปิด SELinux
- แก้ kernel
- รัน C# lab เป็น root
- ตั้ง real-time scheduler priority บนเครื่องหลัก
- ทำ intentional thrashing

protection-fault lab แยก failure ไป child process

concurrency demos ใช้ bounded loops/timeouts เพื่อไม่ให้ intentional infinite hang เป็นวิธีสอนหลัก

---

## 10. Study Rule

~~~text
Predict
→ Run
→ Observe
→ Explain
→ Modify
→ Verify
~~~

PID, timing, thread interleaving และ virtual addresses เปลี่ยนได้ระหว่าง run

ห้ามจำ output ที่ไม่ deterministic เป็นค่าตายตัว
