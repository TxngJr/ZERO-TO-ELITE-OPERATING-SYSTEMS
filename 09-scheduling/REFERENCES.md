# Scheduling References

ส่วน Linux scheduler เป็น implementation detail ที่เปลี่ยนตาม kernel version จึงควรอ่าน upstream documentation ควบคู่กับบทเรียน

## Official Linux Kernel Documentation

- EEVDF Scheduler: https://docs.kernel.org/scheduler/sched-eevdf.html
- CFS Scheduler: https://docs.kernel.org/scheduler/sched-design-CFS.html
- Scheduler Domains: https://docs.kernel.org/scheduler/sched-domains.html
- Deadline Scheduling: https://docs.kernel.org/scheduler/sched-deadline.html
- sched_ext: https://docs.kernel.org/scheduler/sched-ext.html

## Rule

ก่อนสรุปว่าเครื่อง Fedora ปัจจุบันใช้ behavior แบบใด:

~~~bash
uname -r
~~~

แล้วเทียบกับ documentation ของ kernel generation ที่ใช้งาน

อย่าใช้บทความเก่าที่บอกว่า "Linux scheduler = CFS" เป็นคำอธิบายทั้งหมดของ modern kernel
