# Chapter 03 — Process & Context Switch Part II

## 1. Goals

หลังบทนี้ต้อง:

- อธิบาย context switch อย่างถูกต้อง
- แยก mode switch/system-call transition ออกจาก process context switch
- เข้าใจ fork
- เข้าใจ exec family mental model
- เข้าใจ wait/waitpid
- trace process lifecycle
- อธิบาย zombie และ orphan/reparenting
- วิเคราะห์ shell command execution แบบ simplified fork/exec model
- ใช้ strace, ps, pstree และ /proc ตรวจ behavior จริงได้

---

## 2. Process Lifecycle

simplified flow:

~~~text
Executable requested
      ↓
process/task created
      ↓
Runnable
      ↓
Running
   ↙      ↘
wait       exit
 ↓           ↓
Blocked    terminated
 ↓           ↓
Runnable   waitable child state
             ↓
          reaped
~~~

คำว่า "terminated" ใน textbook กับ kernel-specific state/reporting ต้องดู context

---

## 3. Context Switch คืออะไร

สมมติ logical CPU กำลัง execute A

~~~text
CPU execution context = A
PC/RIP concept = A
SP/RSP concept = A
register values = A
~~~

scheduler/kernel ตัดสินใจให้ B รัน

simplified:

~~~text
Process A running
      |
      | save required execution state
      v
Kernel scheduling path
      |
      | choose B
      | restore/switch required state
      v
Process B running
~~~

แนวคิดสำคัญ:

- A ต้องกลับมาทำต่อได้ถูกจุด
- B ต้องเห็น state ของ B ไม่ใช่ของ A
- address-space related state อาจต้องเปลี่ยนเมื่อ switch ข้าม processes
- architecture/kernel implementation จริงซับซ้อนกว่าการ copy "register array" ธรรมดา

---

## 4. Context Switch vs Mode Switch

นี่เป็นจุดที่ออกข้อสอบบ่อยและคนสับสนมาก

### Mode / privilege transition

user process เรียก system call:

~~~text
Process A user mode
       ↓
Process A executing kernel code on behalf of A
       ↓
Process A returns to user mode
~~~

ถ้า kernel ไม่ schedule process B เลย นี่ไม่ใช่ process context switch ตามความหมายที่เรากำลังใช้

### Process context switch

~~~text
A → kernel → B
~~~

execution เปลี่ยนจาก A ไป B

ดังนั้น:

~~~text
system call ≠ context switch เสมอ
~~~

system call อาจนำไปสู่ blocking และจากนั้น scheduler switch ไป task อื่นได้ แต่เป็นคนละแนวคิด

---

## 5. อะไรทำให้ execution ถูกเปลี่ยนได้

ตัวอย่าง:

- time slice / preemption
- task blocks waiting for I/O
- sleep
- synchronization wait
- higher-priority scheduling event
- process exits
- explicit scheduling/yield mechanisms

รายละเอียด exact scheduling policy เรียน Chapter 09

---

## 6. fork()

fork สร้าง child process จาก calling process

interface:

~~~c
pid_t fork(void);
~~~

return values:

- parent: ได้ child PID (> 0)
- child: ได้ 0
- error: -1 ใน parent caller และไม่มี child ใหม่

mental model:

~~~text
Before fork

Parent
  |
  | fork()
  |
  +-------------------+
  |                   |
Parent              Child
fork returned PID   fork returned 0
~~~

ทั้งสอง execution flows ดำเนินต่อจากหลัง fork

---

## 7. fork ไม่ได้แปลว่า copy physical RAM ทั้งก้อนทันที

logical process state ถูกสร้างให้ child มี address space semantics ที่เริ่มต้นจาก parent snapshot ตาม POSIX/Linux rules

Linux ใช้ Copy-on-Write techniques เพื่อหลีกเลี่ยง physical copying ที่ไม่จำเป็น

simplified:

~~~text
Parent virtual pages ----+
                         +--> shared physical backing while safe
Child virtual pages -----+

