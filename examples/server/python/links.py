"""Creates a link, updates it and deletes it again with Deep Blue Links' API.

Needs DBL_API_KEY and DBL_APP_ID; DBL_API_URL defaults to https://thedbl.ink.
"""

import os
import sys

import requests

API_URL = os.environ.get("DBL_API_URL", "https://thedbl.ink")
API_KEY = os.environ.get("DBL_API_KEY")
APP_ID = os.environ.get("DBL_APP_ID")
if not API_KEY or not APP_ID:
    sys.exit("Set DBL_API_KEY to your app's API key (the sandbox key outside production) and DBL_APP_ID to its id.")


def call(method, path, body=None):
    response = requests.request(method, f"{API_URL}{path}", headers={"Api-Key": API_KEY}, json=body, timeout=10)
    if not response.ok:
        raise RuntimeError(f"Deep Blue Links answered {response.status_code}: {response.text or '(empty body)'}")
    return response.json() if response.content else None


link = call("POST", "/api/links", {
    "appId": int(APP_ID),
    "fullUrl": "/products/1234",
    "title": "Spring sale",
    "description": "Up to 50% off this week only.",
    "imageUrl": "https://example.com/images/spring-sale.png",
    "fallbackUrl": "https://example.com/spring-sale",
})
print(f"Created link {link['id']}: {link['shortUrl']}")

updated = call("PUT", f"/api/links/{link['id']}", {
    "appId": int(APP_ID),
    "fullUrl": "/products/1234",
    "title": "Spring sale, extended",
    "description": "",
    "imageUrl": "",
    "fallbackUrl": "",
})
print(f"Updated its title to \"{updated['title']}\"")

call("DELETE", f"/api/links/{link['id']}")
print(f"Deleted link {link['id']}")
