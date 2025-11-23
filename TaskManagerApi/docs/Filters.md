# TaskManager API — Listing Filters (`GET /api/tasks`)

This document explains how to use **filters**, **sorting**, and **pagination** of the `GET /api/tasks` endpoint and provides practical examples (cURL and direct URL).

> The listing accepts a query object (`TaskQuery`) with the parameters below and returns a paged envelope with items and metadata (page, size, and total).

## Query Parameters

| Parameter     | Type   | What it does                                                             | Notes                                                                    |
| ------------- | ------ | ------------------------------------------------------------------------ | ------------------------------------------------------------------------ |
| `UserId`      | GUID   | Filters tasks by a specific user (exact match).                          |                                                                          |
| `CategoryId`  | int    | Filters by specific category (exact match).                              |                                                                          |
| `IsCompleted` | bool   | Filters by completed (`true`) or pending (`false`).                      |                                                                          |
| `Search`      | string | **Case-insensitive** search for **contains** in `Title` **or** `Description`. | Uses `ILIKE` with pattern `"%{Search}%"`. If empty/whitespace, ignored. |
| `SortBy`      | string | Sorting field.                                                           | Accepts: `title`, `created`, `updatedAt`. Invalid value falls back to default. |
| `SortDir`     | string | Sorting direction.                                                       | `asc` or `desc`. Default: `desc`.                                        |
| `Page`        | int    | Page (1-based).                                                          | If less than 1, server adjusts to 1.                                     |
| `PageSize`    | int    | Page size.                                                               | Clamped between **1** and **200** on the server.                         |

**Sorting defaults**: if you don't send `SortBy`/`SortDir`, the server uses `updatedAt` **desc** by default.

**Pagination**: server calculates `Skip = (Page - 1) * PageSize` and `Take = PageSize`.

## Response Format (Summary)

The response is a `PagedResponse<TaskDto>` with:

* `items`: list of tasks with `id`, `userId`, `title`, `description`, `categoryId`, `isCompleted`, `created`, `updatedAt`
* `page`, `pageSize`, `total`

> Property names may be in *camelCase* (default System.Text.Json configuration).

## Quick Examples

### 1) Search by text (in title **or** description)

**Direct URL**

```
http://localhost:8080/api/tasks?Search=boleto
```

**cURL**

```bash
curl -s "http://localhost:8080/api/tasks?Search=boleto"
```

### 2) Filter by user and pending only

```
http://localhost:8080/api/tasks?UserId=8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701&IsCompleted=false
```

### 3) Filter by category and sort by title (A→Z)

```
http://localhost:8080/api/tasks?CategoryId=3&SortBy=title&SortDir=asc
```

### 4) Pagination — page 2 with 50 items per page

```
http://localhost:8080/api/tasks?Page=2&PageSize=50
```

### 5) Combine filters with search

```
http://localhost:8080/api/tasks?UserId=8a1a5b1e-3c2d-4f8a-9b1c-12a34b56c701&IsCompleted=false&Search=internet&SortBy=updatedAt&SortDir=desc
```

## Tips

* `Search` does not need `%` — the server already uses `"%{Search}%"` internally.
* Search is case-insensitive (uses `ILIKE`).
* Invalid `SortBy`/`SortDir` values fall back to default `updatedAt desc`.
* If `Page < 1`, server adjusts to `1`; `PageSize` is clamped to `[1, 200]`.
