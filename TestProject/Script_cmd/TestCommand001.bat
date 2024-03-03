rem # Windows用curl (cmd版のほう)

rem # 事前準備
rem # - curlコマンドが使用できる状態
rem #   ※最近のWindows10や、Windows11ならば基本的に何もしなくてOK
rem # - jqコマンドの準備
rem #   ※↓のページからjqコマンドをダウンロードし、PATHを通しておく。
rem #     https://jqlang.github.io/jq/

rem # サーバ情報
set SERVER_URL=http://localhost
set SERVER_PORT=5000

rem # =========================================================
rem # プロファイル一覧を取得
rem # =========================================================
rem # 通常どおりのGETリクエスト
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/user/profile | jq

rem # 何も指定しないPOSTリクスト
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/profile | jq

rem # 値を渡すPOSTリクエスト (Refresh = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/profile ^
  -d "refresh=true" | jq

rem # 値を渡すPOSTリクエスト (Refresh = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/profile ^
  -d "refresh=false" | jq

rem # JSONで値を渡すPOSTリクエスト (Refresh = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/profile ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": true }" | jq

rem # JSONで値を渡すPOSTリクエスト (Refresh = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/profile ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": false }" | jq


rem # =========================================================
rem # ログイン中セッションの一覧を取得
rem # =========================================================
rem # ログイン中セッションの一覧を取得
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/user/session | jq

rem # ログイン中セッションの一覧を取得
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/session | jq

rem # ログイン中セッションの一覧を取得 (Refresh = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/session ^
  -d "refresh=false" | jq

rem # ログイン中セッションの一覧を取得 (Refresh = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/session ^
  -d "refresh=true" | jq

rem # ログイン中セッションの一覧を取得 Jsonで値渡し (Refresh = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/session ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": false }" | jq

rem # ログイン中セッションの一覧を取得 Jsonで値渡し (Refresh = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/session ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": true }" | jq


rem # =========================================================
rem # ログを取得
rem # =========================================================
rem # ログを取得 (GET, 最後10行)
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/log/print | jq

rem # ログを取得 (POST, 最後10行)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/log/print | jq

rem # ログを取得 (POST, 最後1行)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/log/print ^
    -d "line=1" | jq

rem # ログを取得 (POST, 最後20行)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/log/print ^
    -d "line=20" | jq

rem # ログを取得 (POST, 全ログ出力)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/log/print ^
    -d "allprint=true" | jq


rem # =========================================================
rem # サーバ情報の取得
rem # =========================================================
rem # サーバ情報の取得 (GETメソッド)
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/server/info | jq

rem # サーバ情報の取得 (POSTメソッド)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/info | jq


rem # =========================================================
rem # 終了
rem # =========================================================
rem # サーバ終了 (GETメソッド)
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/server/close | jq

rem # サーバ終了 (POSTメソッド)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/close | jq


