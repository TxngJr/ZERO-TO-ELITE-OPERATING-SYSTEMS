#!/usr/bin/env bash
set -euo pipefail
ulimit -c 0 || true

run() {
  timeout 20s dotnet run --no-build -c Release --project "$1" -- "${@:2}"
}

assert_contains() {
  local output="$1"
  local expected="$2"

  if ! grep -Fq "$expected" <<<"$output"; then
    echo "ASSERTION FAILED: expected output to contain: $expected" >&2
    echo "---- actual output ----" >&2
    echo "$output" >&2
    exit 1
  fi
}

echo "==> Chapter 01 native boundary"
out="$(run 01-os-introduction/examples/Chapter01.csproj raw-write 2>&1)"
assert_contains "$out" "Hello through libc write from C#"

echo "==> Chapter 03 child lifecycle"
out="$(run 03-process-context-II/examples/Chapter03.csproj)"
assert_contains "$out" "child exit code=42"

echo "==> Chapter 04 thread IDs"
out="$(run 04-concurrency-I/examples/Chapter04.csproj basic)"
assert_contains "$out" "TID="

echo "==> Chapter 05 visibility"
out="$(run 05-concurrency-II/examples/Chapter05.csproj visibility)"
assert_contains "$out" "payload=42"

echo "==> Chapter 06 synchronization self-test"
out="$(run 06-synchronization-I/examples/Chapter06.csproj self-test)"
assert_contains "$out" "Chapter06 self-test PASS"

echo "==> Chapter 06 CAS invariant"
out="$(run 06-synchronization-I/examples/Chapter06.csproj cas)"
assert_contains "$out" "final stock=0"

echo "==> Chapter 07 producer-consumer invariant"
out="$(run 07-synchronization-II/examples/Chapter07.csproj producer-consumer)"
assert_contains "$out" "final count=0"

echo "==> Chapter 08 graph correctness"
out="$(run 08-synchronization-III/examples/Chapter08.csproj self-test)"
assert_contains "$out" "Chapter08 self-test PASS"

echo "==> Chapter 08 bounded livelock termination"
out="$(run 08-synchronization-III/examples/Chapter08.csproj livelock)"
assert_contains "$out" "bounded livelock demo ended"

echo "==> Chapter 09 scheduler golden tests"
out="$(run 09-scheduling/examples/Chapter09.csproj self-test)"
assert_contains "$out" "Chapter09 self-test PASS"

echo "==> Chapter 10 translation/TLB tests"
out="$(run 10-address-translation/examples/Chapter10.csproj self-test)"
assert_contains "$out" "Chapter10 self-test PASS"

echo "==> Chapter 11 replacement/COW tests"
out="$(run 11-virtual-memory/examples/Chapter11.csproj self-test)"
assert_contains "$out" "Chapter11 self-test PASS"

echo "==> Chapter 11 demand paging"
out="$(run 11-virtual-memory/examples/Chapter11.csproj faults)"
assert_contains "$out" "minor-fault delta after touch="

echo "==> Chapter 11 shared file mapping"
out="$(run 11-virtual-memory/examples/Chapter11.csproj mmap)"
assert_contains "$out" "shared file mapping readback=123456"

echo "==> Chapter 11 page protection"
out="$(run 11-virtual-memory/examples/Chapter11.csproj protection 2>&1)"
assert_contains "$out" "Protection demo PASS"

echo "ALL CORRECTNESS CHECKS PASSED"
