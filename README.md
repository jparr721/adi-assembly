# adi_enum – “AppDomain‑Injectable” Path Enumerator

**⚠️ Warning – this tool is for defensive security use only.
Do not use it on systems you do not own or have explicit permission to analyze.**

## What It Does

`adi_enum` scans a directory tree and reports every **.NET executable** that **lacks a companion `.config` file**.
Missing configuration files are a classic indicator that an attacker could *inject* a new AppDomain or tamper with the runtime environment without the defender’s knowledge.

For each candidate binary it also reports:

| Item | Meaning |
|------|---------|
| **Writable** | Is the binary’s parent directory writable by the current user? |
| **IsDotNet** | Is the file a managed .NET assembly (vs. a native EXE)? |

The tool outputs only “vulnerable” binaries by default, but you can list every EXE or silence normal output with flags.

## Building
```
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:TrimUnusedCode=true
```

## Running

```bash
# Basic scan (shows only vulnerable binaries)
./adi_enum.exe C:\Path\To\Scan

# Quiet mode – one line per vulnerable binary
./adi_enum.exe -q C:\Path\To\Scan

# List every executable found
./adi_enum.exe -a C:\Path\To\Scan
```

If no directory is supplied, or you pass `-h`/`--help`, the tool prints a help message.

---

## Command‑Line Options

| Flag | Description |
|------|-------------|
| `-q` | Suppress normal output; only the paths of vulnerable binaries are printed. |
| `-a` | Also list non‑vulnerable binaries (i.e., every `.exe` found). |
| `-h` / `--help` | Show the usage message. |

Typical output (normal mode):

```
[*] Searching for .NET binaries without .config files

[+] Vulnerable: myapp.exe
    Path: C:\Users\Public\myapp.exe
    Missing: C:\Users\Public\myapp.exe.config

    Writable: True
    IsDotNet: True
```

In quiet mode only the path of vulnerable binaries is printed:

```
C:\Users\Public\myapp.exe
```

---

## License

This project is licensed under the **MIT License** – see the `LICENSE` file for details.

---
