# コール業務管理システム 開発進捗管理

## プロジェクト情報

- **プロジェクト名**: コール業務管理システム (Call Center Management System)
- **開発開始日**: 2025/01/14
- **技術スタック**: WPF, .NET 8.0, C#, SQLite/SQL Server/Access, Dapper
- **アーキテクチャ**: MVVM

---

## 開発フェーズ概要

| フェーズ | 状態 | 開始日 | 完了日 | 備考 |
|---------|------|--------|--------|------|
| Phase 0: プロジェクトセットアップ | 完了 | 2025/01/14 | 2025/01/14 | フォルダ構造、NuGetパッケージ、設定ファイル、Git初期化完了 |
| Phase 1: データベース層 | 完了 | 2025/01/14 | 2025/01/14 | DB抽象化層、SQLiteスクリプト、Repository、DI設定完了 |
| Phase 2: モデル層 | 完了 | 2025/01/14 | 2025/01/14 | エンティティ、DTO、ViewModelBase、RelayCommand完了 |
| Phase 3: 共通基盤・インフラ | 完了 | 2025/01/15 | 2025/01/15 | 認証、セッション管理、設定管理、ダイアログ、ナビゲーション、バリデーション完了 |
| Phase 4: ログイン機能 | 完了 | 2025/01/15 | 2025/01/15 | LoginViewModel, LoginView, 認証処理, テストデータ投入完了 |
| Phase 5: メイン画面基盤 | 完了 | 2025/01/15 | 2025/01/15 | MainViewModel, MainView, ヘッダー, 左右パネルレイアウト完了 |
| Phase 6: 検索機能 | 完了 | 2025/01/15 | 2025/01/15 | 顧客検索、問合せ履歴検索のUI・ViewModel完了 |
| Phase 7: 顧客情報表示 | 完了 | 2025/01/15 | 2025/01/15 | 顧客情報表示、過去履歴表示のUI・ViewModel完了 |
| Phase 8: 問合せ登録・更新 | 完了 | 2025/01/15 | 2025/01/15 | InquiryViewModel、新規登録・更新・履歴ログ保存完了 |
| Phase 9: カスタム項目 | 完了 | 2025/01/15 | 2025/01/15 | カスタム項目動的UI生成、バリデーション、保存処理完了 |
| Phase 10: エクスポート・レポート | 完了 | 2025/01/16 | 2025/01/16 | CSV出力機能、レポート表示・印刷機能完了 |
| Phase 11: テスト・品質保証 | 完了 | 2025/01/16 | 2025/01/16 | xUnitテスト、Repository/Service/ViewModelテスト、品質チェック完了 |
| Phase 12: デプロイ準備 | 未着手 | - | - | - |

**状態**: 未着手 / 進行中 / 完了 / 保留

---

## Phase 0: プロジェクトセットアップ

**目的**: 開発環境の構築とプロジェクト構成の準備

### タスク一覧

- [x] **0.1** Visual Studio 2022でWPFプロジェクト作成
  - [x] プロジェクト名: DenDen
  - [x] ターゲットフレームワーク: .NET 8.0
  - [x] プロジェクトテンプレート: WPF Application
- [x] **0.2** プロジェクト構造作成
  - [x] Models フォルダ
  - [x] ViewModels フォルダ
  - [x] Views フォルダ
  - [x] Services フォルダ
  - [x] Data フォルダ（Database, Repositories）
  - [x] Common フォルダ（Helpers, Converters）
  - [x] Resources フォルダ
- [x] **0.3** NuGetパッケージインストール
  - [x] Dapper
  - [x] Microsoft.Data.Sqlite
  - [x] Microsoft.Extensions.Configuration
  - [x] Microsoft.Extensions.Configuration.Json
  - [x] Microsoft.Extensions.DependencyInjection
  - [x] BCrypt.Net-Next（パスワードハッシュ化）
  - [x] System.Text.Json
- [x] **0.4** 設定ファイル作成
  - [x] appsettings.json（開発環境用）
  - [x] appsettings.Development.json
  - [x] appsettings.Production.json
- [x] **0.5** Gitリポジトリ初期化
  - [x] .gitignore作成
  - [x] 初回コミット

### 進捗メモ

