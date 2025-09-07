# SimpleGreetingBot (.NET 8 + Bot Framework v4)

シンプルな挨拶ボットです。ユーザーのメッセージに対して「おはよう」「こんにちは」「こんばんは」のいずれかをランダムに返します。

このプロジェクトは「Web からのチャット（Web Chat）」と「Microsoft Teams アプリ」両方の学習を目的とした最小構成です。

## 特徴
- .NET 8 / ASP.NET Core Minimal Hosting
- Bot Framework v4 CloudAdapter 採用
- エンドポイント: `/api/messages`
- 受信/送信メッセージの詳細ログ（タグ: `[GreetingBot]`）
- HTTP ログミドルウェア、簡易ヘルスチェック(`/health`)

## ローカルで動かす
前提: .NET 8 SDK

```pwsh
# ビルド & 実行
 dotnet build
 dotnet run
```
- 表示された URL の `/api/messages` を Bot Framework Emulator で開く
- AppId/Password は空でOK（開発時）

## Azure へデプロイ（概要）
1) App Service（.NET 8、Linux 推奨、B1 以上）を作成（Always On 推奨）
2) Entra ID でアプリ登録 → AppId/Client Secret を取得
3) Azure Bot（Bot Channels Registration）を作成（App type: SingleTenant）
4) Azure Bot → Messaging endpoint を `https://<WebApp>.azurewebsites.net/api/messages` に設定
5) App Service の 環境変数 > アプリ設定 へ以下を追加
   - `MicrosoftAppType = SingleTenant`
   - `MicrosoftAppId = <APP_ID>`
   - `MicrosoftAppPassword = <CLIENT_SECRET>`
   - `MicrosoftAppTenantId = <TENANT_ID>`
6) 発行してデプロイ（例）
```pwsh
 dotnet publish -c Release -o .\publish
# VS Code の Azure App Service 拡張で .\publish を Deploy to Web App
```
7) Azure Bot の「Test in Web Chat」で確認

## Teams で使う（メモ）
基本的には Bot Service の構成は共通です。Teams で使うには:
- Azure Bot → チャネル → Microsoft Teams を有効化
- Teams 側のテナントでボットの利用が許可されていることを確認
- 必要に応じて Teams アプリ パッケージ（manifest v1.11+）を作成し、Teams 管理センター経由でアップロード
（学習目的での1:1チャットであれば、Teams チャネルを有効化するだけで動作確認できます）

## ログとヘルスチェック
- 受信/送信ログ: `[GreetingBot]` タグ付きで出力
- App Service → ログ ストリームで確認
- ヘルスチェック: `GET /health` → `{ "status": "ok" }`

## トラブルシュート
- 401/403: `MicrosoftAppId/Password/TenantId` を再確認。保存後は Web App を再起動
- 404/405: Azure Bot の Messaging endpoint が `/api/messages` か確認
- 500: 例外はログストリームで確認／再発行
- 応答がない: Always On を有効化、`Incoming/Outgoing` のログ有無を確認

## 主なファイル
- `Program.cs` … DI、HTTP ログ、ルーティング、`/health`
- `Controllers/BotController.cs` … `/api/messages`
- `Bots/GreetingBot.cs` … ランダム挨拶と動作ログ
- `AdapterWithErrorHandler.cs` … CloudAdapter + エラーハンドリング + ミドルウェア
- `Middleware/ActivityLoggingMiddleware.cs` … 受信/送信 Activity の詳細ログ

## セキュリティ
- AppId/Secret はリポジトリに含めず、App Service のアプリ設定で管理
- シークレットは定期的にローテーションを推奨
