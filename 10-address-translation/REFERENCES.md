# Chapter 10 References

Use upstream Linux documentation for architecture details that can vary by kernel/hardware.

- Linux x86-64 memory management:
  https://docs.kernel.org/arch/x86/x86_64/mm.html
- Linux x86-64 support index:
  https://docs.kernel.org/arch/x86/x86_64/index.html
- 5-level paging:
  https://docs.kernel.org/arch/x86/x86_64/5level-paging.html
- Process pagemap interfaces:
  https://docs.kernel.org/admin-guide/mm/pagemap.html

Important:

- x86-64 Linux can support 4-level or 5-level paging depending on hardware/kernel configuration.
- unprivileged PFN visibility via pagemap is restricted for security reasons.
- use runtime observation and current kernel docs rather than hardcoding assumptions.