```
開始日: 2025/01/14
完了日: 2025/01/14
備考:
- MVVMパターンに準拠したフォルダ構造を作成
- 開発環境（SQLite）、本番環境（SQL Server）の設定ファイルを分離
- 必要なNuGetパッケージをすべて追加（Dapper, SQLite, Configuration, BCrypt等）
- Gitリポジトリを初期化し、.gitignoreを作成
```

---

## Phase 1: データベース層

**目的**: データベース抽象化層とRepository層の実装

### タスク一覧

#### 1.1 データベース抽象化層

- [x] **1.1.1** IDbConnectionFactory インターフェース作成
  - [x] CreateMasterConnection()
  - [x] CreateHistoryConnection()
  - [x] CreateCustomerConnection()
- [x] **1.1.2** SqliteConnectionFactory 実装
- [x] **1.1.3** SqlServerConnectionFactory 実装
- [x] **1.1.4** AccessConnectionFactory 実装（OleDb）
- [x] **1.1.5** DatabaseSettings モデル作成
- [x] **1.1.6** DI設定（App.xaml.cs）

#### 1.2 SQLiteデータベース初期化

- [x] **1.2.1** SQLスクリプト作成
  - [x] 設定・マスタDB用スクリプト
  - [x] 問合せ履歴DB用スクリプト
  - [x] テスト用顧客マスタDB用スクリプト
- [x] **1.2.2** DatabaseInitializer サービス作成
  - [x] 初回起動時のDB自動生成
  - [x] 初期データ投入（マスタデータ）
- [x] **1.2.3** 開発用データベースファイル生成
  - [x] Data/Master.db
  - [x] Data/History.db
  - [x] Data/Customer.db

#### 1.3 Repository層

- [x] **1.3.1** IProjectRepository + 実装
- [x] **1.3.2** IOperatorRepository + 実装
- [x] **1.3.3** ICategoryRepository + 実装
- [x] **1.3.4** IStatusRepository + 実装
- [ ] **1.3.5** ICustomFieldDefinitionRepository + 実装（Phase 9で実装予定）
- [ ] **1.3.6** ICustomerMasterDisplayConfigRepository + 実装（Phase 7で実装予定）
- [ ] **1.3.7** IInquiryHistoryRepository + 実装（Phase 8で実装予定）
- [ ] **1.3.8** ICustomerMemoRepository + 実装（Phase 7で実装予定）
- [ ] **1.3.9** ICustomerRepository + 実装（動的クエリ対応）（Phase 6で実装予定）

### 進捗メモ

```
開始日: 2025/01/14
完了日: 2025/01/14
備考:
- データベース抽象化層を完全実装（SQLite, SQL Server, Access対応）
- SQLiteの初期化スクリプトと自動セットアップ機能を実装
- 認証・画面表示に必要な主要Repositoryを実装
- エンティティクラス（ProjectMaster, OperatorMaster, CategoryMaster, StatusMaster）作成
- DIコンテナの設定完了、アプリ起動時にDB自動初期化
- 残りのRepositoryは後のPhaseで必要に応じて実装
```

---

## Phase 2: モデル層

**目的**: ドメインモデルとDTOの実装

### タスク一覧

- [x] **2.1** エンティティクラス作成
  - [x] ProjectMaster
  - [x] OperatorMaster
  - [x] CategoryMaster
  - [x] StatusMaster
  - [x] CustomFieldDefinition
  - [x] CustomerMasterDisplayConfig
  - [x] CustomerMasterColumnConfig
  - [x] HistoryLogSettings
  - [x] InquiryHistory
  - [x] InquiryHistoryLog
  - [x] CustomerMemo
- [x] **2.2** DTOクラス作成
  - [x] LoginRequest
  - [x] LoginResponse
  - [x] CustomerSearchRequest
  - [x] CustomerSearchResult
  - [x] InquirySearchRequest
  - [x] InquirySearchResult
- [x] **2.3** ViewModelベースクラス作成
  - [x] ViewModelBase（INotifyPropertyChanged実装）
  - [x] RelayCommand（ICommand実装）

### 進捗メモ

```
開始日: 2025/01/14
完了日: 2025/01/14
備考:
- すべてのエンティティクラスを作成（11クラス）
- ログイン、検索用のDTOクラスを作成（6クラス）
- MVVMパターンの基盤となるViewModelBaseとRelayCommandを実装
- SetProperty、OnPropertyChangedによる効率的なプロパティ変更通知
- 汎用的なRelayCommand<T>でパラメータ付きコマンドもサポート
```

---

## Phase 3: 共通基盤・インフラ

