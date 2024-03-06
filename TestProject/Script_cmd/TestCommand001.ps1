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
$res = Invoke-WebRequest -Method Get -Uri "${SERVER_URL}:${SERVER_PORT}/api/profile/list"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# プロファイル情報を取得。(POSTメソッド)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/profile/list"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# プロファイル情報を取得。Refresh = true を渡すPOST
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/profile/list" `
  -Body "refresh=true"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# プロファイル情報を取得。Refresh = false を渡すPOST
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/profile/list" `
  -Body "refresh=false"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# プロファイル情報を取得。Refresh = true を渡すPOST (JSON)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/profile/list" `
  -ContentType "application/json" `
  -Body "{ `"refresh`": true }"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# =========================================================
# プロファイルの削除
# =========================================================

# プロファイルの削除 (指定ユーザーのプロファイルを削除)
# ※Protectedプロファイルは除外
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/profile/delete" `
  -Body "username=Test001"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# プロファイルの削除 (全ユーザーのプロファイルを削除)
# ※Protectedプロファイルは除外
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/profile/delete" `
  -Body "all=true"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# プロファイルの削除 (DELETEメソッド, 全ユーザーのプロファイルを削除, JSON値渡し)
# ※Protectedプロファイルは除外
$res = Invoke-WebRequest -Method Delete -Uri "${SERVER_URL}:${SERVER_PORT}/api/profile/delete" `
  -ContentType "application/json" `
  -Body "{ `"all`": true }"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# =========================================================
# ログイン中セッションの一覧を取得
# =========================================================

# ログイン中セッションの一覧を取得(GETメソッド)
$res = Invoke-WebRequest -Method Get -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/session"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログイン中セッションの一覧を取得(POSTメソッド)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/session"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログイン中セッションの一覧を取得 (Refresh = false)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/session" `
  -Body "refresh=false"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログイン中セッションの一覧を取得 (Refresh = true)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/session" `
  -Body "refresh=true"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログイン中セッションの一覧を取得 Jsonで値渡し (Refresh = false)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/session" `
  -ContentType "application/json" `
  -Body "{ `"refresh`": false }"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログイン中セッションの一覧を取得 Jsonで値渡し (Refresh = true)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/session" `
  -ContentType "application/json" `
  -Body "{ `"refresh`": true }"
$res.Content | ConvertFrom-Json | ConvertTo-Json


# =========================================================
# ユーザーのログオン
# =========================================================

# 指定したユーザーでログオン
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/logon" `
  -Body "username=Administrator&password=Password"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# 指定したユーザーでログオン (ドメイン指定有り)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/logon" `
  -Body "username=Administrator&password=Password&domainname=EXAMPLE-DOMAIN"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# 指定したユーザーでログオン (JSONで値渡し)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/logon" `
  -ContentType "application/json" `
  -Body "{ `"username`": `"Administrator`", `"password`": `"Password`", `"domainname`": `"EXAMPLE-DOMAIN`" }"
$res.Content | ConvertFrom-Json | ConvertTo-Json


# =========================================================
# ログイン中のセッションをログオフ
# =========================================================

# 全セッションをログオフ (GETメソッド)
$res = Invoke-WebRequest -Method Get -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/logoff"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# 全セッションをログオフ (POSTメソッド)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/logoff"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# 指定したユーザーのセッションをログオフ (POSTメソッド, Administratorのみログオフ)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/logoff" `
  -Body "username=Administrator"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# 指定したユーザーのセッションをログオフ (POSTメソッド, Administratorのみログオフ, ドメイン指定有り)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/logoff" `
  -Body "username=Administrator&domainname=EXAMPLE-DOMAIN"
$res.Content | ConvertFrom-Json | ConvertTo-Json


# =========================================================
# RDPセッションを切断
# =========================================================

# 全RDPセッションを切断 (GETメソッド)
$res = Invoke-WebRequest -Method Get -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/disconnect"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# 全RDPセッションを切断 (POSTメソッド)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/disconnect"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# 指定したRDPセッションを切断 (POSTメソッド, Administratorのみ切断)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/disconnect" `
  -Body "username=Administrator"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# 指定したRDPセッションを切断 (POSTメソッド, Administratorのみ切断, ドメイン指定有り)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/user/disconnect" `
  -Body "username=Administrator&domainname=EXAMPLE-DOMAIN"
$res.Content | ConvertFrom-Json | ConvertTo-Json


# =========================================================
# ログを取得
# =========================================================

# ログを取得 (GET, 最後10行)
$res = Invoke-WebRequest -Method Get -Uri "${SERVER_URL}:${SERVER_PORT}/api/log/print"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログを取得 (POST, 最後10行)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/log/print"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログを取得 (POST, 最後1行)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/log/print" `
  -Body "line=1"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログを取得 (POST, 最後20行)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/log/print" `
  -Body "line=20"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログを取得 (POST, 全ログ出力)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/log/print" `
  -Body "all=true"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログを取得 (POST, 最後20行, JSONで値渡し)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/log/print" `
  -ContentType "application/json" `
  -Body "{ `"line`": 20 }"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログを取得 (POST, 最後のRequest 1回分のみ出力)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/log/print" `
  -Body "request=1"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# ログを取得 (POST, 最後のRequest 3回分のみ出力, JSONで値渡し)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/log/print" `
  -ContentType "application/json" `
  -Body "{ `"request`": 3 }"
$res.Content | ConvertFrom-Json | ConvertTo-Json
  

# =========================================================
# サーバ情報の取得
# =========================================================

# マシン情報の取得 (GETメソッド)
$res = Invoke-WebRequest -Method Get -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/info"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# マシン情報の取得 (POSTメソッド)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/info"
$res.Content | ConvertFrom-Json | ConvertTo-Json


# =========================================================
# ネットワーク情報の取得
# =========================================================

# ネットワークアドレス情報の取得 (GETメソッド, メイン使用のNICのみ)
$res = Invoke-WebRequest -Method Get -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/network"
$res.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10

# ネットワークアドレス情報の取得 (POSTメソッド, メイン使用のNICのみ)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/network"
$res.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10

# ネットワークアドレス情報の取得 (POSTメソッド, Refresh = true)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/network" `
  -Body "refresh=true"
$res.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10

# ネットワークアドレス情報の取得 (POSTメソッド, Refresh = false)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/network" `
  -Body "refresh=false"
$res.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10

# ネットワークアドレス情報の取得 (POSTメソッド, Refresh = true, All = true)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/network" `
  -Body "refresh=true&all=true"
$res.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10

# ネットワークアドレス情報の取得 (POSTメソッド, Refresh = false, All = true, JSONで値渡し)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/network" `
  -ContentType "application/json" `
  -Body "{ `"refresh`": false, `"all`": true }"
$res.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10


# =========================================================
# 終了
# =========================================================

# サーバ終了 (GETメソッド)
$res = Invoke-WebRequest -Method Get -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/close"
$res.Content | ConvertFrom-Json | ConvertTo-Json

# サーバ終了 (POSTメソッド)
$res = Invoke-WebRequest -Method Post -Uri "${SERVER_URL}:${SERVER_PORT}/api/server/close"
$res.Content | ConvertFrom-Json | ConvertTo-Json

