---
post_title: "BartectorQuery"
author1: "Maintainer"
post_slug: "bartectorquery"
microsoft_alias: ""
featured_image: ""
categories: ["tools"]
tags: ["Bartector","WinForms","NPOI","SharpZipLib"]
ai_note: "no"
summary: "以 .NET Framework/WinForms 建置，用於查詢與處理 BarTender/Bartector 資料的桌面應用程式說明。"
post_date: "2025-09-09"
---

# BartectorQuery

## 簡短說明

本專案為一個 Windows 桌面應用程式（.NET Framework / WinForms），用來查詢並處理 BarTender / Bartector 相關的資料與匯出需求。專案名稱：`BartectorQuery`。

## 主要功能

- 從資料來源查詢資料並匯出為 Excel（使用 NPOI 支援）
- 使用 SharpZipLib / BouncyCastle 處理壓縮與加密需求

## 主要檔案與結構

- `BartectorQuery.sln` — Visual Studio 解決方案
- `BartectorQuery/` — 專案資料夾
  - `Program.cs` — 程式進入點
  - `Form1.cs` / `Form1.Designer.cs` — 主視窗與 UI
  - `Query.cs` / `Db.cs` — 查詢與資料存取邏輯
  - `packages/` — 已包含的套件原始檔（NPOI、SharpZipLib、Portable.BouncyCastle 等）

## 相依套件（已包含於 `packages/`）

- NPOI (v2.5.6) — Excel/Office 檔案處理
- SharpZipLib — 壓縮處理
- Portable.BouncyCastle — 加密/安全相關

## 開發環境

- 作業系統：Windows
- 開發工具：Visual Studio （支援 .NET Framework 4.x 的任何版本）
- 目標框架：.NET Framework（以專案設定為準）

## 如何 建置（快速）

使用 Visual Studio 開啟 `BartectorQuery.sln`，以 Debug/Release 模式 建置（預設）。

可使用 PowerShell + MSBuild 自動化建置：

```powershell
# 切換到專案目錄
Set-Location -Path 'd:\專案\QA\Bartector查詢'
# 建置解決方案（需先安裝 MSBuild 或 Visual Studio）
msbuild .\BartectorQuery.sln /t:Build /p:Configuration=Debug
```

## 執行

- 建置後，可在 `BartectorQuery\bin\Debug\BartectorQuery.exe`（或 Release 對應資料夾）執行。
- 或在 Visual Studio 中按 F5（偵錯）或 Ctrl+F5（非偵錯）執行。

## 輸出與測試資料

- 若程式執行有匯出流程，輸出檔案常見於 `BartectorQuery/bin/Debug/Export/`（視執行流程而定）。

## 常見問題與疑難排解

- NuGet 或相依函式庫錯誤：專案已包含 `packages/`，若仍缺少 assembly，請在 Visual Studio 中還原套件或確認參考路徑。
- 找不到 DLL：確認 `bin/Debug` 下所需的 DLL（NPOI、SharpZipLib、BouncyCastle）皆存在，或清除並重建專案。
- 權限問題：在受限環境執行時，請確認有讀寫資料夾的權限。

## 安全性注意事項

- 若處理機敏資料，請依公司政策使用適當加密與存取機制。Portable.BouncyCastle 提供基礎加密功能，請選擇合適演算法並妥善管理金鑰。

## 建議下一步

1. 在 Visual Studio 中 開啟並 建置，確認主視窗能正常啟動。
2. 測試匯出流程並檢查 `Export/` 路徑輸出格式。
3. 若要加入自動化測試或 CI，考慮建立單元測試專案並加入建置管線。

## 授權與來源

此 README 為專案說明範本；第三方套件授權請參考 `packages/` 中的 LICENSE 檔案。

## 聯絡

若需要協助新增使用說明、範例資料，或建立自動化建置/CI 流程，請回覆具體需求。

## 系統操作詳情（操作流程與執行細節）

以下章節提供更實務、逐步的操作細節，讓開發者與使用者能快速上手、設定與除錯。


### 前置條件

- 作業系統：Windows（建議 Windows 10/11）

- .NET Framework：請以專案設定為準（通常是 .NET Framework 4.x），開發與建置需安裝相容的 Visual Studio 與 MSBuild。

- 相依套件：專案已在 `packages/` 內包含常見套件（NPOI、SharpZipLib、Portable.BouncyCastle 等）。若出現找不到 assembly，請在 Visual Studio 中還原或手動確認參考路徑。


### 設定檔與常見變更點

- `App.config`：若程式需連線資料庫或使用外部服務，連線字串與設定通常放在此檔案。請確認 `connectionStrings`、Timeout、或自訂設定是否正確。

