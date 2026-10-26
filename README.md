## About
A **Course Management System** built while completing the *Entity Framework Core Fundamentals* course (Programming Advices). The goal was to understand how EF Core works internally, not just how to use it.

## Entities
Students, Courses, Enrollments, Departments

## What this project covers
- Entity models, `DbContext`, and `DbSet`
- Relationships between entities
- LINQ queries and how they translate to SQL
- Tracking vs No-Tracking queries
- Eager loading with `Include`, pagination, and reports
- Change Tracker and entity states
- CRUD operations with async patterns
- Batch inserts (`AddRange` + a single `SaveChangesAsync`)
- Bulk updates with `ExecuteUpdateAsync`
- Migrations
- Inspecting the generated SQL
