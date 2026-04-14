# Task King – Task Management

## Description
Task management system allowing creation, organizing, and tracking of tasks.

---

## Tech Stack

### Backend
- .NET 9 Web API
- Entity Framework Core

### Database
- MariaDB (Dockerized)
- Flyway (database migrations)

### Frontend
- Static HTML/JavaScript (served via `wwwroot`)

### DevOps / Infrastructure
- Docker + Docker Compose
- GitHub Actions CI/CD
- k6 (load testing)
- TestCafe (E2E testing)
- GitHub Container Registry (GHCR)

---

## Architecture

### High-level structure
Presentation → API → Service Layer → Database

- Presentation layer: `wwwroot` (HTML/JS)
- API layer: Controllers
- Service layer: Business logic
- Data layer: EF Core + MariaDB

---

## DevOps Pipeline

### CI/CD Flow
1. Build .NET application
2. Run unit tests
3. Build Docker image
4. Push image to GHCR
5. Deploy to staging (dev branch)
6. Run E2E test(s) (TestCafe)
7. Run load test(s) (k6)
8. Deploy to production (main branch)

---


## Feature plan
(Not doing individual feature branches because I'm solo, **chore:** in commit means new feature)

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