**目的**: 認証、セッション管理、共通サービスの実装

### タスク一覧

- [x] **3.1** 認証サービス
  - [x] IAuthenticationService インターフェース
  - [x] AuthenticationService 実装（BCrypt対応）
- [x] **3.2** セッション管理
  - [x] SessionManager クラス
  - [x] 現在のオペレーター情報保持
  - [x] 現在の案件情報保持
- [x] **3.3** 設定管理サービス
  - [x] IConfigurationService インターフェース
  - [x] ConfigurationService 実装
  - [x] appsettings.json読み込み
- [x] **3.4** ダイアログサービス
  - [x] IDialogService インターフェース
  - [x] DialogService 実装
  - [x] ShowMessage, ShowConfirm, ShowError
- [x] **3.5** ナビゲーションサービス
  - [x] INavigationService インターフェース
  - [x] NavigationService 実装
  - [x] 画面遷移管理
- [x] **3.6** バリデーションヘルパー
  - [x] ValidationHelper クラス
  - [x] 共通バリデーションルール

### 進捗メモ

```
開始日: 2025/01/15
完了日: 2025/01/15
備考:
- IAuthenticationService + AuthenticationService を作成（BCrypt対応）
- SessionManager を作成（ログイン状態管理）
- IConfigurationService + ConfigurationService を作成（設定読み込み）
- IDialogService + DialogService を作成（MessageBox抽象化）
- INavigationService + NavigationService を作成（画面遷移管理）
- ValidationHelper を作成（共通バリデーションルール）
- App.xaml.cs に全サービスのDI登録を追加
- CustomerMasterSettings, CustomFieldSearchSettings, ImageSettings モデルを追加
```

---

## Phase 4: ログイン機能

**目的**: ログイン画面とログイン機能の実装

### タスク一覧

- [x] **4.1** LoginViewModel 実装
  - [x] プロパティ（ProjectList, LoginID, Password）
  - [x] LoginCommand
  - [x] バリデーション
  - [x] 認証処理
- [x] **4.2** LoginView (XAML) 作成
  - [x] UI要素配置
  - [x] データバインディング設定
  - [x] スタイル適用
- [x] **4.3** MainWindow起動フロー修正
  - [x] 初回起動時LoginView表示
  - [x] ログイン成功後メイン画面表示
- [x] **4.4** テスト用オペレーターデータ投入
  - [x] 管理者アカウント（admin / admin123）
  - [x] 一般オペレーター（operator001 / operator123）
- [x] **4.5** 動作確認
  - [x] ログイン成功パターン
  - [x] ログイン失敗パターン
  - [x] バリデーションエラー

### 進捗メモ

```
開始日: 2025/01/15
完了日: 2025/01/15
備考:
- LoginViewModel を作成（プロジェクト選択、ログインID、パスワード入力）
- LoginView を作成（中央配置のログインフォーム）
- 共通コンバーター作成（BoolToVisibility, InverseBool, StringToVisibility）
- App.xaml にコンバーターとスタイルを登録
- MainWindow を更新（ログイン→メイン画面切替）
- DatabaseInitializer でパスワードハッシュを自動生成
```

---

## Phase 5: メイン画面基盤

**目的**: メイン画面のレイアウトと基本構造の実装

### タスク一覧

- [x] **5.1** MainViewModel 実装
  - [x] セッション情報表示
  - [x] LogoutCommand
  - [x] 子ViewModelの管理
- [x] **5.2** MainView (XAML) 作成
  - [x] ヘッダーエリア（60px）
  - [x] 左パネル（600px）
  - [x] 右パネル（1270px）
  - [x] レイアウト調整
- [x] **5.3** 左パネル - 検索エリア基盤
  - [x] TabControl作成
  - [x] タブ動的生成準備
- [x] **5.4** 左パネル - 検索結果エリア
  - [x] ListBox/ListView作成
  - [x] ItemTemplate設定
- [x] **5.5** 左パネル - 顧客情報エリア基盤
  - [x] TabControl作成
  - [x] 動的タブ生成準備
- [x] **5.6** 右パネル - 問合せ登録エリア基盤
  - [x] 基本レイアウト作成
  - [x] ボタン配置

### 進捗メモ

