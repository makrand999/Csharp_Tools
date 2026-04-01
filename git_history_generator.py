#!/usr/bin/env python3
"""
C# Git History Generator

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

     1 python3 git_history_generator.py
    You'll see:

     1 📅 Choose date range option:
     2    1. Use preset (January - March 2026)
     3    2. Enter custom start and end dates
     4    3. Enter start date and number of days
     5
     6    Select option [1]:

    2. Non-interactive (command line args):

     1 python3 git_history_generator.py -s 2026-01-01 -e
       2026-03-31
     2 python3 git_history_generator.py -s 2025-12-01 -e
       2026-02-28

    3. Preview first (dry-run):

     1 python3 git_history_generator.py --dry-run

    Features:
     - ✅ Auto-detects C# projects
     - ✅ Projects ≤3 files → single commit
     - ✅ Projects >3 files → batched commits (3 files each)
     - ✅ Spreads commits evenly across date range
     - ✅ Interactive or command-line date input
     - ✅ Handles nested project folders
"""

import os
import sys
import subprocess
import argparse
from datetime import datetime, timedelta
from pathlib import Path
from typing import List, Tuple, Optional
from dataclasses import dataclass


@dataclass
class CSharpProject:
    """Represents a C# project folder."""
    path: Path
    program_number: int
    category: str
    files: List[Path]
    file_count: int


@dataclass
class CommitPlan:
    """Plans a commit with files and date."""
    files: List[Path]
    date: datetime
    message: str
    program_number: Optional[int] = None


def run_git(args: List[str], cwd: str = None, env: dict = None):
    """Run a git command."""
    cmd = ["git"] + args
    subprocess.run(cmd, cwd=cwd, env=env, check=True, capture_output=True)


def find_csharp_projects(root_dir: Path) -> List[CSharpProject]:
    """
    Find all C# projects anywhere inside root_dir.

    A project is any directory containing at least one .csproj file.
    """

    projects = []
    visited = set()  # prevent nested duplicates

    for csproj in root_dir.rglob("*.csproj"):
        project_dir = csproj.parent

        # Skip if already covered by a parent project
        if any(str(project_dir).startswith(v) for v in visited):
            continue

        visited.add(str(project_dir))

        # Collect files (ignore build + git junk)
        all_files = [
            f for f in project_dir.rglob("*")
            if f.is_file() and not any(p in str(f) for p in ["/bin/", "/obj/", "/.git/"])
        ]

        # Extract program number (optional)
        prog_num = 0
        parts = project_dir.name.split("-")
        if parts[0].isdigit():
            prog_num = int(parts[0])

        projects.append(CSharpProject(
            path=project_dir,
            program_number=prog_num,
            category=project_dir.parent.name,  # dynamic category
            files=all_files,
            file_count=len(all_files)
        ))

    # Sort safely (projects without numbers go last)
    projects.sort(key=lambda p: (p.program_number == 0, p.program_number))

    return projects


def generate_commit_dates(
    start_date: datetime,
    end_date: datetime,
    total_commits: int
) -> List[datetime]:
    """Generate evenly distributed dates across the range."""
    if total_commits <= 1:
        return [start_date]
    
    total_days = (end_date - start_date).days
    if total_days < 1:
        total_days = 1
    
    dates = []
    for i in range(total_commits):
        # Spread commits across the date range
        day_offset = int((i / (total_commits - 1)) * total_days) if total_commits > 1 else 0
        commit_date = start_date + timedelta(days=day_offset)
        
        # Add random-ish time variation
        hour = 8 + (i % 12)  # 8 AM to 7 PM
        minute = 15 + (i * 7) % 45  # Distributed minutes
        
        commit_date = commit_date.replace(hour=hour, minute=minute, second=0)
        dates.append(commit_date)
    
    return dates