write occurs
     ↓
copy private page as needed
~~~

รายละเอียด page tables และ page faults จะกลับมา Chapter 10–11

---

## 8. fork output ordering

run:

~~~bash
cd 03-process-context-II
make
./bin/fork-demo
~~~

อย่าคาดว่าบรรทัด parent และ child หลัง fork ต้องเรียงเหมือนเดิมทุกครั้ง

scheduler สามารถให้ฝ่ายใดรันก่อน

นี่เป็นจุดเริ่มต้นของ concurrency reasoning

---

## 9. stdio buffering trap around fork

ถ้ามี buffered output ที่ยังไม่ flush ก่อน fork child สามารถ inherit user-space buffer state ทำให้ output ดูเหมือนซ้ำเมื่อทั้งสอง flush ภายหลัง

จึงเห็น fflush(stdout) ในตัวอย่างบางชนิด

นี่แสดงว่า:

~~~text
fork + user-space runtime state
~~~

มี interaction ที่มากกว่าการจำ return value

---

## 10. exec — Replace the Process Image

exec family ไม่ได้ "สร้าง child ใหม่" โดยตัวมันเอง

core idea:

~~~text
same process identity context
      |
      | successful exec
      v
new program image
~~~

successful exec ไม่ return กลับไปคำสั่งถัดไปของ old image

ถ้า exec return แสดงว่าเกิด error

### exec family

เช่น:

- execl
- execv
- execlp
- execvp
- execve

ความแตกต่างเกี่ยวกับ argument representation, PATH search และ environment handling

kernel interface สำคัญคือ execve-like execution path

อ่าน:

~~~bash
man 3 exec
man 2 execve
~~~

---

## 11. fork + exec

shell-style mental model แบบง่าย:

~~~text
Shell
  |
  | fork
  +-----------------+
  |                 |
Shell(parent)     Child
                    |
                    | exec("program")
                    v
                New program
~~~

นี่เป็น teaching model

shell จริงต้องจัดการ pipes, redirection, signals, job control, built-ins, process groups และ optimization/implementation details เพิ่ม

---

## 12. wait และ waitpid

parent สามารถรอ child state change/termination และเก็บ status ได้

สำคัญเพราะ child ที่ terminate แล้วอาจต้องเหลือ termination information ให้ parent collect

ตัวอย่าง:

~~~c
waitpid(child_pid, &status, 0);
~~~

หลัง return สามารถใช้ macros เช่น:

- WIFEXITED(status)
- WEXITSTATUS(status)
- WIFSIGNALED(status)
- WTERMSIG(status)

อย่าอ่าน integer status ตรง ๆ แล้ว assume low byte = exit code โดยไม่ใช้ documented macros

---

## 13. Zombie Process

zombie คือ child ที่ terminate แล้ว แต่ parent ยังไม่ได้ reap termination status

simplified:

~~~text
Child running
    ↓
exit
    ↓
Zombie / waitable terminated state
    ↓ parent calls wait/waitpid
Reaped
~~~

zombie:

- ไม่ได้ execute instructions ต่อ
- ไม่ใช่ process ที่ "กิน CPU ทำงานอยู่"
- ยังต้องมี kernel bookkeeping บางส่วน เช่น identity/termination information จนถูก reap

ดูตัวอย่าง:

~~~bash
./bin/zombie-demo
~~~

ในช่วง 20 วินาที เปิดอีก terminal:

~~~bash
ps -o pid,ppid,stat,comm | grep zombie-demo
~~~

ควรเห็น child Z ในช่วงเวลานั้น

โปรแกรมตัวอย่างจะ wait ภายหลังเพื่อ cleanup

---

## 14. Orphan / Reparenting

ถ้า parent จบก่อน child ที่ยังทำงาน child ไม่จำเป็นต้องตายตามทันที

kernel จะจัดการ parent relationship/reparenting ตาม Linux process model