```
開始日: 2025/01/15
完了日: 2025/01/15
備考:
- MainViewModel を作成（セッション情報表示、ログアウト処理）
- MainView を作成（ヘッダー、左パネル、右パネルの3分割レイアウト）
- ヘッダーにタイトル、セッション情報、ログアウトボタンを配置
- 左パネルに検索、検索結果、顧客情報エリアを配置
- 右パネルに問合せ登録フォームのプレースホルダーを配置
- 検索機能は Phase 6 で実装予定
- 問合せ登録機能は Phase 8 で実装予定
```

---

## Phase 6: 検索機能

**目的**: 顧客マスタ検索、カスタム項目検索、問合せ履歴検索の実装

### タスク一覧

#### 6.1 顧客マスタ検索

- [x] **6.1.1** SearchPanelViewModel 実装（統合ViewModel）
  - [x] 顧客ID、顧客名、電話番号での検索
  - [x] SearchCustomerCommand
  - [x] ClearSearchCommand
  - [x] CustomerSearchResults（ObservableCollection）
- [x] **6.1.2** ICustomerRepository + CustomerRepository 実装
  - [x] SearchAsync（顧客検索）
  - [x] GetByIdAsync（顧客詳細取得）
  - [x] GetCustomerDataAsync（全カラム取得）
- [x] **6.1.3** 検索UI（MainView内）
  - [x] 顧客ID、顧客名、電話番号の入力フィールド
  - [x] 検索・クリアボタン
- [x] **6.1.4** 検索結果DataGrid表示
  - [x] 顧客Key、顧客名、電話番号を表示
  - [x] 選択時にCustomerSelectedイベント発火

#### 6.2 カスタム項目検索

- [ ] **6.2.1** CustomFieldSearchViewModel 実装（Phase 9で実装予定）

#### 6.3 問合せ履歴検索

- [x] **6.3.1** SearchPanelViewModel に問合せ履歴検索を統合
  - [x] 受電日（開始・終了）
  - [x] カテゴリー、ステータス
  - [x] キーワード
- [x] **6.3.2** IInquiryHistoryRepository + InquiryHistoryRepository 実装
  - [x] SearchAsync（条件検索）
  - [x] GetByCustomerKeyAsync（顧客の問合せ履歴取得）
  - [x] GetByIdAsync, InsertAsync, UpdateAsync
- [x] **6.3.3** 検索UI（MainView内）
  - [x] 期間、カテゴリ、ステータス、キーワードの入力
  - [x] 検索・クリアボタン
- [x] **6.3.4** 検索結果DataGrid表示
  - [x] 受付日時、顧客Key、問合せ内容を表示

### 進捗メモ

```
開始日: 2025/01/15
完了日: 2025/01/15
備考:
- SearchPanelViewModelで顧客検索と問合せ履歴検索を統合
- ICustomerRepository + CustomerRepository を実装（SQLite対応）
- IInquiryHistoryRepository + InquiryHistoryRepository を実装
- MainView.xamlに検索UIとデータバインディングを統合
- カスタム項目検索はPhase 9で実装予定
```

---

## Phase 7: 顧客情報表示

**目的**: 顧客情報の動的表示、画像表示、顧客メモ、過去履歴表示

### タスク一覧

- [x] **7.1** CustomerInfoViewModel 実装
  - [x] CurrentCustomer プロパティ
  - [x] CustomerData（全カラムデータ Dictionary）
  - [x] CustomerInfoItems（表示用リスト）
  - [x] InquiryHistories（過去問合せ履歴）
  - [x] LoadCustomerAsync メソッド
  - [x] Clear メソッド
- [x] **7.2** 顧客情報表示
  - [x] 基本情報タブ（カラム名と値のリスト表示）
  - [x] 表示名のマッピング（CustomerID→顧客ID 等）
  - [x] DataGridでの表示
- [ ] **7.3** 関連画像表示（将来実装）
  - [ ] 画像DBからパス取得
  - [ ] サムネイル表示
- [ ] **7.4** 顧客メモ（将来実装）
  - [ ] 表示処理
  - [ ] 編集処理
- [x] **7.5** 過去問合せ履歴表示
  - [x] InquiryHistory取得（顧客Key + プロジェクトIDで取得）
  - [x] DataGridでリスト表示
  - [x] SelectedInquiryでの選択連携
- [x] **7.6** 顧客紐付けセクション
  - [x] 紐付けボタン（顧客未紐付け時のみ有効）
  - [x] 解除ボタン（顧客紐付け時のみ有効）
  - [x] 登録・更新時に顧客Keyも保存

