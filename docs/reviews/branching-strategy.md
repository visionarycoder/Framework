# Branching Strategy Playbook

## Purpose

Define a clear, reproducible branching model aligned with Nerdbank.GitVersioning (NBGV) and our CI/CD pipelines.

---

## Branch Types

### `main`

- **Purpose:** Integration branch for stable development.
- **Versioning:** `-preview.{height}` prereleases.
- **Publishing:** Nightly/previews to GitHub Packages.
- **Rules:**
  - All PRs must pass CI.
  - No direct commits; always via PR.

### `feature/*`

- **Purpose:** Experimental or short-lived work.
- **Versioning:** `-alpha.{height}` prereleases.
- **Publishing:** GitHub Packages only (optional).
- **Rules:**
  - Branch from `main`.
  - Merge back via PR with review.

### `release/vX.Y`

- **Purpose:** Stabilization branch for upcoming release.
- **Versioning:** `-rc.{height}` prereleases.
- **Publishing:** GitHub Packages (release candidates).
- **Rules:**
  - Only bug fixes, docs, and release prep.
  - No new features.
  - Cut from `main` when feature set is frozen.

### Tags `vX.Y.Z`

- **Purpose:** Production-ready releases.
- **Versioning:** Clean semantic version (no suffix).
- **Publishing:** NuGet.org (stable) + GitHub Packages.
- **Rules:**
  - Tag only from `release/*` or `main` after sign-off.
  - Tagging triggers CI/CD to publish stable package and changelog.

---

## Flow Summary

```plaintext
feature/*  →  main (preview)  →  release/vX.Y (rc)  →  tag vX.Y.Z (stable)
```

```Mermaid
gitGraph
   commit id: "Init"
   branch feature/new-api
   checkout feature/new-api
   commit id: "Feature work"
   commit id: "More feature work"
   checkout main
   merge feature/new-api id: "Merge feature → main"
   commit id: "Preview build"
   branch release/v1.1
   checkout release/v1.1
   commit id: "Stabilization"
   commit id: "Bugfix"
   checkout main
   merge release/v1.1 id: "Merge release → main"
   checkout release/v1.1
   commit id: "Final RC"
   tag: "v1.1.0"
   checkout main
   merge release/v1.1 id: "Release v1.1.0"
```

---

## Related Visuals

- [Quarterly Review Timeline](quarterly-radar-review.md#quarterly-architecture-governance-cycle)
- [Radar Quadrants](../../best-practices/radar.md#visual-radar-mermaid)
