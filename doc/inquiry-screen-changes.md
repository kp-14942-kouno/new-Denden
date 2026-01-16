# 問合せ登録画面の構成変更

## 変更一覧

| No | 変更内容 | 概要 |
|----|---------|------|
| 1 | 新規登録の表示制御 | 起動時は空表示、ヘッダーに新規登録ボタン配置、未保存時の確認アラート |
| 2 | 名称統一 | 「顧客Key」→「顧客ID」に統一 |
| 3 | 問合せ・対応内容のレイアウト | 縦並び→横並び（左:問合せ、右:対応） |
| 4 | 解除ボタン削除 | 顧客IDの「解除」ボタンと機能を削除 |
| 5 | フィールド配置変更 | 左側:顧客ID/カテゴリー/ステータス、右側:日時/オペレーター情報 |
| 6 | 必須項目追加 | カテゴリー・ステータスを必須に |
| 7 | フォントサイズ拡縮 | Shift+ホイールでフォントサイズ変更 |
| 8 | 印刷・プレビュー改善 | 全項目表示、QRコード、カスタム項目設定 |
| 9 | 検索結果ラベル表示 | 検索元に応じて「[顧客マスタ]検索結果」等を表示 |

---

## 実装フェーズ（効率的な順序）

### フェーズA: 基盤となる小さな変更
**理由**: 他の変更に依存せず、先に済ませるとレイアウト変更がシンプルになる

#### A-1: 変更2 - 名称統一（顧客Key→顧客ID）
**対象ファイル**:
- `Views/MainView.xaml` - ラベル変更
- `ViewModels/InquiryViewModel.cs` - 必要に応じてプロパティ名確認

**修正箇所**:
- MainView.xaml 407行目: `Text="顧客Key"` → `Text="顧客ID"`
- MainView.xaml 201行目: DataGrid列ヘッダー `Header="顧客Key"` → `Header="顧客ID"`
- MainView.xaml 222行目: DataGrid列ヘッダー `Header="顧客Key"` → `Header="顧客ID"`

#### A-2: 変更4 - 解除ボタン削除
**対象ファイル**:
- `Views/MainView.xaml` - ボタン削除
- `ViewModels/InquiryViewModel.cs` - UnlinkCustomerCommand関連削除

**修正箇所**:
- MainView.xaml 418-419行目: 解除ボタン削除
- InquiryViewModel.cs: UnlinkCustomerCommand、UnlinkCustomerメソッド削除

#### A-3: 変更6 - カテゴリー・ステータス必須化
**対象ファイル**:
- `Views/MainView.xaml` - 必須マーク追加
- `ViewModels/InquiryViewModel.cs` - バリデーション追加

**修正箇所**:
- MainView.xaml: カテゴリー・ステータスラベルに `<Run Text="*" Foreground="Red"/>` 追加
- InquiryViewModel.cs Validate(): SelectedCategory/SelectedStatusのnullチェック追加

#### A-4: 変更9 - 検索結果ラベル表示
**対象ファイル**:
- `Views/MainView.xaml` - 検索結果ラベルのバインディング変更
- `ViewModels/SearchPanelViewModel.cs` - 検索結果ラベル用プロパティ追加

**実装方針**（将来のカスタム項目検索も考慮）:
```csharp
// 検索タイプの列挙型（拡張可能）
public enum SearchType
{
    None,
    CustomerMaster,    // 顧客マスタ
    InquiryHistory,    // 問合せ履歴
    // CustomField,    // 将来追加予定
}

// 現在の検索タイプ
public SearchType CurrentSearchType { get; set; }

// 検索結果ラベル（バインディング用）
public string SearchResultLabel => CurrentSearchType switch
{
    SearchType.CustomerMaster => "[顧客マスタ]検索結果",
    SearchType.InquiryHistory => "[問合せ履歴]検索結果",
    // SearchType.CustomField => "[カスタム項目]検索結果",
    _ => "検索結果"
};
```

**修正箇所**:
- MainView.xaml 173行目: `Text="検索結果"` → `Text="{Binding SearchResultLabel}"`
- SearchPanelViewModel.cs: 検索実行時に `CurrentSearchType` を設定

---

### フェーズB: 画面モード制御
**理由**: レイアウト変更前に状態管理を整理