### 進捗メモ

```
開始日: 2025/01/15
完了日: 2025/01/15
備考:
- CustomerInfoViewModelを実装（顧客情報表示、過去履歴表示）
- MainView.xamlに顧客情報エリアを統合（基本情報タブ、過去履歴タブ）
- 検索結果の顧客を選択すると、CustomerInfoViewModelに顧客情報がロードされる
- MainViewModelがSearchPanelとCustomerInfoを統合管理
- 顧客紐付け機能を実装（紐付け・解除ボタン、登録・更新時に顧客Keyも保存）
- 画像表示、顧客メモは将来のPhaseで実装予定
```

---

## Phase 8: 問合せ登録・更新

**目的**: 問合せの新規登録と既存履歴の更新機能

### タスク一覧

- [x] **8.1** InquiryViewModel 実装
  - [x] 受電日時、オペレーター（自動）
  - [x] カテゴリー、ステータス
  - [x] 問合せ内容、対応内容
  - [x] SaveCommand（新規・更新共通）
  - [x] ClearCommand
  - [x] NewCommand（編集モード時）
- [x] **8.2** InquiryView (XAML) 作成
  - [x] 入力フォーム（MainView.xaml右パネルに統合）
  - [x] ボタン配置
- [x] **8.3** 新規登録処理
  - [x] バリデーション
  - [x] INSERT処理
  - [x] 成功・失敗処理
- [x] **8.4** 既存履歴更新処理
  - [x] バリデーション
  - [x] InquiryHistoryLogへの退避
  - [x] UPDATE処理
- [x] **8.5** クリア処理
  - [x] 確認ダイアログ
  - [x] フィールドクリア
- [x] **8.6** モード管理
  - [x] 新規入力モード
  - [x] 編集モード
  - [x] ボタン表示切り替え

### 進捗メモ

```
開始日: 2025/01/15
完了日: 2025/01/15
備考:
- InquiryViewModelを作成（新規登録・更新・クリア・モード切替）
- MainView.xaml右パネルに問合せ登録フォームを統合
- IInquiryHistoryLogRepository + InquiryHistoryLogRepositoryを作成
- 更新時に更新前の状態をInquiryHistoryLogに保存
- 検索結果や過去履歴からの選択で編集モードに切り替え
```

---

## Phase 9: カスタム項目

**目的**: カスタム項目の動的生成と入力処理

### タスク一覧

- [x] **9.1** CustomFieldDefinition読み込み
  - [x] ICustomFieldDefinitionRepository + CustomFieldDefinitionRepository作成
  - [x] IsEnabled=1の項目のみ取得
  - [x] DisplayOrder順にソート
- [x] **9.2** カスタム項目UI動的生成
  - [x] Text型: TextBox
  - [x] Number型: TextBox（数値入力）
  - [x] Date型: DatePicker
  - [x] Select型: ComboBox（JSON解析）
  - [x] FieldTypeToVisibilityConverterで型別表示切替
- [x] **9.3** バリデーション
  - [x] IsRequired=1の項目チェック
  - [x] 必須項目未入力時のエラー表示
- [x] **9.4** 保存処理
  - [x] CustomCol01~10へのマッピング
  - [x] INSERT/UPDATE対応
- [ ] **9.5** カスタム項目検索との連携（将来実装）
  - [ ] IsSearchable=1の項目を検索に使用

### 進捗メモ

```
開始日: 2025/01/15
完了日: 2025/01/15
備考:
- ICustomFieldDefinitionRepository + CustomFieldDefinitionRepositoryを作成
- InquiryViewModelにCustomFieldsプロパティを追加（動的カスタム項目リスト）
- MainView.xamlにItemsControlでカスタム項目を動的生成
- FieldTypeToVisibilityConverterで型別コントロール切替
- テストデータ: 発信者名(Text), 発信者電話番号(Text), 優先度(Select: 低/中/高)
- カスタム項目検索との連携は将来のPhaseで実装予定
```

---

## Phase 10: エクスポート・レポート

**目的**: CSV出力とレポート機能の実装

### タスク一覧

- [x] **10.1** CSV出力機能
  - [x] IExportService インターフェース作成
  - [x] ExportService 作成
  - [x] 問合せ履歴検索結果をCSV出力
  - [x] ファイル名: `問合せ履歴_YYYYMMDD_HHMMSS.csv`
  - [x] UTF-8 with BOM
  - [x] SaveFileDialog
