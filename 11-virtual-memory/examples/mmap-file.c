#define _POSIX_C_SOURCE 200809L
#include <fcntl.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/mman.h>
#include <unistd.h>

int main(void)
{
    char path[] = "/tmp/zero-to-elite-mmap-XXXXXX";
    int fd = mkstemp(path);

    if (fd == -1) {
        perror("mkstemp");
        return 1;
    }

    const size_t length = 4096;

    if (ftruncate(fd, (off_t)length) != 0) {
        perror("ftruncate");
        close(fd);
        unlink(path);
        return 1;
    }

    char *mapping = mmap(
        NULL,
        length,
        PROT_READ | PROT_WRITE,
        MAP_SHARED,
        fd,
        0
    );

    if (mapping == MAP_FAILED) {
        perror("mmap");
        close(fd);
        unlink(path);
        return 1;
    }

    const char message[] = "written through MAP_SHARED";
    memcpy(mapping, message, sizeof(message));

    if (msync(mapping, length, MS_SYNC) != 0) {
        perror("msync");
    }

    char buffer[64] = {0};
    if (pread(fd, buffer, sizeof(buffer) - 1U, 0) == -1) {
        perror("pread");
    } else {
        printf("file readback: %s\n", buffer);
    }

    printf("temporary file: %s\n", path);

    munmap(mapping, length);
    close(fd);
    unlink(path);
    return 0;
}