#### B-1: 変更1 - 新規登録の表示制御
**対象ファイル**:
- `Views/MainView.xaml` - ヘッダーに新規ボタン追加、フォーム表示制御
- `ViewModels/InquiryViewModel.cs` - 状態プロパティ追加、確認ダイアログ処理

**画面の3つの状態**:

| 状態 | フォーム | 新規ボタン | 説明 |
|------|---------|-----------|------|
| 空表示（起動時・クリア後） | 非表示（項目自体なし） | 表示 | ヘッダーとボタンの間は何もない |
| 新規入力中 | 表示 | 非表示 | 既に新規入力中なので不要 |
| 履歴表示中（編集モード） | 表示 | 表示 | 新規登録に切り替え可能 |

**新規プロパティ**:
```csharp
// 新規入力中かどうか（新規ボタン押下でtrue、登録完了orクリアでfalse）
public bool IsNewEntryMode { get; set; }

// フォームの表示状態（新規入力中または編集モード時にtrue）
public bool IsFormVisible => IsNewEntryMode || IsEditMode;

// 新規ボタンの表示状態（新規入力中でない時に表示）
public bool IsNewButtonVisible => !IsNewEntryMode;

// 入力内容があるかどうか（未保存確認用）
public bool HasUnsavedChanges { get; }
```

**修正内容**:
1. ヘッダー行（370-372行目）に「新規」ボタン追加（右端配置）
2. ボタンは `IsNewButtonVisible`（= `!IsNewEntryMode`）の時に表示
3. フォーム全体を `IsFormVisible` でVisibility制御
4. 起動時・クリア時は `IsNewEntryMode = false`、`IsEditMode = false` → フォーム非表示、新規ボタン表示
5. 新規ボタン押下で `IsNewEntryMode = true` → フォーム表示、新規ボタン非表示
6. 履歴選択時は `IsEditMode = true`、`IsNewEntryMode = false` → フォーム表示、新規ボタン表示
7. 新規入力中または編集中に履歴選択時、`HasUnsavedChanges` が true なら確認ダイアログ表示
8. 編集モードで新規ボタン押下時も `HasUnsavedChanges` が true なら確認ダイアログ表示
9. 下部の「新規」ボタンは削除（ヘッダーに移動するため）

---

### フェーズC: 画面レイアウト変更
**理由**: フェーズA/Bの変更を踏まえた上でレイアウトを変更

#### C-1: 変更5 - フィールド配置の大幅変更
**対象ファイル**:
- `Views/MainView.xaml` - Grid構造の大幅変更

**新レイアウト構造**:
```
┌─────────────────────────────────────────────────────────┐
│ 問合せ登録                        [新規登録ボタン]       │
├───────────────────────┬─────────────────────────────────┤
│ 顧客ID [____] [紐付け]│ 受電日時 [___] オペレーター [___] │
├───────────────────────┼─────────────────────────────────┤
│ カテゴリー * [▼____]  │ 最終更新 [___] 更新者 [________] │
├───────────────────────┼─────────────────────────────────┤
│ ステータス * [▼____]  │                                 │
├───────────────────────┴─────────────────────────────────┤
│ 問合せ内容 *              │  対応内容                    │（横並び）
│ [________________]        │  [________________]          │
├─────────────────────────────────────────────────────────┤
│ カスタム項目                                             │
└─────────────────────────────────────────────────────────┘
```

**幅のバランス**:
- 日時: 約140px（短め）
- オペレーター名: 残り幅（長め）

#### C-2: 変更3 - 問合せ内容・対応内容の横並び
**フェーズC-1と同時に実装**

---

### フェーズD: 機能追加

#### D-1: 変更7 - フォントサイズ拡縮機能
**対象ファイル**:
- `Views/MainView.xaml` - TextBoxにイベントハンドラ追加
- `Views/MainView.xaml.cs` - イベントハンドラ実装

**実装内容**:
```csharp
private void TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
{
    if (Keyboard.Modifiers == ModifierKeys.Shift && sender is TextBox textBox)
    {
        double delta = e.Delta > 0 ? 1 : -1;
        double newSize = textBox.FontSize + delta;
        if (newSize >= 8 && newSize <= 32)
        {
            textBox.FontSize = newSize;
        }
        e.Handled = true;
    }
}
```

- 問合せ内容・対応内容のTextBoxに `PreviewMouseWheel` イベント追加
- 最小8pt、最大32ptの制限

---