- [x] **10.2** レポート機能
  - [x] ReportViewModel 作成
  - [x] ReportView (XAML/Window) 作成
  - [x] 問合せ履歴一覧表示（DataGrid）
  - [x] 印刷機能（FlowDocument使用）
- [x] **10.3** メイン画面へのボタン追加
  - [x] CSV出力ボタン（検索結果エリア）
  - [x] レポート表示ボタン（検索結果エリア）
  - [x] PositiveToBoolConverter追加

### 進捗メモ

```
開始日: 2025/01/16
完了日: 2025/01/16
備考:
- IExportService + ExportService を作成（CSV出力機能）
- CSVエスケープ処理実装（カンマ、ダブルクォート、改行対応）
- ReportViewModel + ReportView を作成（レポート表示・印刷機能）
- FlowDocumentを使用した印刷機能を実装
- MainViewModelにCSV出力・レポート表示コマンドを追加
- 検索結果エリアにCSV出力・レポートボタンを配置
- 問合せ履歴検索結果がある場合のみボタンが有効になる

【追加実装: 問合せ印刷・プレビュー機能】
- ReportCustomerDisplayConfigテーブル追加（レポート用顧客表示項目設定、最大3項目）
- PrintPreviewWindow作成（DocumentViewer使用の共通プレビューウィンドウ）
- IInquiryPrintService + InquiryPrintService作成（問合せ印刷用FlowDocument生成）
- 顧客情報をレポートに表示（DB設定に基づく最大3項目）
- InquiryViewModelに印刷・プレビューコマンド追加
- 問合せ登録エリアの左下に「プレビュー」「印刷」ボタンを配置
- DatabaseInitializerにマイグレーション処理追加（既存DBへのテーブル追加対応）
```

---

## Phase 11: テスト・品質保証

**目的**: 単体テスト、結合テスト、品質チェック

### タスク一覧

#### 11.1 単体テスト

- [x] **11.1.1** テストプロジェクト作成（xUnit）
- [x] **11.1.2** Repository層テスト
  - [x] ProjectRepository
  - [x] OperatorRepository
  - [x] InquiryHistoryRepository
- [x] **11.1.3** サービス層テスト
  - [x] AuthenticationService
  - [x] ExportService
- [x] **11.1.4** ViewModel層テスト
  - [x] LoginViewModel

#### 11.2 結合テスト

- [ ] **11.2.1** ログインフロー（手動テスト実施）
- [ ] **11.2.2** 顧客検索 → 問合せ登録フロー（手動テスト実施）
- [ ] **11.2.3** 問合せ履歴検索 → 更新フロー（手動テスト実施）
- [ ] **11.2.4** 顧客紐付けフロー（手動テスト実施）
- [ ] **11.2.5** CSV出力フロー（手動テスト実施）

#### 11.3 品質チェック

- [x] **11.3.1** コードレビュー
- [x] **11.3.2** MVVM準拠チェック（Viewはイベントハンドラのみ）
- [ ] **11.3.3** メモリリーク確認
- [ ] **11.3.4** パフォーマンステスト
- [x] **11.3.5** セキュリティチェック
  - [x] SQLインジェクション対策（パラメータ化クエリ使用）
  - [x] パスワードハッシュ化確認（BCrypt workFactor:12）

#### 11.4 受入テスト準備

- [ ] **11.4.1** テストデータ作成
- [ ] **11.4.2** テストシナリオ作成
- [ ] **11.4.3** テスト環境構築（SQL Server）

### 進捗メモ

```
開始日: 2025/01/16
完了日: 2025/01/16
備考:
- DenDen.Testsプロジェクト作成（xUnit + Moq + FluentAssertions）
- Repository層テスト: ProjectRepository, OperatorRepository, InquiryHistoryRepository
- サービス層テスト: AuthenticationService, ExportService（CSVエスケープ、UTF-8 BOM）
- ViewModel層テスト: LoginViewModel（プロパティ変更通知、コマンド確認）
- セキュリティチェック: SQLインジェクション対策確認、BCrypt使用確認
- MVVM準拠確認: Viewコードビハインドはイベントハンドラのみ
```

---

## Phase 12: デプロイ準備

**目的**: ClickOnce配布準備とドキュメント整備

### タスク一覧

- [ ] **12.1** 本番用設定ファイル作成
  - [ ] appsettings.Production.json
  - [ ] SQL Server接続文字列設定
