#!/usr/bin/env python3
"""
C# Git History Generator (optimized)

Creates a realistic git history for C# projects by analyzing folder structure
and spreading commits across a date range.

Features:
- Auto-detects C# project structure (.csproj, Program.cs, README.md)
- Spreads commits across specified date range
- Groups small projects (< 3 files) into single commits
- Groups large projects into batches of 3 files
- Handles nested project folders (e.g., basics/006-hello-world/HelloWorld/)

Usage options:

    1. Interactive mode (prompts for dates):
       python3 git_history_generator.py

    2. Non-interactive (command line args):
       python3 git_history_generator.py -s 2026-01-01 -e 2026-03-31

    3. Preview first (dry-run):
       python3 git_history_generator.py --dry-run
"""

import os
import sys
import subprocess
import argparse
import shutil
from datetime import datetime, timedelta
from pathlib import Path
from typing import List, Optional
from dataclasses import dataclass, field


# ---------------------------------------------------------------------------
# Data structures
# ---------------------------------------------------------------------------

@dataclass
class CSharpProject:
    path: Path
    program_number: int
    files: List[Path]

    @property
    def file_count(self) -> int:
        return len(self.files)


@dataclass
class CommitPlan:
    files: List[Path]
    date: datetime
    message: str
    program_number: Optional[int] = None


# ---------------------------------------------------------------------------
# Git helpers
# ---------------------------------------------------------------------------

_GIT_ENV_BASE: dict = {}   # populated once in create_git_history

def _run(args: List[str], cwd: str, env: dict = None, capture: bool = True) -> subprocess.CompletedProcess:
    """Thin wrapper around subprocess.run with sensible defaults."""
    return subprocess.run(
        args, cwd=cwd, env=env or _GIT_ENV_BASE,
        check=True, capture_output=capture,
    )


def _git(args: List[str], cwd: str, env: dict = None) -> subprocess.CompletedProcess:
    return _run(["git"] + args, cwd=cwd, env=env)


def _git_out(args: List[str], cwd: str) -> str:
    result = _run(["git"] + args, cwd=cwd)
    return result.stdout.decode().strip()


# ---------------------------------------------------------------------------
# Project discovery
# ---------------------------------------------------------------------------

_SKIP_DIRS = frozenset({"bin", "obj", ".git"})


def find_csharp_projects(root_dir: Path) -> List[CSharpProject]:
    """
    Walk the tree once and collect all .csproj directories with their files.
    Avoids redundant rglob() calls by doing a single os.walk() pass.
    """
    projects: List[CSharpProject] = []
    project_roots: List[str] = []          # absolute paths already claimed

    # First pass: collect all .csproj directories (sorted for reproducibility)
    csproj_dirs = sorted(
        {str(p.parent) for p in root_dir.rglob("*.csproj")},
        key=lambda d: d.lower(),
    )

    root_str = str(root_dir)

    for proj_dir_str in csproj_dirs:
        # Skip if covered by an already-discovered parent project
        if any(proj_dir_str.startswith(pr) for pr in project_roots):
            continue
        project_roots.append(proj_dir_str + os.sep)

        proj_dir = Path(proj_dir_str)

        # Collect files with a single walk rooted at this directory
        all_files: List[Path] = []
        for dirpath, dirnames, filenames in os.walk(proj_dir_str):
            # Prune skip dirs in-place (modifying dirnames stops os.walk descent)
            dirnames[:] = [d for d in dirnames if d not in _SKIP_DIRS]
            for fname in filenames:
                all_files.append(Path(dirpath) / fname)

        # Extract optional leading program number from directory name
        parts = proj_dir.name.split("-")
        prog_num = int(parts[0]) if parts[0].isdigit() else 0

        projects.append(CSharpProject(
            path=proj_dir,
            program_number=prog_num,
            files=all_files,
        ))

    projects.sort(key=lambda p: (p.program_number == 0, p.program_number))
    return projects


# ---------------------------------------------------------------------------
# Commit planning
# ---------------------------------------------------------------------------

def generate_commit_dates(
    start_date: datetime,
    end_date: datetime,
    total_commits: int,
) -> List[datetime]:
    if total_commits <= 1:
        return [start_date]

    total_days = max((end_date - start_date).days, 1)
    n = total_commits - 1

    return [
        (start_date + timedelta(days=int(i / n * total_days))).replace(
            hour=8 + (i % 12),
            minute=15 + (i * 7) % 45,
            second=0,
            microsecond=0,
        )
        for i in range(total_commits)
    ]


