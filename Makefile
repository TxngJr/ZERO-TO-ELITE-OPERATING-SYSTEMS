CHAPTERS := \
	01-os-introduction \
	02-process-context-I \
	03-process-context-II \
	04-concurrency-I \
	05-concurrency-II \
	06-synchronization-I \
	07-synchronization-II \
	08-synchronization-III \
	10-address-translation \
	11-virtual-memory

.PHONY: all core-build python-check smoke clean

all: core-build python-check

core-build:
	@set -e; for dir in $(CHAPTERS); do \
		echo "==> building $$dir"; \
		$(MAKE) -C $$dir all; \
	done

python-check:
	python3 -m py_compile \
		09-scheduling/scheduler_sim.py \
		10-address-translation/page_table_sim.py \
		11-virtual-memory/page_replacement_sim.py

smoke: all
	./01-os-introduction/bin/hello
	./02-process-context-I/bin/memory-layout
	./03-process-context-II/bin/wait-demo
	./04-concurrency-I/bin/thread-basic
	./05-concurrency-II/bin/release-acquire
	./06-synchronization-I/bin/mutex-counter
	./07-synchronization-II/bin/producer-consumer
	./08-synchronization-III/bin/waitfor-cycle
	python3 09-scheduling/scheduler_sim.py --algo rr --quantum 2
	./10-address-translation/bin/address-split 0x12345678
	python3 10-address-translation/page_table_sim.py
	./11-virtual-memory/bin/page-fault-demo
	./11-virtual-memory/bin/cow-demo
	./11-virtual-memory/bin/mmap-file
	./11-virtual-memory/bin/protection-demo
	python3 11-virtual-memory/page_replacement_sim.py

clean:
	@set -e; for dir in $(CHAPTERS); do \
		$(MAKE) -C $$dir clean; \
	done
	find . -type d -name __pycache__ -prune -exec rm -rf {} +
