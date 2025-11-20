# Notes Backend (Minimal API)

Simple REST API for personal notes. Runs on http://localhost:3001 and exposes OpenAPI docs at http://localhost:3001/docs.

Endpoints:
- POST /notes
- GET /notes
- GET /notes/{id}
- PUT /notes/{id}
- DELETE /notes/{id}

Request/Response JSON shape:
{
  "id": "guid",
  "title": "string",
  "content": "string",
  "createdAt": "ISO-8601",
  "updatedAt": "ISO-8601"
}

Examples (curl):
- Create:
  curl -s -X POST http://localhost:3001/notes -H "Content-Type: application/json" -d '{"title":"Hello","content":"World"}'
- List:
  curl -s http://localhost:3001/notes
- Get:
  curl -s http://localhost:3001/notes/{id}
- Update:
  curl -s -X PUT http://localhost:3001/notes/{id} -H "Content-Type: application/json" -d '{"title":"New","content":"Content"}'
- Delete:
  curl -s -o /dev/null -w "%{http_code}\n" -X DELETE http://localhost:3001/notes/{id}

Notes:
- In-memory storage (data resets on restart).
- createdAt and updatedAt are UTC.
