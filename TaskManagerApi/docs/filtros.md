# TaskManager API — Filtros de Listagem (`GET /api/tasks`)

Este documento explica como usar os **filtros**, **ordenação** e **paginação** do endpoint `GET /api/tasks` e traz exemplos práticos (cURL e URL direta).

> A listagem aceita um objeto de query (`TaskQuery`) com os parâmetros abaixo e retorna um envelope paginado com itens e metadados (página, tamanho e total).

## Parâmetros de query

| Parâmetro     | Tipo   | O que faz                                                                | Observações                                                              |
| ------------- | ------ | ------------------------------------------------------------------------ | ------------------------------------------------------------------------ |
| `UserId`      | GUID   | Filtra tarefas de um usuário específico (igualdade exata).               |                                                                          |
| `CategoryId`  | int    | Filtra por categoria específica (igualdade exata).                       |                                                                          |
| `IsCompleted` | bool   | Filtra por concluídas (`true`) ou pendentes (`false`).                   |                                                                          |
| `Search`      | string | Busca *case-insensitive* por **contém** em `Title` **ou** `Description`. | Usa `ILIKE` com padrão `"%{Search}%"`. Se vier vazio/branco, não aplica. |
| `SortBy`      | string | Campo de ordenação.                                                      | Aceita: `title`, `created`, `updatedAt`. Valor inválido cai no padrão.   |
| `SortDir`     | string | Direção de ordenação.                                                    | `asc` ou `desc`. Padrão: `desc`.                                         |
| `Page`        | int    | Página (1-based).                                                        | Se menor que 1, o servidor ajusta para 1.                                |
| `PageSize`    | int    | Tamanho da página.                                                       | Limitado entre **1** e **200** no servidor.                              |

**Padrões de ordenação**: se você não enviar `SortBy`/`SortDir`, o servidor usa `updatedAt` **desc** por padrão.

**Paginação**: o servidor calcula `Skip = (Page - 1) * PageSize` e `Take = PageSize`.

## Formato de resposta (resumo)

A resposta é um `PagedResponse<TaskDto>` com:

* `items`: lista de tarefas com `id`, `userId`, `title`, `description`, `categoryId`, `isCompleted`, `created`, `updatedAt`
* `page`, `pageSize`, `total`

> Os nomes de propriedade podem sair em *camelCase* (configuração padrão do System.Text.Json).

## Exemplos rápidos

### 1) Buscar por texto (em título **ou** descrição)

**URL direta**

```
/api/tasks?Search=boleto
```

**cURL**

```bash
curl -s "{{baseUrl}}/api/tasks?Search=boleto"
```

### 2) Filtrar por usuário e somente pendentes

```
/api/tasks?UserId=11111111-1111-1111-1111-111111111111&IsCompleted=false
```

### 3) Filtrar por categoria e ordenar por título (A→Z)

```
/api/tasks?CategoryId=3&SortBy=title&SortDir=asc
```

### 4) Paginação — página 2 com 50 itens por página

```
/api/tasks?Page=2&PageSize=50
```

### 5) Combinar filtros com busca

```
/api/tasks?UserId=11111111-1111-1111-1111-111111111111&IsCompleted=false&Search=internet&SortBy=updatedAt&SortDir=desc
```

## Dicas

* `Search` não precisa de `%` — o servidor já usa `"%{Search}%"` internamente.
* A busca ignora maiúsculas/minúsculas por usar `ILIKE`.
* Valores inválidos de `SortBy`/`SortDir` caem no padrão `updatedAt desc`.
* Se `Page < 1`, o servidor ajusta para `1`; `PageSize` é ajustado para o intervalo `[1, 200]`.

