# Lab 07 — Producer/Consumer, Readers/Writers, Dining

~~~bash
dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- producer-consumer
dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- readers-writers
dotnet run --project 07-synchronization-II/examples/Chapter07.csproj -- dining
~~~

## Main Exercise

นำ Thread Safe Buffer ที่เรียนมาเขียนใหม่โดย:

- ห้าม Busy Wait
- ใช้ lock
- ใช้ Monitor.Wait
- ใช้ Monitor.PulseAll
- producers ต้องบอกว่า finished
- consumers ต้อง exit ได้เองเมื่อไม่มี item เหลือ

อธิบาย invariant ของ Front, Back และ Count
