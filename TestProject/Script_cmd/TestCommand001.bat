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
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/profile/list | jq

rem # 何も指定しないPOSTリクスト
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list | jq

rem # 値を渡すPOSTリクエスト (Refresh = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list ^
  -d "refresh=true" | jq

rem # 値を渡すPOSTリクエスト (Refresh = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list ^
  -d "refresh=false" | jq

rem # JSONで値を渡すPOSTリクエスト (Refresh = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": true }" | jq

rem # JSONで値を渡すPOSTリクエスト (Refresh = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/list ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": false }" | jq


rem # =========================================================
rem # プロファイルの削除
rem # =========================================================

rem # プロファイルの削除 (指定ユーザーのプロファイルを削除)
rem # ※Protectedプロファイルは除外
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/delete ^
  -d "username=Test001" | jq

rem # プロファイルの削除 (全ユーザーのプロファイルを削除)
rem # ※Protectedプロファイルは除外
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/profile/delete ^
  -d "all=true" | jq

rem # プロファイルの削除 (DELETEメソッド, 全ユーザーのプロファイルを削除)
rem # ※Protectedプロファイルは除外
curl -X DELETE %SERVER_URL%:%SERVER_PORT%/api/profile/delete ^
  -d "all=true" | jq


rem # =========================================================
rem # ログイン中セッションの一覧を取得
rem # =========================================================

rem # ログイン中セッションの一覧を取得(GETメソッド)
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/user/session | jq

rem # ログイン中セッションの一覧を取得(POSTメソッド)
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
rem # ユーザーのログオン
rem # =========================================================

rem # 指定したユーザーでログオン
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/logon ^
  -d "username=Administrator&password=Password" | jq

rem # 指定したユーザーでログオン (ドメイン指定有り)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/logon ^
  -d "username=Administrator&password=Password&domainname=EXAMPLE-DOMAIN" | jq

rem # 指定したユーザーでログオン (JSONで値渡し)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/logon ^
  -H "Content-Type: application/json" ^
  -d "{ \"username\": \"Administrator\", \"password\": \"Password\", \"domainname\": \"EXAMPLE-DOMAIN\" }" | jq


rem # =========================================================
rem # ログイン中のセッションをログオフ
rem # =========================================================

rem # 全Rセッションをログオフ (GETメソッド)
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/user/logoff | jq

rem # 全セッションをログオフ (POSTメソッド)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/logoff | jq

rem # 指定したユーザーのセッションをログオフ (POSTメソッド, Administratorのみログオフ)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/logoff ^
  -d "username=Administrator" | jq

rem # 指定したユーザーのセッションをログオフ (POSTメソッド, Administratorのみログオフ, ドメイン指定有り)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/logoff ^
  -d "username=Administrator&domainname=s" | jq

rem # =========================================================
rem # RDPセッションを切断
rem # =========================================================

rem # 全RDPセッションを切断 (GETメソッド)
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/user/disconnect | jq

rem # 全RDPセッションを切断 (POSTメソッド)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/disconnect | jq

rem # 指定したRDPセッションを切断 (POSTメソッド, Administratorのみ切断)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/disconnect ^
  -d "username=Administrator" | jq

rem # 指定したRDPセッションを切断 (POSTメソッド, Administratorのみ切断, ドメイン指定有り)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/user/disconnect ^
  -d "username=Administrator&domainname=EXAMPLE-DOMAIN" | jq


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
  -d "all=true" | jq

rem # ログを取得 (POST, JSONで値渡し, 最後20行)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/log/print ^
  -H "Content-Type: application/json" ^
  -d "{ \"line\": 5 }" | jq

rem # ログを取得 (POST, JSONで値渡し, 最後5行, 全ログ出力)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/log/print ^
  -H "Content-Type: application/json" ^
  -d "{ \"line\": 5, \"all\": true }" | jq

rem # ログを取得 (POST, 最後のReqesut 1回分のみ出力)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/log/print ^
  -d "request=3" | jq

rem # ログを取得 (POST, JSONで値渡し, 最後のReqesut 3回分のみ出力)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/log/print ^
  -H "Content-Type: application/json" ^
  -d "{ \"request\": 3 }" | jq


rem # =========================================================
rem # サーバ情報の取得
rem # =========================================================
rem # マシン情報の取得 (GETメソッド)
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/server/info | jq

rem # マシン情報の取得 (POSTメソッド)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/info | jq


rem # =========================================================
rem # ネットワーク情報の取得
rem # =========================================================
rem # ネットワークアドレス情報の取得 (GETメソッド)
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/server/network | jq

rem # ネットワークアドレス情報の取得 (POSTメソッド)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/network | jq

rem # ネットワークアドレス情報の取得 (POSTメソッド, Refresh = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/network ^
  -d "refresh=true" | jq

rem # ネットワークアドレス情報の取得 (POSTメソッド, Refresh = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/network ^
  -d "refresh=false" | jq

rem # ネットワークアドレス情報の取得
rem # (POSTメソッド, Refresh = false, All = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/network ^
  -d "refresh=false&all=true" | jq

rem # ネットワークアドレス情報の取得
rem # (POSTメソッド, JSONで値渡し, Refresh = false, All = false)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/network ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": false, \"all\": false }" | jq

rem # ネットワークアドレス情報の取得
rem # (POSTメソッド, JSONで値渡し, Refresh = false, All = true)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/network ^
  -H "Content-Type: application/json" ^
  -d "{ \"refresh\": false, \"all\": true }" | jq

rem # =========================================================
rem # 終了
rem # =========================================================
rem # サーバ終了 (GETメソッド)
curl -X GET %SERVER_URL%:%SERVER_PORT%/api/server/close | jq

rem # サーバ終了 (POSTメソッド)
curl -X POST %SERVER_URL%:%SERVER_PORT%/api/server/close | jq


