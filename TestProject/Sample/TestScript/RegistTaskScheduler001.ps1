

# 実行ファイルへのパス
$executePath = "C:\App\publish\ProfileList.exe"

# 実行ユーザー
$account = "SYSTEM"

# タスクの名前
$taskName = "ProfileList"

# 登録開始
$trigger = New-ScheduledTaskTrigger `
  -AtStartup
$action = New-ScheduledTaskAction `
  -Execute $executePath `
  -WorkingDirectory "C:\App\publish"
$principal = New-ScheduledTaskPrincipal `
  -UserId $account `
  -LogonType Password
$settings = New-ScheduledTaskSettingsSet `
  -AllowStartIfOnBatteries
$task = New-ScheduledTask `
  -Trigger $trigger `
  -Action $action `
  -Settings $settings `
  -Principal $principal
Register-ScheduledTask `
  -TaskPath "\" `
  -TaskName $taskName `
  -InputObject $task `
  -User $account

# 登録を解除
Unregister-ScheduledTask -TaskName $taskName -Confirm:$false