def plan_commits(
    projects: List[CSharpProject],
    start_date: datetime,
    end_date: datetime
) -> List[CommitPlan]:
    """
    Plan all commits based on project structure.
    
    Rules:
    - Projects with < 3 files: single commit per project
    - Projects with >= 3 files: group into batches of 3
    - Tools category: commit all together (small utility scripts)
    """
    commit_plans = []
    
    # Count total commits needed
    total_commits = 0
    for project in projects:
        if project.file_count < 3:
            total_commits += 1
        else:
            # Group into batches of 3 files
            batches = (project.file_count + 2) // 3
            total_commits += batches
    
    # Generate dates for all commits
    commit_dates = generate_commit_dates(start_date, end_date, total_commits)
    date_index = 0
    
    for project in projects:
        if project.file_count <= 3:
            # Small project: single commit with all files
            commit_plans.append(CommitPlan(
                files=project.files,
                date=commit_dates[date_index],
                message=f"Add program #{project.program_number}: {project.path.name}",
                program_number=project.program_number
            ))
            date_index += 1
        else:
            # Large project: batch commits of 3 files
            for i in range(0, len(project.files), 3):
                batch_files = project.files[i:i+3]
                batch_num = (i // 3) + 1
                total_batches = (len(project.files) + 2) // 3
                
                # Only include program number in first batch
                if batch_num == 1:
                    msg = f"Add program #{project.program_number}: {project.path.name} (part {batch_num}/{total_batches})"
                else:
                    msg = f"Add program #{project.program_number}: {project.path.name} (part {batch_num}/{total_batches})"
                
                commit_plans.append(CommitPlan(
                    files=batch_files,
                    date=commit_dates[date_index],
                    message=msg,
                    program_number=project.program_number if batch_num == 1 else None
                ))
                date_index += 1
    
    return commit_plans


def create_git_history(
    root_dir: Path,
    start_date: datetime,
    end_date: datetime,
    dry_run: bool = False
):
    """Create the git history."""
    
    print(f"📁 Scanning for C# projects in: {root_dir}")
    projects = find_csharp_projects(root_dir)
    print(f"✅ Found {len(projects)} C# projects")
    
    if not projects:
        print("❌ No C# projects found!")
        return
    
    # Show project summary
    print("\n📊 Project Summary:")
    for proj in projects[:5]:
        print(f"   #{proj.program_number}: {proj.path.name} ({proj.file_count} files)")
    if len(projects) > 5:
        print(f"   ... and {len(projects) - 5} more")
    
    print(f"\n📅 Date range: {start_date.strftime('%Y-%m-%d')} to {end_date.strftime('%Y-%m-%d')}")
    
    # Plan commits
    commit_plans = plan_commits(projects, start_date, end_date)
    print(f"📝 Planned {len(commit_plans)} commits")
    
    if dry_run:
        print("\n🔍 DRY RUN - Commit plan:")
        for i, plan in enumerate(commit_plans[:10]):
            print(f"   {i+1}. {plan.date.strftime('%Y-%m-%d %H:%M')} - {plan.message}")
            for f in plan.files[:3]:
                print(f"      - {f.relative_to(root_dir)}")
            if len(plan.files) > 3:
                print(f"      ... and {len(plan.files) - 3} more files")
        if len(commit_plans) > 10:
            print(f"   ... and {len(commit_plans) - 10} more commits")
        return
    
    # Initialize git repo
    print("\n🔧 Initializing git repository...")
    run_git(["init"], cwd=str(root_dir))
    run_git(["config", "user.email", "dev@example.com"], cwd=str(root_dir))
    run_git(["config", "user.name", "Developer"], cwd=str(root_dir))
    
    # Create .gitignore
    gitignore_content = """bin/
obj/
*.user
*.suo
*.cache
.vs/
.vscode/
*.log
.qwen/
"""
    gitignore_path = root_dir / ".gitignore"
    gitignore_path.write_text(gitignore_content)
    
    # Initial commit
    run_git(["add", ".gitignore"], cwd=str(root_dir))
    env = os.environ.copy()
    env["GIT_AUTHOR_DATE"] = start_date.strftime("%Y-%m-%dT%H:%M:%S")
    env["GIT_COMMITTER_DATE"] = start_date.strftime("%Y-%m-%dT%H:%M:%S")
    run_git(
        ["commit", "-m", "Initial commit: Project setup"],
        cwd=str(root_dir),
        env=env
    )
    print("✅ Initial commit created")
    
    # Create commits
    print("\n📦 Creating commits...")
    for i, plan in enumerate(commit_plans):
        # Stage files
        for f in plan.files:
            try:
                rel_path = f.relative_to(root_dir)
                run_git(["add", str(rel_path)], cwd=str(root_dir))
            except ValueError:
                pass  # File outside repo
        
        # Check if there's anything to commit
        result = subprocess.run(
            ["git", "status", "--porcelain"],
            cwd=str(root_dir),
            capture_output=True,
            text=True
        )
        if not result.stdout.strip():
            continue  # Nothing to commit
        
        # Set commit date
        env = os.environ.copy()
        env["GIT_AUTHOR_DATE"] = plan.date.strftime("%Y-%m-%dT%H:%M:%S")
        env["GIT_COMMITTER_DATE"] = plan.date.strftime("%Y-%m-%dT%H:%M:%S")
        
        run_git(["commit", "-m", plan.message], cwd=str(root_dir), env=env)
        
        # Progress indicator
        if (i + 1) % 50 == 0 or i == len(commit_plans) - 1:
            print(f"   Committed {i + 1}/{len(commit_plans)} ({plan.date.strftime('%Y-%m-%d')})")
    
    # Summary
    print("\n✅ Git history created successfully!")
    
    # Show stats
    result = subprocess.run(
        ["git", "rev-list", "--count", "HEAD"],
        cwd=str(root_dir),
        capture_output=True,
        text=True
    )
    total_commits = int(result.stdout.strip())
    
    result = subprocess.run(
        ["git", "log", "--reverse", "--format=%ad", "--date=short"],
        cwd=str(root_dir),
        capture_output=True,
        text=True
    )
    first_date = result.stdout.strip().split("\n")[0] if result.stdout.strip() else "N/A"
    
    result = subprocess.run(
        ["git", "log", "--format=%ad", "--date=short"],
        cwd=str(root_dir),
        capture_output=True,
        text=True
    )
    last_date = result.stdout.strip().split("\n")[0] if result.stdout.strip() else "N/A"
    
    print(f"\n📊 Statistics:")
    print(f"   Total commits: {total_commits}")
    print(f"   Date range: {first_date} to {last_date}")
    
    print("\n📜 Last 5 commits:")
    result = subprocess.run(
        ["git", "log", "--oneline", "-5"],
        cwd=str(root_dir),
        capture_output=True,
        text=True
    )
    for line in result.stdout.strip().split("\n"):
        print(f"   {line}")


def get_date_input(prompt: str, default: str) -> datetime:
    """Prompt user for a date with a default value."""
    while True:
        user_input = input(f"{prompt} [{default}]: ").strip()
        date_str = user_input if user_input else default
        
        try:
            return datetime.strptime(date_str, "%Y-%m-%d")
        except ValueError:
            print("   ❌ Invalid format. Please use YYYY-MM-DD (e.g., 2026-01-01)")


def get_timespan_input() -> tuple[datetime, datetime]:
    """Prompt user for timespan options."""
    print("\n📅 Choose date range option:")
    print("   1. Use preset (January - March 2026)")
    print("   2. Enter custom start and end dates")
    print("   3. Enter start date and number of days")
    
    choice = input("\n   Select option [1]: ").strip() or "1"
    
    if choice == "2":
        start = get_date_input("   Start date", "2026-01-01")
        end = get_date_input("   End date", "2026-03-31")
        return start, end
    elif choice == "3":
        start = get_date_input("   Start date", "2026-01-01")
        days_str = input("   Number of days [90]: ").strip() or "90"
        try:
            days = int(days_str)
            end = start + timedelta(days=days)
            return start, end
        except ValueError:
            print("   ❌ Invalid number, using 90 days")
            return start, start + timedelta(days=90)
    else:
        # Default: Jan 1 - Mar 31, 2026 (90 days)
        return datetime(2026, 1, 1), datetime(2026, 3, 31)


def main():
    parser = argparse.ArgumentParser(
        description="Create realistic git history for C# projects",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  %(prog)s                          # Interactive mode (prompts for dates)
  %(prog)s -s 2026-01-01 -e 2026-03-31  # Non-interactive
  %(prog)s --start 2025-12-01 --end 2026-02-28
  %(prog)s --dry-run                # Preview without creating commits
        """
    )
    
    parser.add_argument(
        "-d", "--directory",
        type=Path,
        default=Path("."),
        help="Root directory of C# projects (default: current directory)"
    )
    
    parser.add_argument(
        "-s", "--start",
        type=str,
        default=None,
        help="Start date YYYY-MM-DD (default: prompt user)"
    )
    
    parser.add_argument(
        "-e", "--end",
        type=str,
        default=None,
        help="End date YYYY-MM-DD (default: prompt user)"
    )
    
    parser.add_argument(
        "--dry-run",
        action="store_true",
        help="Preview commit plan without creating commits"
    )
    
    parser.add_argument(
        "-i", "--interactive",
        action="store_true",
        help="Force interactive mode (prompt for all options)"
    )
    
    parser.add_argument(
        "--interactive-mode",
        action="store_true",
        help="Same as -i, force interactive mode"
    )
    
    args = parser.parse_args()
    
    # Get dates (interactive or from args)
    if args.interactive or args.interactive_mode or not args.start or not args.end:
        start_date, end_date = get_timespan_input()
    else:
        try:
            start_date = datetime.strptime(args.start, "%Y-%m-%d")
            end_date = datetime.strptime(args.end, "%Y-%m-%d")
        except ValueError as e:
            print(f"❌ Invalid date format. Use YYYY-MM-DD")
            sys.exit(1)
    
    if start_date > end_date:
        print("❌ Start date must be before end date")
        sys.exit(1)
    
    # Resolve directory
    root_dir = args.directory.resolve()
    if not root_dir.exists():
        print(f"❌ Directory not found: {root_dir}")
        sys.exit(1)
    
    # Remove existing .git if present
    git_dir = root_dir / ".git"
    if git_dir.exists():
        import shutil
        print(f"🗑️  Removing existing .git directory...")
        shutil.rmtree(git_dir)
    
    # Create history
    create_git_history(root_dir, start_date, end_date, args.dry_run)


if __name__ == "__main__":
    main()