อย่าสรุปตายตัวว่า PPID ใหม่ต้องเป็น 1 เสมอในทุก environment เพราะ subreaper, container/PID namespace และ service/session managers สามารถทำให้ observation ต่างออกไป

ทดลอง:

~~~bash
./bin/orphan-demo
~~~

ดู PPID ที่ child รายงานก่อนและหลัง parent exit

---

## 15. wait ทำไมสำคัญ

นอกจาก synchronization แล้ว wait ช่วย:

- เก็บ termination status
- reap terminated child
- ทำให้ parent รู้ child result

ถ้า long-running parent สร้าง children แล้วไม่ wait/reap อาจสะสม zombies

---

## 16. Shell command execution

เมื่อพิมพ์:

~~~bash
ls
~~~

simplified:

~~~text
shell parses command
   ↓
create execution path
   ↓
child-like execution context
   ↓
exec ls
   ↓
ls runs
   ↓
ls exits
   ↓
shell observes completion
   ↓
prompt returns
~~~

แต่ shell built-ins เช่น cd ไม่สามารถอธิบายด้วย "fork child แล้ว exec cd" แบบปกติได้ เพราะ directory ของ shell parent เองต้องเปลี่ยน

ลอง:

~~~bash
type cd
type ls
~~~

---

## 17. Background job

~~~bash
sleep 30 &
jobs
ps -o pid,ppid,stat,comm -C sleep
~~~

shell ไม่รอแบบ foreground blocking เดียวกันก่อนคืน prompt

นี่เป็น preview ของ job control

---

## 18. strace fork/exec/wait

trace:

~~~bash
strace -f -e trace=process ./bin/exec-demo
~~~

-f ให้ตาม children

trace=process ช่วย focus กลุ่ม system calls ที่เกี่ยวกับ process lifecycle แต่ชื่อ exact syscalls ที่เห็นอาจเป็น fork/clone/execve/wait variants ตาม libc/kernel implementation และ architecture

อย่าบังคับว่าต้องเห็น syscall ชื่อ fork literal เสมอ

---

## 19. Why Linux may show clone-like calls

high-level API และ kernel syscall ไม่จำเป็นต้องชื่อเดียวกัน 1:1

libc implementation สามารถ implement fork semantics ผ่าน kernel mechanisms ที่เหมาะสม

บทเรียนสำคัญ:

~~~text
C/POSIX API layer
≠
kernel syscall naming one-to-one
~~~

ใช้ strace เพื่อสังเกต ไม่ใช่ท่องชื่อจาก source code

---

## 20. Error Handling

ทุก process API มี failure cases

ตัวอย่าง fork:

~~~c
pid_t pid = fork();
if (pid < 0) {
    perror("fork");
    return 1;
}
~~~

exec:

~~~c
execlp(...);
perror("execlp");
_exit(127);
~~~

ถ้า exec สำเร็จ บรรทัด perror ไม่ถูก execute

---

## 21. exit vs _exit Preview

หลัง fork โดยเฉพาะ child error path ก่อน/หลัง exec อาจใช้ _exit เพื่อหลีกเลี่ยง user-space stdio/atexit handling ที่ถูก inherit บางอย่าง

ในตัวอย่างเราจะใช้ _exit ใน child cases ที่เหมาะสม

รายละเอียด C runtime cleanup เรียนภายหลัง

---

## 22. Common Misconceptions

### "fork เริ่มรัน child ตั้งแต่ main ใหม่"

ผิด ทั้ง parent/child continue จากจุดหลัง fork ตาม semantics

### "fork return child PID ทั้งสองฝั่ง"

ผิด child ได้ 0

### "exec สร้าง process ใหม่"

ผิด mental model หลักคือ replace process image

### "wait สร้าง child"

ผิด

### "zombie คือ process ที่ยังทำงานแต่ค้าง"

ผิด child terminate แล้ว

### "system call = context switch"

ผิด

### "parent ตายแล้ว child ต้องตาย"

ไม่จริงโดยทั่วไป

---

## 23. Performance Notes

context switches มี cost เช่น:

