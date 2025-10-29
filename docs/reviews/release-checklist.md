# Release Checklist

This checklist ensures every production release is consistent, traceable, and automated.

---

## Pre‑Release
- [ ] All feature branches merged into `main`.
- [ ] `main` build is green (CI passes on all platforms).
- [ ] ADRs updated for any new decisions.
- [ ] Capsules updated to reflect new practices.
- [ ] Radar updated and reviewed.
- [ ] Changelog updated (auto‑generated PR merged).

---

## Tagging
- [ ] Decide release version (e.g., `v1.2.0`).
- [ ] Create annotated tag:
  ```bash
  git checkout main
  git pull origin main
  git tag -a v1.2.0 -m "Release v1.2.0"
  git push origin v1.2.0