### フェーズE: 印刷・プレビュー改善

#### E-1: 変更8 - 印刷・プレビュー機能の大幅改善
**対象ファイル**:
- `Services/InquiryPrintService.cs` - ドキュメント生成ロジック全面改修
- `Services/IInquiryPrintService.cs` - インターフェース更新（必要に応じて）
- `ViewModels/InquiryViewModel.cs` - 印刷データ生成部分の更新
- 新規: 印刷用カスタム項目設定（設定ファイルまたはDB）

**レイアウト設計（A4縦）**:
```
┌─────────────────────────────────────────┐
│ [案件名]                        [QRコード]│ ← ヘッダー
├─────────────────────────────────────────┤
│ 受電日時: xxxx  オペレーター: xxxx       │ ← 1行
│ 最終更新: xxxx  更新者: xxxx             │ ← 1行
│ カテゴリー: xxxx  ステータス: xxxx        │ ← 1行
│ 顧客ID: xxxx                            │ ← 1行
│ カスタム1: xxx  カスタム2: xxx  カスタム3: xxx │ ← 1行（最大3項目）
├─────────────────────────────────────────┤
│ 問合せ内容                               │
│ [                                       ]│ ← スペース最大化
│ [                                       ]│
├─────────────────────────────────────────┤
│ 対応内容                                 │
│ [                                       ]│ ← スペース最大化
│ [                                       ]│
├─────────────────────────────────────────┤
│ 出力日時: xxxx/xx/xx xx:xx:xx           │ ← フッター
└─────────────────────────────────────────┘
```

**QRコード実装**:
- NuGet: `QRCoder` または `ZXing.Net` を追加
- 顧客IDをQRコードに変換して表示

**カスタム項目設定**:
- 印刷に含めるカスタム項目を最大3つまで設定可能
- appsettings.jsonまたはDBで管理

---

## 修正ファイル一覧

| ファイル | フェーズ | 修正内容 |
|---------|---------|---------|
| Views/MainView.xaml | A,B,C,D | レイアウト全面改修 |
| Views/MainView.xaml.cs | D | フォントサイズ変更イベント |
| ViewModels/InquiryViewModel.cs | A,B | 状態管理、バリデーション、コマンド |
| ViewModels/SearchPanelViewModel.cs | A | 検索結果ラベル、SearchType列挙型 |
| ViewModels/MainViewModel.cs | B | 履歴選択時の確認処理 |
| Services/InquiryPrintService.cs | E | 印刷ドキュメント生成改修 |
| Services/IInquiryPrintService.cs | E | インターフェース更新 |

---

## 検証方法

### フェーズA検証
1. アプリ起動
2. 「顧客ID」表記の確認（画面・検索結果）
3. 解除ボタンがないことを確認
4. カテゴリー・ステータス未選択で登録→エラー表示確認
5. 顧客マスタ検索実行→「[顧客マスタ]検索結果」と表示される
6. 問合せ履歴検索実行→「[問合せ履歴]検索結果」と表示される

### フェーズB検証
1. 起動時：フォームが非表示（項目自体なし）、「新規」ボタンが表示されていることを確認
2. 「新規」ボタン押下：フォーム表示、「新規」ボタン非表示
3. 新規入力中にクリアボタン押下：フォーム非表示、「新規」ボタン表示（空表示に戻る）
4. 新規入力中に履歴選択：確認アラート表示 → OKでフォームに履歴表示、「新規」ボタン表示
5. 履歴表示中（編集モード）：フォーム表示、「新規」ボタン表示されていることを確認
6. 編集モードで「新規」ボタン押下：確認アラート表示 → OKで新規入力モードへ

### フェーズC検証
1. 新しいレイアウトの確認
2. 左側: 顧客ID、カテゴリー、ステータス（縦並び）
3. 右側: 受電日時+オペレーター、最終更新+更新者
4. 問合せ内容・対応内容が横並び

### フェーズD検証
1. 問合せ内容にフォーカス
2. Shift+ホイール上下でフォントサイズ変更確認
3. 対応内容でも同様に確認

### フェーズE検証
1. 問合せ登録後にプレビュー表示
2. ヘッダーに案件名、QRコード（顧客IDあり時）表示確認
3. 全項目の値が表示されていることを確認
4. カスタム項目（最大3つ）の表示確認
5. 印刷実行してA4サイズ確認
