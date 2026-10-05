# Lab 10 — Virtual Addresses, Pages, TLB Mental Model

## Build

~~~bash
cd 10-address-translation
make clean
make
~~~

## A. Runtime Page Size

~~~bash
getconf PAGESIZE
~~~

เปรียบเทียบกับ:

~~~bash
./bin/address-demo
~~~

## B. Observe Virtual Addresses

~~~bash
for i in {1..5}; do ./bin/address-demo; done
~~~

ตอบ:

- page size เปลี่ยนหรือไม่
- stack/heap/code addresses เปลี่ยนหรือไม่
- VPN เปลี่ยนหรือไม่
- offset มีช่วงค่าเท่าไร

## C. Split Address

~~~bash
./bin/address-split 0x12345678
./bin/address-split 0x400000
./bin/address-split 4097
~~~

คำนวณด้วยมือก่อน แล้ว compare

## D. Process Maps

~~~bash
cat /proc/$$/maps
pmap -x $$
~~~

เลือก mapping 3 ตัว แล้วบันทึก:

- start/end
- permissions
- pathname/label ถ้ามี

## E. Run a Long-Lived Process

~~~bash
sleep 60 &
PID=$!
cat /proc/$PID/maps
pmap -x $PID
wait $PID
~~~

## F. Translation Simulator

~~~bash
python3 page_table_sim.py
~~~

ก่อนดู output คำนวณ:

~~~text
VA 0x11ABC
page size 0x1000
VPN = ?
offset = ?
PFN = 0x22
PA = ?
~~~

## G. TLB Paper Trace

สมมติ TLB capacity 2 entries:

~~~text
access VPN A
access VPN B
access VPN A
access VPN C
access VPN B
~~~

สร้าง table:

~~~text
Access | Hit/Miss | TLB after access
~~~

ใช้ replacement policy ที่โจทย์กำหนดเอง เช่น LRU

จุดประสงค์คือแยก:

~~~text
TLB miss
page-table lookup
page fault
~~~

สามอย่างนี้ไม่ใช่คำเดียวกัน

## H. Security Note

ไม่ต้องใช้ sudo เพื่ออ่าน physical PFN จาก pagemap

Lab นี้ตั้งใจเรียน mapping abstraction โดยไม่ bypass kernel PFN restrictions
