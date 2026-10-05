# Source-Aligned Answer Guide

เอกสารนี้ให้หลักคิด ไม่ได้บังคับ wording เดียว

---

## A1

Sequential version เรียก plus จน return ก่อนจึงเรียก minus

ไม่มี Thread อื่น executeสอง method นี้พร้อมกัน จึงไม่มี concurrent modification ของ sum จากสอง worker flows

## A2

~~~text
sum(1..1,000,000)
-
sum(0..999,999)
=
1,000,000
~~~

## A3

เกิด lost-update/race ได้ เพราะทั้งสอง threads ทำ read-modify-write ต่อ sum โดยไม่มี synchronization

## A4

~~~text
read old sum
compute old + i
write new sum
~~~

interleaving ระหว่าง read และ write ทำให้ update ถูกทับได้

## A5

lock acquisition/release ทุก iteration เพิ่ม synchronization overhead และทำให้ shared update serialize

## A6

Activity 02 ใช้ System.Threading.Lock สำหรับ mutual exclusion

Activity 03 ใช้ object monitor เพราะต้องใช้ Monitor.Wait/PulseAll

ห้ามถือว่าสอง mechanism map เหมือนกันทุกประการ

## A7

JIT warmup, scheduling, CPU frequency, background work และ cache state ทำให้ timing รอบเดียว noisy

---

## B3

ต้องใช้ while เพราะ wake-up ไม่รับประกันว่า predicate ยัง true ตอน thread ได้ monitor กลับ

## B4

ใช่ ต้องถือ monitor ก่อน Wait

## B5

ไม่ Wait ปล่อย monitor ระหว่างรอ

## B6

ใช่ Wait return หลัง reacquire monitor

## B7

writer ต้องไม่ overwrite slot เดิมก่อน reader consume

## B8

writer อาจนอนรอ hasValue=false โดยไม่มี signal ให้กลับไปตรวจ predicate

---

## C1

หนึ่ง reader consume ต่อ published value ใน protocol นี้

## C2

PulseAll ทำให้ waiters พร้อมแข่งขัน แต่มีเพียง thread ที่ acquire monitor ได้ทีละหนึ่ง เมื่อ winner เปลี่ยน hasValue=false ตัวอื่นต้อง re-check แล้วกลับไปรอ

## C4

ต้องเก็บ state เพิ่ม เช่น sequence number และ acknowledgement ต่อ reader หรือ queue แยก ไม่ใช่ boolean เดียว

---

## D6

producer แรกสร้าง 50 values: 1..50

producer สองสร้าง 51 values: 100..150

รวม 101

## D7

consumer 3 ตัว × 60 attempts = 180 dequeue attempts

## D8

180 > 101 จึงไม่สามารถเป็น fixed-count consumer protocol ที่ถูกต้อง

---

## E3

ถ้า buffer ว่างและไม่มี producer ใดจะผลิตเพิ่มแล้ว การรอต่อไม่มี event ที่จะทำให้มี item ใหม่ จึง terminate ได้

## E4

ProducerFinished เป็น shared state ที่อยู่ใน predicate ของ consumers จึงต้องเปลี่ยนภายใต้ synchronization protocol เดียวกัน

## E5

consumer ที่กำลังรอ empty condition ต้องตื่นเพื่อตรวจว่าตอนนี้ producer ทุกตัวจบแล้วหรือยัง

## E9

Sleep เปลี่ยน timing/interleaving เท่านั้น ไม่รับประกัน ordering correctness

---

## F1

localResult ลด shared writes และลด lock contention

## F2

มีเพียง merge สุดท้ายที่แตะ global result

## F5

ตอบว่า source ที่ให้มายังไม่มี specification ของ Calculate1/extra region จึงไม่ควรเดา

## F6

จำนวน threads เป็น application design choice ส่วน CPU topology เป็น hardware/runtime environment

## F7

ไม่ มี scheduling, contention, cache/memory และ overhead

---

## G1

ไม่ Scheduler เป็นผู้ตัดสิน actual execution timing

## G2

Join เป็น explicit completion synchronization ระหว่าง caller กับ target thread

## G3

ไม่ Wait ทำให้ thread blocked/waiting จนถูก signal และกลับมาแข่งขัน lock

## G5

buffer เป็น static managed state ใน virtual address space ของ process เดียวกัน

## G6

threads ของ process share process address space/managed heap จึงเข้าถึง static state เดียวกันได้

## G7

synchronization ต้องถูกต้องสำหรับ valid interleavings ไม่ใช่หวังว่า scheduler จะเรียง execution ตามที่ต้องการ
