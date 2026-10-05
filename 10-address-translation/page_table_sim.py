#!/usr/bin/env python3
"""Small explicit VPN -> PFN translation simulator."""

PAGE_SIZE = 4096

PAGE_TABLE = {
    0x10: {"pfn": 0x7A, "writable": False},
    0x11: {"pfn": 0x22, "writable": True},
    0x12: {"pfn": 0x9F, "writable": True},
}


def translate(va: int, write: bool = False) -> int:
    vpn, offset = divmod(va, PAGE_SIZE)

    print(f"VA=0x{va:x}")
    print(f"VPN=0x{vpn:x} offset=0x{offset:x}")

    pte = PAGE_TABLE.get(vpn)
    if pte is None:
        raise KeyError(f"VPN 0x{vpn:x} is unmapped")

    if write and not pte["writable"]:
        raise PermissionError(f"VPN 0x{vpn:x} is read-only")

    pfn = int(pte["pfn"])
    pa = pfn * PAGE_SIZE + offset

    print(f"PTE: PFN=0x{pfn:x} writable={pte['writable']}")
    print(f"PA=0x{pa:x}")
    return pa


def main() -> None:
    tests = [
        (0x10020, False),
        (0x11ABC, True),
        (0x12FED, False),
    ]

    for va, write in tests:
        print("=" * 40)
        translate(va, write=write)

    print("=" * 40)
    try:
        translate(0x13000)
    except KeyError as exc:
        print(f"translation fault in simulator: {exc}")


if __name__ == "__main__":
    main()
