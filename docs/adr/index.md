# Architecture Decision Records (ADR) Index

This index provides a chronological overview of all ADRs in this repository.  
Each ADR captures a significant architectural decision, its context, and consequences.  
ADRs are **immutable**: once accepted, they remain as historical records. If a decision changes, a new ADR supersedes the old one.

---

## ADRs

| ADR ID     | Title                                                            | Status    | Date       | Supersedes |
|------------|------------------------------------------------------------------|-----------|------------|-------------|
| ADR-0001   | Establish Solution Architect Radar and Best Practice Capsules    | Accepted  | 2025-10-04 | –           |
| ADR-0002   | Adopt GitOps for CI/CD                                           | Accepted  | 2025-10-04 | –           |
| ADR-0003   | XML Documentation Generation and Unit Testing Strategy           | Accepted  | 2025-10-16 | –           |
| ADR-0004   | Modular Copilot Instruction Architecture                          | Accepted  | 2025-11-14 | –           |

---

## Status Legend

- **Proposed** → Under discussion, not yet accepted.  
- **Accepted** → Decision is in effect.  
- **Superseded** → Replaced by a newer ADR.  
- **Deprecated** → No longer relevant, but kept for historical context.  

---

## How to Add a New ADR

1. Copy the [ADR template](./ADR-template.md) into a new file:  
   `ADR-XXXX.md` (increment the number).  
2. Fill in the details (context, decision, consequences, etc.).  
3. Update this `index.md` with the new ADR entry.  
4. If the new ADR supersedes an old one, update the **Supersedes** column.  

---

## References

- [Architecture Decision Records (Joel Parker Henderson)](https://github.com/joelparkerhenderson/architecture_decision_record)
- [ThoughtWorks Tech Radar](https://www.thoughtworks.com/radar)