- [ ] **12.2** ClickOnce設定
  - [ ] 発行設定
  - [ ] 更新設定
  - [ ] 必須コンポーネント設定
- [ ] **12.3** インストーラー作成
  - [ ] .NET 8.0 Runtime組み込み
- [ ] **12.4** ドキュメント作成
  - [ ] ユーザーマニュアル
  - [ ] セットアップガイド
  - [ ] 運用マニュアル
- [ ] **12.5** SQL Serverセットアップスクリプト
  - [ ] データベース作成スクリプト
  - [ ] 初期データ投入スクリプト
  - [ ] マイグレーションスクリプト

### 進捗メモ

```
開始日:
完了日:
備考:
```

---

## 追加機能・改善タスク（オプション）

**優先度が低い、または将来的に実装する機能**

- [ ] ログ機能強化
  - [ ] NLog/Serilog導入
  - [ ] エラーログ、操作ログの記録
- [ ] マイグレーション機能
  - [ ] DbUp/FluentMigrator導入
  - [ ] バージョン管理
- [ ] 高度な検索機能
  - [ ] 全文検索
  - [ ] 複合条件検索
- [ ] ダッシュボード
  - [ ] 問合せ件数の統計
  - [ ] グラフ表示
- [ ] 多言語対応
  - [ ] リソースファイル化
  - [ ] 言語切り替え機能

---

## 開発ルール・規約

### コーディング規約

- **命名規則**: C#標準に準拠（PascalCase, camelCase）
- **クラス名**: 単数形、名詞
- **メソッド名**: 動詞 + 名詞
- **プライベートフィールド**: `_camelCase`
- **コメント**: 複雑なロジックには必須

### MVVMルール

- **View**: コードビハインドは最小限（イベントハンドラのみ）
- **ViewModel**: ビジネスロジック、プロパティ、コマンド
- **Model**: データ構造、ドメインロジック
- **ViewとViewModelの分離**: ViewはViewModelに依存、逆は不可

### Git運用

- **ブランチ戦略**: feature/機能名
- **コミットメッセージ**: `[Phase X.Y] タスク名: 変更内容`
- **プルリクエスト**: フェーズ単位

### レビューチェックリスト

- [ ] MVVMパターンに準拠しているか
- [ ] using文でリソース解放しているか
- [ ] 非同期処理は適切か（async/await）
- [ ] バリデーションは実装されているか
- [ ] エラーハンドリングは適切か
- [ ] コメントは十分か

---

## 課題・ブロッカー

| 日付 | 課題内容 | 状態 | 解決日 | 備考 |
|------|---------|------|--------|------|
| - | - | - | - | - |

---

## 変更履歴

| 日付 | 版数 | 変更内容 | 変更者 |
|------|------|---------|--------|
| 2025/01/14 | 1.0 | 初版作成 | - |
| 2025/01/15 | 1.1 | Phase 3-7 実装完了 | - |
| 2025/01/15 | 1.2 | ログイン画面に終了ボタン追加、検索ボタン高さ調整(36px)、問合せ履歴検索のキーワードをオペレータ選択に変更 | - |
| 2025/01/15 | 1.3 | Phase 8-9 実装完了（問合せ登録・更新、カスタム項目動的生成、履歴ログ保存） | - |
| 2025/01/15 | 1.4 | 顧客紐付け機能実装（紐付け・解除ボタン、未紐付け時のみ紐付け可能、更新時に顧客Key保存） | - |
| 2025/01/16 | 1.5 | Phase 10 エクスポート・レポート実装完了（CSV出力、レポート表示・印刷機能） | - |
| 2025/01/16 | 1.6 | 問合せ印刷・プレビュー機能追加（DocumentViewer、顧客情報表示設定DB対応） | - |
| 2025/01/16 | 1.7 | Phase 11 テスト・品質保証完了（xUnit、Repository/Service/ViewModelテスト、セキュリティチェック） | - |

---

## 次回作業予定

```
次回着手フェーズ: Phase 12 デプロイ準備
予定作業時間:
目標: ClickOnce配布準備、本番用設定ファイル、ドキュメント整備
```

---

**備考**:
- 各フェーズ完了時に「状態」を更新すること
- 課題が発生した場合は「課題・ブロッカー」セクションに記録すること
- 定期的に進捗を確認し、計画を見直すこと
