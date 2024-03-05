# curl -> Invoke-WebRequestのエイリアス

# 事前準備
# 比較的新しいWindows OSならば、特に不要

# サーバ情報
$SERVER_URL = "http://localhost"
$SERVER_PORT = 5000

# =========================================================
# プロファイル一覧を取得
# =========================================================

# プロファイル情報取得。(GETメソッド)
$res = Invoke-WebRequest -Method Get "${SERVER_URL}:${SERVER_PORT}/api/profile/list"
$res.Content | ConvertFrom-Json

# プロファイル情報を取得。(POSTメソッド)
$res = Invoke-WebRequest -Method Post "${SERVER_URL}:${SERVER_PORT}/api/profile/list"
$res.Content | ConvertFrom-Json

# プロファイル情報を取得。Refresh = true を渡すPOST
$res = Invoke-WebRequest -Method Post "${SERVER_URL}:${SERVER_PORT}/api/profile/list" `
  -Body "refresh=true"
$res.Content | ConvertFrom-Json



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




