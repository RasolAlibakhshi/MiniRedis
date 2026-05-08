# MiniRedis


# MiniRedis (C#) — A Tiny Redis-like TCP Key-Value Server

MiniRedis is a small, educational **Redis-inspired** in-memory key-value server built with **C#** and **TCP**.  
Clients connect over TCP, send simple **line-based text commands**, and receive responses immediately.

---

## Why MiniRedis?

- Lightweight and easy to understand (no external dependencies)
- **Multi-client** support (each connection handled in its own Task)
- **Thread-safe** in-memory storage using `ConcurrentDictionary`
- Super easy to test with **telnet** or **netcat**
- Great for learning: TCP, streams, async/await, concurrency

---

## Features / Commands

| Command | Example | Description |
|--------|---------|-------------|
| `SET`  | `SET name rasol` | Set a value for a key |
| `GET`  | `GET name` | Get the value of a key |
| `DEL`  | `DEL name` | Delete a key |
| `KEYS` | `KEYS` | List all keys |
| `EXIT` | `EXIT` | Close the client connection |

---




## Quick Start

Run the server
Default port is **5000**:

```bash
dotnet run

![alt text](image.png)


Connect & Test
![alt text](image-2.png)

telnet 127.0.0.1 5000
![alt text](image-1.png)
or

build Dockerfile

Technical Notes
    TCP server built with TcpListener / TcpClient
    UTF-8 text I/O via:
    StreamReader
    StreamWriter (AutoFlush = true)
    In-memory database:
    ConcurrentDictionary<string, string>
    Multiple clients:
    accepted in an async loop
    each client handled concurrently in its own Task
    Clean shutdown:
    CancellationTokenSource
    listener.Stop()

Current Limitations
This project is intentionally minimal and educational:

Data is in-memory only (no persistence)
No authentication/security
No TTL/expire support
Not using the Redis RESP protocol (simple text protocol instead)
Only string values
Roadmap / Ideas
Want to level it up?

 Persistence (snapshot / append-only log)
 TTL support (EXPIRE, TTL)
 Multiple databases (like Redis)
 Implement RESP protocol (Redis-compatible clients)
 Add PING
 Logging, metrics, and unit tests
Contributing
Issues and PRs are welcome.

If you add a new command/feature, please also add:

a README usage example
(ideally) a small test