import requests
import json

r = requests.post("http://localhost:8208/api/auth/login", json={"email": "admin@catalogo.local", "senha": "AdminPassword123!"})
token = r.json().get("token")

logs_res = requests.get("http://localhost:8208/api/logs?limite=10", headers={"Authorization": f"Bearer {token}"})
print(f"Status HTTP: {logs_res.status_code}")
print(json.dumps(logs_res.json(), indent=2, ensure_ascii=False))
