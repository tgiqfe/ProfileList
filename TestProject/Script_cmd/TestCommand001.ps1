# curl -> Invoke-WebRequestのエイリアス

# 事前準備
# 比較的新しいWindows OSならば、特に不要

# サーバ情報
$SERVER_URL = "http://localhost"
$SERVER_PORT = 5000

# =========================================================
# プロファイル一覧を取得
# =========================================================

# 通常どおりのGETリクエスト
$ret = Invoke-WebRequest -Method Get "${SERVER_URL}:${SERVER_PORT}/api/profile/list"
$ret.Content




curl -X GET %SERVER_URL%:%SERVER_PORT%/api/profile/list | jq

# 何も指定しないPOSTリクスト
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list | jq

# 値を渡すPOSTリクエスト (Refresh = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list ^
  -d "refresh=true" | jq

# 値を渡すPOSTリクエスト (Refresh = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list ^
  -d "refresh=false" | jq

# JSONで値を渡すPOSTリクエスト (Refresh = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": true }" | jq

# JSONで値を渡すPOSTリクエスト (Refresh = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": false }" | jq




