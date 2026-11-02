#!/usr/bin/env bash
# Creates a link, updates it and deletes it again with Deep Blue Links' API.
# Needs DBL_API_KEY and DBL_APP_ID; DBL_API_URL defaults to https://thedbl.ink.
set -euo pipefail

if [[ -z ${DBL_API_KEY:-} || -z ${DBL_APP_ID:-} ]]; then
  echo "Set DBL_API_KEY to your app's API key (the sandbox key outside production) and DBL_APP_ID to its id." >&2
  exit 1
fi
API_URL="${DBL_API_URL:-https://thedbl.ink}"

# Sends a request and prints the body; exits with the status and the API's message on an error.
call() {
  local method=$1 path=$2 body=${3:-}
  local args=(-sS -X "$method" "$API_URL$path" -H "Api-Key: $DBL_API_KEY" -w '\n%{http_code}')
  if [[ -n $body ]]; then args+=(-H 'Content-Type: application/json' -d "$body"); fi
  local output status
  output=$(curl "${args[@]}")
  status=${output##*$'\n'}
  output=${output%$'\n'*}
  if (( status >= 400 )); then
    echo "Deep Blue Links answered $status: ${output:-(empty body)}" >&2
    exit 1
  fi
  echo "$output"
}

created=$(call POST /api/links "{
  \"appId\": $DBL_APP_ID,
  \"fullUrl\": \"/products/1234\",
  \"title\": \"Spring sale\",
  \"description\": \"Up to 50% off this week only.\",
  \"imageUrl\": \"https://example.com/images/spring-sale.png\",
  \"fallbackUrl\": \"https://example.com/spring-sale\"
}")
echo "Created: $created"

id=$(echo "$created" | sed -E 's/.*"id":([0-9]+).*/\1/')

updated=$(call PUT "/api/links/$id" "{ \"appId\": $DBL_APP_ID, \"fullUrl\": \"/products/1234\", \"title\": \"Spring sale, extended\", \"description\": \"\", \"imageUrl\": \"\", \"fallbackUrl\": \"\" }")
echo "Updated: $updated"

call DELETE "/api/links/$id" > /dev/null
echo "Deleted link $id"
