# Task King – Task Management

## Description
Task management system allowing creating, organizing, and tracking tasks.

---

## Tech-stack
- Backend: .NET Web API
- Frontend: Static HTML/JS via wwwroot
- Database: MariaDB with Flyway versioning
- ORM: Entity Framework Core (Npgsql)

---

## Architecture
A simple 3-layer architecture:

- Presentation layer: Static HTML/JS served from `wwwroot`
- Application layer: Services handling business logic
- Data layer: EF Core with PostgreSQL

Structure:
Controllers → Services → DbContext

---

## Feature plan

### Week 5
Kick-off week - no features to be planned here

---

### Week 6
- Feature 1: Create tasks
- Feature 2: View all tasks

---

### Week 7
Winter vacation - nothing planned.

---

### Week 8
- Feature 1: Update tasks
- Feature 2: Delete tasks

---

### Week 9
- Feature 1: Add status to tasks (Todo, In Progress, Done)
- Feature 2: Filter tasks by status

---

### Week 10
- Feature 1: Add priority (Low, Medium, High)
- Feature 2: Sort tasks by priority or creation date

---

### Week 11
- Feature 1: Categories (create categories)
- Feature 2: Assign tasks to categories

---

### Week 12
- Feature 1: Due dates
- Feature 2: Show overdue tasks

---

### Week 13
- Feature 1: Comments on tasks
- Feature 2: List comments per task

---

### Week 14
Easter vacation - nothing planned.

---

### Week 15
- Feature 1: Search tasks (by title/description)
- Feature 2: Pagination for task list

---

### Week 16
- Feature 1: Tag system (many-to-many with tasks)
- Feature 2: Filter by tags

---

### Week 17
- Feature 1: Activity log (track changes to tasks)**
- Feature 2: Export tasks (CSV)**
- **Not sure if I get to this week.**

---