- scheduler/kernel work
- execution state transitions
- cache/TLB-related effects ขึ้นกับ case
- loss of locality

แต่ห้ามบอกว่าทุก context switch flush cache/TLB ทั้งหมด เพราะ hardware/kernel มี mechanisms เพื่อลด cost และ behavior ขึ้นกับชนิด switch/architecture

fork ที่ใช้ Copy-on-Write ก็ไม่ได้แปลว่า free: ยังมี kernel structures/page-table-related work

---

## 24. Security Notes

exec ไม่ได้หมายความว่า process ได้ privilege เพิ่มเอง

credentials, file permissions, capabilities, set-user-ID semantics และ security policy มี rules เฉพาะ

เรายังไม่ใช้ privileged execution ใน Lab นี้

---

## 25. Exercises

1. Context คืออะไร
2. Context switch คืออะไร
3. mode transition ต่างจาก process switch อย่างไร
4. system call ต้องสลับ process เสมอหรือไม่
5. fork return อะไรใน parent
6. fork return อะไรใน child
7. fork error return อะไร
8. exec สำเร็จแล้ว return หรือไม่
9. exec สร้าง PID ใหม่โดยหลักหรือไม่
10. fork + exec ใช้คู่กันเพื่ออะไร
11. waitpid ทำหน้าที่อะไร
12. zombie คืออะไร
13. ทำไม zombie ยังมี PID/reportable entry ชั่วคราว
14. orphan child คือแนวคิดอะไร
15. ทำไม PPID หลัง reparenting ไม่ควรถูก hardcode ว่าต้อง 1 ทุก environment
16. อธิบาย Copy-on-Write intro
17. ทำไม output parent/child order ไม่ deterministic
18. ทำไม buffered printf ก่อน fork ทำให้ output surprise ได้
19. ใช้ strace -f อธิบาย lifecycle ของ exec-demo
20. วิเคราะห์เหตุผลที่ cd เป็น shell builtin

### Trace challenge

พิจารณา:

~~~c
printf("A\n");
pid_t p = fork();
if (p == 0) {
    printf("C\n");
} else {
    printf("P\n");
}
~~~

บอกสิ่งที่ guaranteed และสิ่งที่ scheduling อาจสลับลำดับ โดยสมมติ stdout behavior ชัดเจนตาม environment ที่กำหนด

---

## 26. Quiz

1. process context switch ต้องเปลี่ยน execution entity หรือไม่
2. user→kernel→same user process ถือเป็น process switch เสมอหรือไม่
3. fork child return value คืออะไร
4. successful exec return หรือไม่
5. waitpid ใช้กับ child lifecycle หรือไม่
6. zombie ยัง running หรือไม่
7. zombie ถูก reap ด้วยอะไร
8. fork physical-copy RAM ทั้งหมดทันทีเสมอหรือไม่
9. shell command external มักเชื่อมกับ exec concept หรือไม่
10. output order ของ parent/child หลัง fork deterministic หรือไม่

### Answers

1. ใช่ใน mental model นี้
2. ไม่
3. 0
4. ไม่
5. ใช่
6. ไม่
7. wait/waitpid family behavior
8. ไม่; Linux ใช้ Copy-on-Write techniques
9. ใช่
10. ไม่โดยทั่วไป

---

## 27. Explain-It-Back

ต้องอธิบาย scenario นี้จากต้นจนจบ:

~~~text
shell
  ↓
fork-like creation
  ↓
parent + child
  ↓
child exec
  ↓
new program executes
  ↓
program exits
  ↓
parent wait/reap
~~~

จากนั้นตอบ:

- register/context เกี่ยวตรงไหน
- scheduler เกี่ยวตรงไหน
- PID/PPID เปลี่ยนหรือไม่
- zombie เกิดตรงไหนถ้า parent ยังไม่ wait
- system call transition ตรงไหนไม่จำเป็นต้องเป็น process switch

ถ้าตอบครบ แสดงว่าพร้อมขึ้น Concurrency