- `Program.cs`：程式進入點，會初始化 Logger、讀取設定並啟動主視窗（`Form1`）。想要自動執行匯出或測試流程，可在此加上短期的 Debug 呼叫（記得不要提交到主分支）。

- `Query.cs` / `Db.cs`：負責查詢與資料存取（SQL）。如需修改查詢邏輯或對資料庫優化，請在此檔案進行。

- `Logger.cs`：負責紀錄執行紀錄與錯誤，預設會輸出到 Console 或檔案（視實作）。檢查 Logger 的路徑與權限，避免無法寫入造成靜默失敗。


### 執行與測試（開發者）

1. 在 Visual Studio 開啟 `BartectorQuery.sln`。

1. 選擇組態（Debug / Release）。

1. 若要使用命令列建置（PowerShell 範例）：

```powershell
Set-Location -Path 'd:\專案\QA\Bartector查詢';
msbuild .\BartectorQuery.sln /t:Build /p:Configuration=Debug
```

1. 建置成功後，主程式位於 `BartectorQuery\bin\Debug\BartectorQuery.exe`（或 Release 對應資料夾），直接雙擊或從 PowerShell 執行即可。


### 執行者流程（使用者操作主視窗）

1. 啟動應用程式後，主視窗 `Form1` 提供查詢輸入欄位與按鈕（視 UI 設計而定）。

1. 輸入查詢條件後按下「查詢」或相應按鈕，程式會透過 `Query.cs` 與 `Db.cs` 送出 SQL 查詢並回傳結果。

1. 若使用匯出功能，系統會將結果轉為 Excel（使用 NPOI）並儲存在 `Export/` 或 `BartectorQuery/bin/Debug/Export/`（視執行目錄而定）。

1. 若匯出遇到錯誤（如權限或檔案鎖定），請檢查 Logger（`Logger.cs`）輸出與目錄權限。


### 匯出與檔案格式

- 使用 NPOI 產生 Office 檔案；匯出路徑預設在執行目錄下的 `Export/` 子目錄。

- 若要更改輸出路徑或檔名格式，可在產生匯出時的程式碼區段（搜尋 `Export` 或 `NPOI` 關鍵字）修改路徑邏輯。


### 日誌與偵錯

- 日誌位置：檢查 `Logger.cs` 的實作，預設可能輸出到 Console、Text 檔或 EventLog。若遇到異常請先查看日誌檔案。

- 常見錯誤：

  - 找不到 assembly → 檢查 `packages/` 與專案參考

  - 權限錯誤（無法寫入 Export 或 Log）→ 檢查執行者帳戶、資料夾權限

  - 資料庫連線失敗 → 檢查 `App.config` 中的 connection string、網路與資料庫服務狀態


### 測試與驗證（快速 Smoke Test）

1. 建置並啟動應用程式。

1. 在 UI 中執行一個簡單查詢（例如查詢最近 10 筆資料或測試資料），確認畫面回應與結果顯示。

1. 點選匯出（若有），確認 `Export/` 下實際產生的檔案，開啟檔案檢查欄位與格式。


### 維運建議

- 建議在正式環境使用專屬資料夾並設定正確 ACL（存取控制清單）以避免權限問題。

- 長期執行或自動化匯出：請將匯出流程移至背景服務或排程工具（例如 Windows Task Scheduler），並加入例外處理與重試機制。


### 常見修改場景（快查）

- 要改 SQL 查詢 → 編輯 `Query.cs` 或相關 SQL 檔案

- 要改匯出格式 → 搜尋 NPOI 使用處並修改對應欄位/樣式程式碼

- 要改日誌路徑 → 編輯 `Logger.cs` 或相應設定


### 範例：以 PowerShell 自動化匯出（概念示例）

此範例示範如何用 PowerShell 啟動應用程式（前提是主程式支援命令列或能在啟動後自動執行匯出）。若程式不支援命令列自動化，需在程式內新增對應功能。

```powershell
# 範例：以程序啟動方式執行應用程式
& 'd:\專案\QA\Bartector查詢\BartectorQuery\bin\Debug\BartectorQuery.exe'
```


### 若需擴充：自動化與 CI 建議

- 建議為關鍵流程（匯出、查詢）撰寫單元測試與整合測試，以便在 CI 流水線中驗證行為。

- 若要導入自動化建置/部署（例如 GitHub Actions），在 CI 工作中加上 MSBuild 建置步驟、測試步驟，並將產出（匯出範例）上傳為 Artifact。


## 結語

本章節補充了實務上常見的操作與除錯重點。若你想把某一部分（例如自動化匯出／命令列介面／更完整的日誌機制）納入程式，告訴我你想要的行為，我可以幫你規劃與實作對應的變更或 PR。

