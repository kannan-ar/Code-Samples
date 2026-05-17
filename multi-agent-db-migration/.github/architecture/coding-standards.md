# Coding Standards

## Naming

- Use PascalCase for public members.
- Use camelCase for local variables.
- Use descriptive names.

---

## Async

- Use async/await consistently.
- Avoid .Result and .Wait().
- Propagate CancellationToken.

---

## Logging

- Use ILogger<T>.
- Use structured logging.
- Include batch identifiers.

---

## Error Handling

- Catch only expected exceptions.
- Preserve stack traces.
- Log contextual details.

---

## Resource Management

- Dispose IDisposable objects.
- Use await using where applicable.
- Avoid connection leaks.