def plan_commits(
    projects: List[CSharpProject],
    start_date: datetime,
    end_date: datetime,
) -> List[CommitPlan]:
    # Count total commits upfront
    total_commits = sum(
        1 if p.file_count <= 3 else (p.file_count + 2) // 3
        for p in projects
    )

    commit_dates = generate_commit_dates(start_date, end_date, total_commits)
    date_idx = 0
    plans: List[CommitPlan] = []

    for project in projects:
        name = project.path.name
        num = project.program_number

        if project.file_count <= 3:
            plans.append(CommitPlan(
                files=project.files,
                date=commit_dates[date_idx],
                message=f"Add program #{num}: {name}",
                program_number=num,
            ))
            date_idx += 1
        else:
            total_batches = (project.file_count + 2) // 3
            for batch_idx, i in enumerate(range(0, project.file_count, 3)):
                batch_num = batch_idx + 1
                plans.append(CommitPlan(
                    files=project.files[i:i + 3],
                    date=commit_dates[date_idx],
                    message=(
                        f"Add program #{num}: {name} (part {batch_num}/{total_batches})"
                    ),
                    program_number=num if batch_num == 1 else None,
                ))
                date_idx += 1

    return plans


# ---------------------------------------------------------------------------
# Core: create git history
# ---------------------------------------------------------------------------

_GITIGNORE = "bin/\nobj/\n*.user\n*.suo\n*.cache\n.vs/\n.vscode/\n*.log\n.qwen/\n"


def create_git_history(
    root_dir: Path,
    start_date: datetime,
    end_date: datetime,
    dry_run: bool = False,
) -> None:
    root_str = str(root_dir)

    print(f"📁 Scanning for C# projects in: {root_dir}")
    projects = find_csharp_projects(root_dir)
    print(f"✅ Found {len(projects)} C# projects")

    if not projects:
        print("❌ No C# projects found!")
        return

    print("\n📊 Project summary:")
    for proj in projects[:5]:
        print(f"   #{proj.program_number}: {proj.path.name} ({proj.file_count} files)")
    if len(projects) > 5:
        print(f"   ... and {len(projects) - 5} more")

    print(f"\n📅 Date range: {start_date:%Y-%m-%d} → {end_date:%Y-%m-%d}")

    commit_plans = plan_commits(projects, start_date, end_date)
    print(f"📝 Planned {len(commit_plans)} commits")

    if dry_run:
        print("\n🔍 DRY RUN — commit plan (first 10):")
        for i, plan in enumerate(commit_plans[:10]):
            print(f"   {i+1:3d}. {plan.date:%Y-%m-%d %H:%M}  {plan.message}")
            for f in plan.files[:3]:
                print(f"         {f.relative_to(root_dir)}")
            if len(plan.files) > 3:
                print(f"         … and {len(plan.files) - 3} more")
        if len(commit_plans) > 10:
            print(f"   … and {len(commit_plans) - 10} more commits")
        return

    # ------------------------------------------------------------------
    # Initialise repo
    # ------------------------------------------------------------------
    print("\n🔧 Initialising git repository…")
    _git(["init"], root_str)
    _git(["config", "user.email", "dev@example.com"], root_str)
    _git(["config", "user.name", "Developer"], root_str)

    (root_dir / ".gitignore").write_text(_GITIGNORE)

    init_env = {**os.environ,
                "GIT_AUTHOR_DATE": start_date.strftime("%Y-%m-%dT%H:%M:%S"),
                "GIT_COMMITTER_DATE": start_date.strftime("%Y-%m-%dT%H:%M:%S")}
    _git(["add", ".gitignore"], root_str)
    _git(["commit", "-m", "Initial commit: Project setup"], root_str, env=init_env)
    print("✅ Initial commit created")

    # ------------------------------------------------------------------
    # Batch commits — key optimisations:
    #   1. `git add --` with multiple paths in ONE subprocess call per batch
    #   2. Check `git status --porcelain` once per batch (not per file)
    #   3. Build the env dict once per commit (reuse os.environ copy)
    # ------------------------------------------------------------------
    print("\n📦 Creating commits…")
    total = len(commit_plans)
    skipped = 0

    base_env = os.environ.copy()   # copy once; mutate two keys per iteration

    for i, plan in enumerate(commit_plans):
        # Convert all file paths to repo-relative strings in one pass
        rel_paths: List[str] = []
        for f in plan.files:
            try:
                rel_paths.append(str(f.relative_to(root_dir)))
            except ValueError:
                pass

        if rel_paths:
            # Single subprocess call for all files in this batch
            _git(["add", "--"] + rel_paths, root_str)

        # Check staging area before committing
        status = subprocess.run(
            ["git", "status", "--porcelain"],
            cwd=root_str, capture_output=True, text=True,
        )
        if not status.stdout.strip():
            skipped += 1
            continue

        date_str = plan.date.strftime("%Y-%m-%dT%H:%M:%S")
        commit_env = {**base_env,
                      "GIT_AUTHOR_DATE": date_str,
                      "GIT_COMMITTER_DATE": date_str}
        _git(["commit", "-m", plan.message], root_str, env=commit_env)

        # Inline progress (no newline spam)
        done = i + 1
        if done % 10 == 0 or done == total:
            pct = done * 100 // total
            bar = "█" * (pct // 5) + "░" * (20 - pct // 5)
            print(f"\r   [{bar}] {done}/{total} ({plan.date:%Y-%m-%d})", end="", flush=True)

    print()  # newline after progress bar

    # ------------------------------------------------------------------
    # Summary
    # ------------------------------------------------------------------
    total_commits = int(_git_out(["rev-list", "--count", "HEAD"], root_str))
    first_date = _git_out(["log", "--reverse", "--format=%ad", "--date=short"], root_str).split("\n")[0]
    last_date  = _git_out(["log", "--format=%ad", "--date=short"], root_str).split("\n")[0]

    print(f"\n✅ Done! {total_commits} commits ({first_date} → {last_date})")
    if skipped:
        print(f"   ℹ️  {skipped} batches skipped (nothing to stage)")

    print("\n📜 Last 5 commits:")
    for line in _git_out(["log", "--oneline", "-5"], root_str).split("\n"):
        print(f"   {line}")


# ---------------------------------------------------------------------------
# CLI helpers
# ---------------------------------------------------------------------------

def _parse_date(s: str) -> datetime:
    try:
        return datetime.strptime(s, "%Y-%m-%d")
    except ValueError:
        raise argparse.ArgumentTypeError(f"Invalid date '{s}'. Use YYYY-MM-DD.")


def _prompt_date(prompt: str, default: str) -> datetime:
    while True:
        raw = input(f"{prompt} [{default}]: ").strip() or default
        try:
            return datetime.strptime(raw, "%Y-%m-%d")
        except ValueError:
            print("   ❌ Use YYYY-MM-DD (e.g. 2026-01-15)")


def _prompt_timespan() -> tuple[datetime, datetime]:
    print("\n📅 Choose date range:")
    print("   1. Preset  (2026-01-01 → 2026-03-31)")
    print("   2. Custom start + end dates")
    print("   3. Start date + number of days")

    choice = input("\n   Select [1]: ").strip() or "1"

    if choice == "2":
        start = _prompt_date("   Start date", "2026-01-01")
        end   = _prompt_date("   End date",   "2026-03-31")
        return start, end

    if choice == "3":
        start = _prompt_date("   Start date", "2026-01-01")
        raw = input("   Number of days [90]: ").strip() or "90"
        try:
            days = int(raw)
        except ValueError:
            print("   ❌ Using 90 days")
            days = 90
        return start, start + timedelta(days=days)

    return datetime(2026, 1, 1), datetime(2026, 3, 31)


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(
        description="Create realistic git history for C# projects",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  %(prog)s                                  # interactive
  %(prog)s -s 2026-01-01 -e 2026-03-31     # non-interactive
  %(prog)s --dry-run                        # preview only
        """,
    )
    parser.add_argument("-d", "--directory", type=Path, default=Path("."),
                        help="Root directory (default: current dir)")
    parser.add_argument("-s", "--start", type=str, default=None,
                        help="Start date YYYY-MM-DD")
    parser.add_argument("-e", "--end",   type=str, default=None,
                        help="End date YYYY-MM-DD")
    parser.add_argument("--dry-run", action="store_true",
                        help="Preview without creating commits")
    parser.add_argument("-i", "--interactive", action="store_true",
                        help="Force interactive date prompts")

    args = parser.parse_args()

    # Resolve dates
    if args.interactive or not (args.start and args.end):
        start_date, end_date = _prompt_timespan()
    else:
        try:
            start_date = datetime.strptime(args.start, "%Y-%m-%d")
            end_date   = datetime.strptime(args.end,   "%Y-%m-%d")
        except ValueError:
            print("❌ Invalid date format — use YYYY-MM-DD")
            sys.exit(1)

    if start_date > end_date:
        print("❌ Start date must be before end date")
        sys.exit(1)

    root_dir = args.directory.resolve()
    if not root_dir.exists():
        print(f"❌ Directory not found: {root_dir}")
        sys.exit(1)

    # Remove stale .git
    git_dir = root_dir / ".git"
    if git_dir.exists():
        print("🗑️  Removing existing .git directory…")
        shutil.rmtree(git_dir)

    create_git_history(root_dir, start_date, end_date, args.dry_run)


if __name__ == "__main__":
    main()
