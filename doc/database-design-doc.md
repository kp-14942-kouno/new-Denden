# コール業務管理システム データベース設計書

## 目次
1. [データベース構成概要](#1-データベース構成概要)
2. [グループ②: 設定・マスタDB](#2-グループ②-設定マスタdb)
3. [グループ③: 問合せ履歴DB](#3-グループ③-問合せ履歴db)
4. [グループ①: 顧客マスタ・画像DB](#4-グループ①-顧客マスタ画像db)
5. [ER図](#5-er図)
6. [インデックス設計](#6-インデックス設計)
7. [データ型定義](#7-データ型定義)
8. [初期データ](#8-初期データ)

---

## 1. データベース構成概要

### 1.1 3層分離設計

```
┌─────────────────────────────────────────┐
│ グループ①: 顧客マスタ・画像DB          │
│ - 既存DB参照（読み取り専用）            │
│ - 案件ごとにテーブル構造が異なる        │
│ - SQL Server / Access                   │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ グループ②: 設定・マスタDB              │
│ - アプリケーション設定                  │
│ - マスタデータ                          │
│ - 更新頻度: 低                          │
│ - SQL Server / Access                   │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ グループ③: 問合せ履歴DB                │
│ - トランザクションデータ                │
│ - 日々増加するデータ                    │
│ - 更新頻度: 高                          │
│ - SQL Server / Access                   │
└─────────────────────────────────────────┘
```

### 1.2 接続設定
各グループは独立したDBとして管理され、接続情報はconfig.jsonに記載

---

## 2. グループ②: 設定・マスタDB

### 2.1 案件マスタ（ProjectMaster）

**用途**: 案件情報の管理（複数案件を1DBで管理する場合に使用）

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| ProjectID | INTEGER | NOT NULL | PK | 案件ID（自動採番） |
| ProjectCode | NVARCHAR(50) | NOT NULL | UNIQUE | 案件コード（例: ProjectA） |
| ProjectName | NVARCHAR(100) | NOT NULL | - | 案件名 |
| Description | NVARCHAR(500) | NULL | - | 説明 |
| IsActive | BIT | NOT NULL | - | 有効/無効（デフォルト: 1） |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |
| UpdatedAt | DATETIME | NULL | - | 更新日時 |

**制約**:
- PRIMARY KEY: ProjectID
- UNIQUE: ProjectCode

**SQL Server用 CREATE文**:
```sql
CREATE TABLE ProjectMaster (
    ProjectID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectCode NVARCHAR(50) NOT NULL UNIQUE,
    ProjectName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME
);
```

**Access用 CREATE文**:
```sql
CREATE TABLE ProjectMaster (
    ProjectID AUTOINCREMENT PRIMARY KEY,
    ProjectCode TEXT(50) NOT NULL,
    ProjectName TEXT(100) NOT NULL,
    Description TEXT(500),
    IsActive YESNO NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME,
    CONSTRAINT UK_ProjectCode UNIQUE (ProjectCode)
);
```

---

### 2.2 オペレーターマスタ（OperatorMaster）

**用途**: オペレーターのログイン情報と権限管理

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| OperatorID | INTEGER | NOT NULL | PK | オペレーターID（自動採番） |
| ProjectID | INTEGER | NOT NULL | FK | 案件ID |
| LoginID | NVARCHAR(50) | NOT NULL | - | ログインID |
| OperatorName | NVARCHAR(100) | NOT NULL | - | オペレーター名 |
| PasswordHash | NVARCHAR(255) | NOT NULL | - | パスワードハッシュ（BCrypt） |
| Role | NVARCHAR(20) | NOT NULL | - | 権限（General / Admin） |
| IsActive | BIT | NOT NULL | - | 有効/無効（デフォルト: 1） |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |
| UpdatedAt | DATETIME | NULL | - | 更新日時 |

**制約**:
- PRIMARY KEY: OperatorID
- FOREIGN KEY: ProjectID → ProjectMaster(ProjectID)
- UNIQUE: (ProjectID, LoginID)

**SQL Server用 CREATE文**:
```sql
CREATE TABLE OperatorMaster (
    OperatorID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    LoginID NVARCHAR(50) NOT NULL,
    OperatorName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'General',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    CONSTRAINT FK_Operator_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID),
    CONSTRAINT UK_Operator_Login UNIQUE (ProjectID, LoginID)
);
```

**Role値**:
- `General`: 一般オペレーター
- `Admin`: 管理者

---

### 2.3 カテゴリーマスタ（CategoryMaster）

**用途**: 問合せのカテゴリー管理（案件ごとに異なる）

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| CategoryID | INTEGER | NOT NULL | PK | カテゴリーID（自動採番） |
| ProjectID | INTEGER | NOT NULL | FK | 案件ID |
| CategoryName | NVARCHAR(50) | NOT NULL | - | カテゴリー名 |
| DisplayOrder | INTEGER | NOT NULL | - | 表示順 |
| IsActive | BIT | NOT NULL | - | 有効/無効（デフォルト: 1） |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |

**制約**:
- PRIMARY KEY: CategoryID
- FOREIGN KEY: ProjectID → ProjectMaster(ProjectID)

**SQL Server用 CREATE文**:
```sql
CREATE TABLE CategoryMaster (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    CategoryName NVARCHAR(50) NOT NULL,
    DisplayOrder INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Category_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID)
);
```

**初期データ例**:
- 料金に関する問合せ
- 契約内容変更
- 解約
- クレーム
- その他

---

### 2.4 ステータスマスタ（StatusMaster）

**用途**: 対応状況の管理（案件ごとに異なる）

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| StatusID | INTEGER | NOT NULL | PK | ステータスID（自動採番） |
| ProjectID | INTEGER | NOT NULL | FK | 案件ID |
| StatusName | NVARCHAR(50) | NOT NULL | - | ステータス名 |
| DisplayOrder | INTEGER | NOT NULL | - | 表示順 |
| IsActive | BIT | NOT NULL | - | 有効/無効（デフォルト: 1） |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |

**制約**:
- PRIMARY KEY: StatusID
- FOREIGN KEY: ProjectID → ProjectMaster(ProjectID)

**SQL Server用 CREATE文**:
```sql
CREATE TABLE StatusMaster (
    StatusID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    StatusName NVARCHAR(50) NOT NULL,
    DisplayOrder INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Status_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID)
);
```

**初期データ例**:
- 対応完了
- 対応中
- 折り返し予定
- 保留

---

### 2.5 カスタム項目定義（CustomFieldDefinition）

**用途**: 問合せ履歴のカスタム項目の定義

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| FieldID | INTEGER | NOT NULL | PK | フィールドID（自動採番） |
| ProjectID | INTEGER | NOT NULL | FK | 案件ID |
| ColumnNumber | INTEGER | NOT NULL | - | カラム番号（1~10） |
| FieldName | NVARCHAR(50) | NOT NULL | - | 項目名（内部用） |
| DisplayName | NVARCHAR(100) | NOT NULL | - | 表示名 |
| FieldType | NVARCHAR(20) | NOT NULL | - | 項目タイプ（Text/Number/Date/Select） |
| IsRequired | BIT | NOT NULL | - | 必須（デフォルト: 0） |
| IsEnabled | BIT | NOT NULL | - | 使用する（デフォルト: 1） |
| DisplayOrder | INTEGER | NOT NULL | - | 表示順 |
| SelectOptions | NVARCHAR(MAX) | NULL | - | 選択肢（JSON形式） |
| IsSearchable | BIT | NOT NULL | - | 検索可能（デフォルト: 0） |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |

**制約**:
- PRIMARY KEY: FieldID
- FOREIGN KEY: ProjectID → ProjectMaster(ProjectID)
- UNIQUE: (ProjectID, ColumnNumber)

**SQL Server用 CREATE文**:
```sql
CREATE TABLE CustomFieldDefinition (
    FieldID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    ColumnNumber INT NOT NULL,
    FieldName NVARCHAR(50) NOT NULL,
    DisplayName NVARCHAR(100) NOT NULL,
    FieldType NVARCHAR(20) NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 0,
    IsEnabled BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL,
    SelectOptions NVARCHAR(MAX),
    IsSearchable BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_CustomField_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID),
    CONSTRAINT UK_CustomField_Column UNIQUE (ProjectID, ColumnNumber),
    CONSTRAINT CK_ColumnNumber CHECK (ColumnNumber BETWEEN 1 AND 10)
);
```

**FieldType値**:
- `Text`: テキスト
- `Number`: 数値
- `Date`: 日付
- `Select`: 選択肢

**SelectOptions例**（JSON形式）:
```json
["低", "中", "高"]
```

---

### 2.6 顧客マスタ表示設定グループ（CustomerMasterDisplayConfig）

**用途**: 顧客情報のグループ（タブ）定義

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| ConfigID | INTEGER | NOT NULL | PK | 設定ID（自動採番） |
| ProjectID | INTEGER | NOT NULL | FK | 案件ID |
| GroupID | INTEGER | NOT NULL | - | グループID（1~5） |
| GroupName | NVARCHAR(50) | NOT NULL | - | グループ名（例: 基本情報） |
| DisplayOrder | INTEGER | NOT NULL | - | 表示順 |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |

**制約**:
- PRIMARY KEY: ConfigID
- FOREIGN KEY: ProjectID → ProjectMaster(ProjectID)
- UNIQUE: (ProjectID, GroupID)

**SQL Server用 CREATE文**:
```sql
CREATE TABLE CustomerMasterDisplayConfig (
    ConfigID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    GroupID INT NOT NULL,
    GroupName NVARCHAR(50) NOT NULL,
    DisplayOrder INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_DisplayConfig_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID),
    CONSTRAINT UK_DisplayConfig_Group UNIQUE (ProjectID, GroupID),
    CONSTRAINT CK_GroupID CHECK (GroupID BETWEEN 1 AND 5)
);
```

---

### 2.7 顧客マスタ項目設定（CustomerMasterColumnConfig）

**用途**: 顧客情報の表示項目定義

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| ColumnConfigID | INTEGER | NOT NULL | PK | 項目設定ID（自動採番） |
| ConfigID | INTEGER | NOT NULL | FK | 設定ID |
| ColumnName | NVARCHAR(50) | NOT NULL | - | DBのカラム名 |
| DisplayName | NVARCHAR(100) | NOT NULL | - | 表示名 |
| DisplayOrder | INTEGER | NOT NULL | - | グループ内の表示順 |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |

**制約**:
- PRIMARY KEY: ColumnConfigID
- FOREIGN KEY: ConfigID → CustomerMasterDisplayConfig(ConfigID)

**SQL Server用 CREATE文**:
```sql
CREATE TABLE CustomerMasterColumnConfig (
    ColumnConfigID INT IDENTITY(1,1) PRIMARY KEY,
    ConfigID INT NOT NULL,
    ColumnName NVARCHAR(50) NOT NULL,
    DisplayName NVARCHAR(100) NOT NULL,
    DisplayOrder INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_ColumnConfig_DisplayConfig FOREIGN KEY (ConfigID) 
        REFERENCES CustomerMasterDisplayConfig(ConfigID) ON DELETE CASCADE
);
```

---

### 2.8 履歴保存設定（HistoryLogSettings）

**用途**: 問合せ履歴の更新履歴保存の有効/無効設定

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| SettingID | INTEGER | NOT NULL | PK | 設定ID（自動採番） |
| ProjectID | INTEGER | NOT NULL | FK | 案件ID |
| EnableHistoryLog | BIT | NOT NULL | - | 履歴保存有効（デフォルト: 0） |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |
| UpdatedAt | DATETIME | NULL | - | 更新日時 |

**制約**:
- PRIMARY KEY: SettingID
- FOREIGN KEY: ProjectID → ProjectMaster(ProjectID)
- UNIQUE: ProjectID

**SQL Server用 CREATE文**:
```sql
CREATE TABLE HistoryLogSettings (
    SettingID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    EnableHistoryLog BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    CONSTRAINT FK_HistoryLogSettings_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID),
    CONSTRAINT UK_HistoryLogSettings_Project UNIQUE (ProjectID)
);
```

---

## 3. グループ③: 問合せ履歴DB

### 3.1 問合せ履歴（InquiryHistory）

**用途**: 受電記録の管理

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| InquiryID | INTEGER | NOT NULL | PK | 問合せID（自動採番） |
| ProjectID | INTEGER | NOT NULL | - | 案件ID |
| CustomerKey | NVARCHAR(50) | NULL | - | 顧客マスタのキー |
| OperatorID | INTEGER | NOT NULL | - | オペレーターID |
| CategoryID | INTEGER | NULL | - | カテゴリーID |
| StatusID | INTEGER | NULL | - | ステータスID |
| InquiryContent | NVARCHAR(MAX) | NOT NULL | - | 問合せ内容 |
| ResponseContent | NVARCHAR(MAX) | NULL | - | 対応内容 |
| FirstReceivedDateTime | DATETIME | NOT NULL | - | 初回受電日時 |
| UpdatedDateTime | DATETIME | NULL | - | 更新日時 |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |
| CreatedBy | INTEGER | NOT NULL | - | 作成者（OperatorID） |
| UpdatedBy | INTEGER | NULL | - | 最終更新者（OperatorID） |
| CustomCol01 | NVARCHAR(500) | NULL | - | カスタム項目1 |
| CustomCol02 | NVARCHAR(500) | NULL | - | カスタム項目2 |
| CustomCol03 | NVARCHAR(500) | NULL | - | カスタム項目3 |
| CustomCol04 | NVARCHAR(500) | NULL | - | カスタム項目4 |
| CustomCol05 | NVARCHAR(500) | NULL | - | カスタム項目5 |
| CustomCol06 | NVARCHAR(500) | NULL | - | カスタム項目6 |
| CustomCol07 | NVARCHAR(500) | NULL | - | カスタム項目7 |
| CustomCol08 | NVARCHAR(500) | NULL | - | カスタム項目8 |
| CustomCol09 | NVARCHAR(500) | NULL | - | カスタム項目9 |
| CustomCol10 | NVARCHAR(500) | NULL | - | カスタム項目10 |

**制約**:
- PRIMARY KEY: InquiryID

**SQL Server用 CREATE文**:
```sql
CREATE TABLE InquiryHistory (
    InquiryID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    CustomerKey NVARCHAR(50),
    OperatorID INT NOT NULL,
    CategoryID INT,
    StatusID INT,
    InquiryContent NVARCHAR(MAX) NOT NULL,
    ResponseContent NVARCHAR(MAX),
    FirstReceivedDateTime DATETIME NOT NULL,
    UpdatedDateTime DATETIME,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    UpdatedBy INT,
    CustomCol01 NVARCHAR(500),
    CustomCol02 NVARCHAR(500),
    CustomCol03 NVARCHAR(500),
    CustomCol04 NVARCHAR(500),
    CustomCol05 NVARCHAR(500),
    CustomCol06 NVARCHAR(500),
    CustomCol07 NVARCHAR(500),
    CustomCol08 NVARCHAR(500),
    CustomCol09 NVARCHAR(500),
    CustomCol10 NVARCHAR(500)
);
```

**Access用**: MAXをMEMOに変更

---

### 3.2 問合せ履歴更新履歴（InquiryHistoryLog）

**用途**: 問合せ履歴の更新前データを退避（設定により有効化）

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| LogID | INTEGER | NOT NULL | PK | ログID（自動採番） |
| InquiryID | INTEGER | NOT NULL | FK | 問合せID |
| ProjectID | INTEGER | NOT NULL | - | 案件ID |
| CustomerKey | NVARCHAR(50) | NULL | - | 顧客マスタのキー |
| OperatorID | INTEGER | NOT NULL | - | オペレーターID |
| CategoryID | INTEGER | NULL | - | カテゴリーID |
| StatusID | INTEGER | NULL | - | ステータスID |
| InquiryContent | NVARCHAR(MAX) | NULL | - | 問合せ内容 |
| ResponseContent | NVARCHAR(MAX) | NULL | - | 対応内容 |
| FirstReceivedDateTime | DATETIME | NULL | - | 初回受電日時 |
| UpdatedDateTime | DATETIME | NULL | - | 更新日時 |
| CustomCol01 ~ 10 | NVARCHAR(500) | NULL | - | カスタム項目1~10 |
| UpdatedBy | INTEGER | NOT NULL | - | 更新者（OperatorID） |
| LoggedAt | DATETIME | NOT NULL | - | ログ記録日時 |

**制約**:
- PRIMARY KEY: LogID
- FOREIGN KEY: InquiryID → InquiryHistory(InquiryID)

**SQL Server用 CREATE文**:
```sql
CREATE TABLE InquiryHistoryLog (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    InquiryID INT NOT NULL,
    ProjectID INT NOT NULL,
    CustomerKey NVARCHAR(50),
    OperatorID INT NOT NULL,
    CategoryID INT,
    StatusID INT,
    InquiryContent NVARCHAR(MAX),
    ResponseContent NVARCHAR(MAX),
    FirstReceivedDateTime DATETIME,
    UpdatedDateTime DATETIME,
    CustomCol01 NVARCHAR(500),
    CustomCol02 NVARCHAR(500),
    CustomCol03 NVARCHAR(500),
    CustomCol04 NVARCHAR(500),
    CustomCol05 NVARCHAR(500),
    CustomCol06 NVARCHAR(500),
    CustomCol07 NVARCHAR(500),
    CustomCol08 NVARCHAR(500),
    CustomCol09 NVARCHAR(500),
    CustomCol10 NVARCHAR(500),
    UpdatedBy INT NOT NULL,
    LoggedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_InquiryLog_Inquiry FOREIGN KEY (InquiryID) 
        REFERENCES InquiryHistory(InquiryID) ON DELETE CASCADE
);
```

---

### 3.3 顧客メモ（CustomerMemo）

**用途**: 顧客ごとのメモ（管理者のみ編集可）

| カラム名 | データ型 | NULL | キー | 説明 |
|---------|---------|------|------|------|
| MemoID | INTEGER | NOT NULL | PK | メモID（自動採番） |
| ProjectID | INTEGER | NOT NULL | - | 案件ID |
| CustomerKey | NVARCHAR(50) | NOT NULL | - | 顧客マスタのキー |
| MemoContent | NVARCHAR(MAX) | NOT NULL | - | メモ内容 |
| CreatedBy | INTEGER | NOT NULL | - | 作成者（OperatorID） |
| CreatedAt | DATETIME | NOT NULL | - | 作成日時 |
| UpdatedBy | INTEGER | NULL | - | 更新者（OperatorID） |
| UpdatedAt | DATETIME | NULL | - | 更新日時 |

**制約**:
- PRIMARY KEY: MemoID
- UNIQUE: (ProjectID, CustomerKey)

**SQL Server用 CREATE文**:
```sql
CREATE TABLE CustomerMemo (
    MemoID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    CustomerKey NVARCHAR(50) NOT NULL,
    MemoContent NVARCHAR(MAX) NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedBy INT,
    UpdatedAt DATETIME,
    CONSTRAINT UK_CustomerMemo UNIQUE (ProjectID, CustomerKey)
);
```

---

## 4. グループ①: 顧客マスタ・画像DB

### 4.1 顧客マスタテーブル（案件固有）

**用途**: 既存の顧客データベース（読み取り専用）

**注意事項**:
- テーブル名、カラム名は案件ごとに異なる
- config.jsonまたはCustomerMasterColumnConfigで設定
- アプリケーションからは参照のみ

**必須要件**:
- ユニークキーとなるカラムが存在すること
- そのカラム名をconfig.jsonで指定すること

**例**:
```sql
-- 案件Aの顧客マスタ
CREATE TABLE T_Customer (
    CustomerID NVARCHAR(50) PRIMARY KEY,
    customer_name NVARCHAR(100),
    kana NVARCHAR(100),
    tel_no NVARCHAR(20),
    email NVARCHAR(100),
    postal_code NVARCHAR(10),
    addr1 NVARCHAR(200),
    addr2 NVARCHAR(200),
    contract_date DATE,
    contract_no NVARCHAR(50),
    plan_name NVARCHAR(50),
    member_rank NVARCHAR(20),
    remarks NVARCHAR(MAX)
);
```

---

### 4.2 画像情報テーブル（オプション）

**用途**: 顧客に関連する画像ファイルのパス情報

**注意事項**:
- テーブル名、カラム名は案件ごとに異なる
- config.jsonで設定

**必須カラム**:
- 顧客キーカラム
- ファイルパスカラム

**例**:
```sql
CREATE TABLE T_CustomerImages (
    ImageID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID NVARCHAR(50) NOT NULL,
    ImagePath NVARCHAR(500) NOT NULL,
    DisplayOrder INT,
    CreatedAt DATETIME DEFAULT GETDATE()
);
```

---

## 5. ER図

### 5.1 設定・マスタDB ER図

```
ProjectMaster (案件マスタ)
    ↓ 1:N
┌───────────────────┐
│OperatorMaster     │
│CategoryMaster     │
│StatusMaster       │
│CustomFieldDefinition│
│DisplayConfig      │
│HistoryLogSettings │
└───────────────────┘

CustomerMasterDisplayConfig
    ↓ 1:N
CustomerMasterColumnConfig
```

### 5.2 問合せ履歴DB ER図

```
InquiryHistory (問合せ履歴)
    ↓ 1:N
InquiryHistoryLog (更新履歴)

CustomerMemo (顧客メモ) ※独立
```

---

## 6. インデックス設計

### 6.1 InquiryHistory

```sql
-- 顧客キーでの検索用
CREATE INDEX IX_InquiryHistory_CustomerKey 
ON InquiryHistory(CustomerKey);

-- 受電日時での検索用（範囲検索）
CREATE INDEX IX_InquiryHistory_ReceivedDateTime 
ON InquiryHistory(FirstReceivedDateTime DESC);

-- カテゴリーでの検索用
CREATE INDEX IX_InquiryHistory_Category 
ON InquiryHistory(CategoryID);

-- ステータスでの検索用
CREATE INDEX IX_InquiryHistory_Status 
ON InquiryHistory(StatusID);

-- オペレーターでの検索用
CREATE INDEX IX_InquiryHistory_Operator 
ON InquiryHistory(OperatorID);

-- 案件IDとの複合（複数案件を1DBで管理する場合）
CREATE INDEX IX_InquiryHistory_Project 
ON InquiryHistory(ProjectID, FirstReceivedDateTime DESC);
```

### 6.2 CustomerMemo

```sql
-- 顧客キーでの検索用
CREATE INDEX IX_CustomerMemo_CustomerKey 
ON CustomerMemo(CustomerKey);
```

---

## 7. データ型定義

### 7.1 SQL Server と Access の対応表

| 用途 | SQL Server | Access |
|------|-----------|--------|
| 整数（自動採番） | INT IDENTITY(1,1) | AUTOINCREMENT |
| 整数 | INT | INTEGER / LONG |
| 短いテキスト | NVARCHAR(N) | TEXT(N) |
| 長いテキスト | NVARCHAR(MAX) | MEMO |
| 日付時刻 | DATETIME | DATETIME |
| 真偽値 | BIT | YESNO |

---

## 8. 初期データ

### 8.1 案件マスタ

```sql
INSERT INTO ProjectMaster (ProjectCode, ProjectName, Description, IsActive, CreatedAt)
VALUES ('ProjectA', '案件A - サポートセンター', '〇〇サポートセンター業務', 1, GETDATE());
```

### 8.2 オペレーターマスタ

```sql
-- 管理者アカウント（パスワード: admin123 のハッシュ）
INSERT INTO OperatorMaster (ProjectID, LoginID, OperatorName, PasswordHash, Role, IsActive, CreatedAt)
VALUES (1, 'admin', '管理者', '$2a$10$N9qo8uLOickgx2ZMRZoMye.KH5xG7Z3uJBcLBzTZZcO8GxGHmz3Ty', 'Admin', 1, GETDATE());

-- 一般オペレーター（パスワード: operator123 のハッシュ）
INSERT INTO OperatorMaster (ProjectID, LoginID, OperatorName, PasswordHash, Role, IsActive, CreatedAt)
VALUES (1, 'operator001', '山田 太郎', '$2a$10$xH7w6.Y.dLLZGkLfGsC6eOxR1mHZB1F.wR8gK3pYiKZ5fN9tZ8pSC', 'General', 1, GETDATE());
```

**注意**: 上記のPasswordHashは例です。実際にはBCryptライブラリでハッシュ化してください。

### 8.3 カテゴリーマスタ

```sql
INSERT INTO CategoryMaster (ProjectID, CategoryName, DisplayOrder, IsActive, CreatedAt)
VALUES 
    (1, '料金に関する問合せ', 1, 1, GETDATE()),
    (1, '契約内容変更', 2, 1, GETDATE()),
    (1, '解約', 3, 1, GETDATE()),
    (1, 'クレーム', 4, 1, GETDATE()),
    (1, 'その他', 5, 1, GETDATE());
```

### 8.4 ステータスマスタ

```sql
INSERT INTO StatusMaster (ProjectID, StatusName, DisplayOrder, IsActive, CreatedAt)
VALUES 
    (1, '対応完了', 1, 1, GETDATE()),
    (1, '対応中', 2, 1, GETDATE()),
    (1, '折り返し予定', 3, 1, GETDATE()),
    (1, '保留', 4, 1, GETDATE());
```

### 8.5 カスタム項目定義

```sql
INSERT INTO CustomFieldDefinition (ProjectID, ColumnNumber, FieldName, DisplayName, FieldType, IsRequired, IsEnabled, DisplayOrder, SelectOptions, IsSearchable, CreatedAt)
VALUES 
    (1, 1, 'contract_no', '契約番号', 'Text', 0, 1, 1, NULL, 1, GETDATE()),
    (1, 2, 'deadline', '対応期限', 'Date', 0, 1, 2, NULL, 0, GETDATE()),
    (1, 3, 'priority', '優先度', 'Select', 0, 1, 3, '["低","中","高"]', 0, GETDATE());
```

### 8.6 顧客マスタ表示設定

```sql
-- グループ1: 基本情報
INSERT INTO CustomerMasterDisplayConfig (ProjectID, GroupID, GroupName, DisplayOrder, CreatedAt)
VALUES (1, 1, '基本情報', 1, GETDATE());

INSERT INTO CustomerMasterColumnConfig (ConfigID, ColumnName, DisplayName, DisplayOrder, CreatedAt)
VALUES 
    (1, 'CustomerID', '顧客ID', 1, GETDATE()),
    (1, 'customer_name', '顧客名', 2, GETDATE()),
    (1, 'kana', 'フリガナ', 3, GETDATE()),
    (1, 'tel_no', '電話番号', 4, GETDATE()),
    (1, 'email', 'メールアドレス', 5, GETDATE());

-- グループ2: 契約情報
INSERT INTO CustomerMasterDisplayConfig (ProjectID, GroupID, GroupName, DisplayOrder, CreatedAt)
VALUES (1, 2, '契約情報', 2, GETDATE());

INSERT INTO CustomerMasterColumnConfig (ConfigID, ColumnName, DisplayName, DisplayOrder, CreatedAt)
VALUES 
    (2, 'contract_date', '契約日', 1, GETDATE()),
    (2, 'contract_no', '契約番号', 2, GETDATE()),
    (2, 'plan_name', '契約プラン', 3, GETDATE());

-- グループ3: 住所
INSERT INTO CustomerMasterDisplayConfig (ProjectID, GroupID, GroupName, DisplayOrder, CreatedAt)
VALUES (1, 3, '住所', 3, GETDATE());

INSERT INTO CustomerMasterColumnConfig (ConfigID, ColumnName, DisplayName, DisplayOrder, CreatedAt)
VALUES 
    (3, 'postal_code', '郵便番号', 1, GETDATE()),
    (3, 'addr1', '住所1', 2, GETDATE()),
    (3, 'addr2', '住所2', 3, GETDATE());

-- グループ4: その他
INSERT INTO CustomerMasterDisplayConfig (ProjectID, GroupID, GroupName, DisplayOrder, CreatedAt)
VALUES (1, 4, 'その他', 4, GETDATE());

INSERT INTO CustomerMasterColumnConfig (ConfigID, ColumnName, DisplayName, DisplayOrder, CreatedAt)
VALUES 
    (4, 'member_rank', '会員ランク', 1, GETDATE()),
    (4, 'remarks', '備考', 2, GETDATE());

-- グループ5: 画像・メモ（特殊グループ、項目定義なし）
INSERT INTO CustomerMasterDisplayConfig (ProjectID, GroupID, GroupName, DisplayOrder, CreatedAt)
VALUES (1, 5, '画像・メモ', 5, GETDATE());
```

### 8.7 履歴保存設定

```sql
INSERT INTO HistoryLogSettings (ProjectID, EnableHistoryLog, CreatedAt)
VALUES (1, 1, GETDATE());  -- 履歴保存を有効化
```

---

## 9. データベース初期化スクリプト

### 9.1 SQL Server用 完全初期化スクリプト

```sql
-- ============================================
-- コール業務管理システム
-- データベース初期化スクリプト (SQL Server)
-- ============================================

USE master;
GO

-- データベース削除（既存の場合）
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'CallCenter_ProjectA_Settings')
BEGIN
    ALTER DATABASE CallCenter_ProjectA_Settings SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CallCenter_ProjectA_Settings;
END
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'CallCenter_ProjectA_Inquiry')
BEGIN
    ALTER DATABASE CallCenter_ProjectA_Inquiry SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CallCenter_ProjectA_Inquiry;
END
GO

-- データベース作成
CREATE DATABASE CallCenter_ProjectA_Settings;
GO

CREATE DATABASE CallCenter_ProjectA_Inquiry;
GO

-- ============================================
-- 設定・マスタDB
-- ============================================
USE CallCenter_ProjectA_Settings;
GO

-- 案件マスタ
CREATE TABLE ProjectMaster (
    ProjectID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectCode NVARCHAR(50) NOT NULL UNIQUE,
    ProjectName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME
);

-- オペレーターマスタ
CREATE TABLE OperatorMaster (
    OperatorID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    LoginID NVARCHAR(50) NOT NULL,
    OperatorName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'General',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    CONSTRAINT FK_Operator_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID),
    CONSTRAINT UK_Operator_Login UNIQUE (ProjectID, LoginID)
);

-- カテゴリーマスタ
CREATE TABLE CategoryMaster (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    CategoryName NVARCHAR(50) NOT NULL,
    DisplayOrder INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Category_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID)
);

-- ステータスマスタ
CREATE TABLE StatusMaster (
    StatusID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    StatusName NVARCHAR(50) NOT NULL,
    DisplayOrder INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Status_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID)
);

-- カスタム項目定義
CREATE TABLE CustomFieldDefinition (
    FieldID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    ColumnNumber INT NOT NULL,
    FieldName NVARCHAR(50) NOT NULL,
    DisplayName NVARCHAR(100) NOT NULL,
    FieldType NVARCHAR(20) NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 0,
    IsEnabled BIT NOT NULL DEFAULT 1,
    DisplayOrder INT NOT NULL,
    SelectOptions NVARCHAR(MAX),
    IsSearchable BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_CustomField_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID),
    CONSTRAINT UK_CustomField_Column UNIQUE (ProjectID, ColumnNumber),
    CONSTRAINT CK_ColumnNumber CHECK (ColumnNumber BETWEEN 1 AND 10)
);

-- 顧客マスタ表示設定グループ
CREATE TABLE CustomerMasterDisplayConfig (
    ConfigID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    GroupID INT NOT NULL,
    GroupName NVARCHAR(50) NOT NULL,
    DisplayOrder INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_DisplayConfig_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID),
    CONSTRAINT UK_DisplayConfig_Group UNIQUE (ProjectID, GroupID),
    CONSTRAINT CK_GroupID CHECK (GroupID BETWEEN 1 AND 5)
);

-- 顧客マスタ項目設定
CREATE TABLE CustomerMasterColumnConfig (
    ColumnConfigID INT IDENTITY(1,1) PRIMARY KEY,
    ConfigID INT NOT NULL,
    ColumnName NVARCHAR(50) NOT NULL,
    DisplayName NVARCHAR(100) NOT NULL,
    DisplayOrder INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_ColumnConfig_DisplayConfig FOREIGN KEY (ConfigID) 
        REFERENCES CustomerMasterDisplayConfig(ConfigID) ON DELETE CASCADE
);

-- 履歴保存設定
CREATE TABLE HistoryLogSettings (
    SettingID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    EnableHistoryLog BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    CONSTRAINT FK_HistoryLogSettings_Project FOREIGN KEY (ProjectID) 
        REFERENCES ProjectMaster(ProjectID),
    CONSTRAINT UK_HistoryLogSettings_Project UNIQUE (ProjectID)
);

-- ============================================
-- 問合せ履歴DB
-- ============================================
USE CallCenter_ProjectA_Inquiry;
GO

-- 問合せ履歴
CREATE TABLE InquiryHistory (
    InquiryID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    CustomerKey NVARCHAR(50),
    OperatorID INT NOT NULL,
    CategoryID INT,
    StatusID INT,
    InquiryContent NVARCHAR(MAX) NOT NULL,
    ResponseContent NVARCHAR(MAX),
    FirstReceivedDateTime DATETIME NOT NULL,
    UpdatedDateTime DATETIME,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    UpdatedBy INT,
    CustomCol01 NVARCHAR(500),
    CustomCol02 NVARCHAR(500),
    CustomCol03 NVARCHAR(500),
    CustomCol04 NVARCHAR(500),
    CustomCol05 NVARCHAR(500),
    CustomCol06 NVARCHAR(500),
    CustomCol07 NVARCHAR(500),
    CustomCol08 NVARCHAR(500),
    CustomCol09 NVARCHAR(500),
    CustomCol10 NVARCHAR(500)
);

-- インデックス
CREATE INDEX IX_InquiryHistory_CustomerKey ON InquiryHistory(CustomerKey);
CREATE INDEX IX_InquiryHistory_ReceivedDateTime ON InquiryHistory(FirstReceivedDateTime DESC);
CREATE INDEX IX_InquiryHistory_Category ON InquiryHistory(CategoryID);
CREATE INDEX IX_InquiryHistory_Status ON InquiryHistory(StatusID);
CREATE INDEX IX_InquiryHistory_Operator ON InquiryHistory(OperatorID);
CREATE INDEX IX_InquiryHistory_Project ON InquiryHistory(ProjectID, FirstReceivedDateTime DESC);

-- 問合せ履歴更新履歴
CREATE TABLE InquiryHistoryLog (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    InquiryID INT NOT NULL,
    ProjectID INT NOT NULL,
    CustomerKey NVARCHAR(50),
    OperatorID INT NOT NULL,
    CategoryID INT,
    StatusID INT,
    InquiryContent NVARCHAR(MAX),
    ResponseContent NVARCHAR(MAX),
    FirstReceivedDateTime DATETIME,
    UpdatedDateTime DATETIME,
    CustomCol01 NVARCHAR(500),
    CustomCol02 NVARCHAR(500),
    CustomCol03 NVARCHAR(500),
    CustomCol04 NVARCHAR(500),
    CustomCol05 NVARCHAR(500),
    CustomCol06 NVARCHAR(500),
    CustomCol07 NVARCHAR(500),
    CustomCol08 NVARCHAR(500),
    CustomCol09 NVARCHAR(500),
    CustomCol10 NVARCHAR(500),
    UpdatedBy INT NOT NULL,
    LoggedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_InquiryLog_Inquiry FOREIGN KEY (InquiryID) 
        REFERENCES InquiryHistory(InquiryID) ON DELETE CASCADE
);

-- 顧客メモ
CREATE TABLE CustomerMemo (
    MemoID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    CustomerKey NVARCHAR(50) NOT NULL,
    MemoContent NVARCHAR(MAX) NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedBy INT,
    UpdatedAt DATETIME,
    CONSTRAINT UK_CustomerMemo UNIQUE (ProjectID, CustomerKey)
);

CREATE INDEX IX_CustomerMemo_CustomerKey ON CustomerMemo(CustomerKey);

PRINT '初期化完了: テーブル作成完了';
GO
```

### 9.2 Access用 テーブル作成スクリプト

**注意**: Accessは個別にテーブルを作成する必要があります。以下は参考例です。

```sql
-- 案件マスタ
CREATE TABLE ProjectMaster (
    ProjectID AUTOINCREMENT PRIMARY KEY,
    ProjectCode TEXT(50) NOT NULL,
    ProjectName TEXT(100) NOT NULL,
    Description TEXT(500),
    IsActive YESNO NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME,
    CONSTRAINT UK_ProjectCode UNIQUE (ProjectCode)
);

-- オペレーターマスタ
CREATE TABLE OperatorMaster (
    OperatorID AUTOINCREMENT PRIMARY KEY,
    ProjectID LONG NOT NULL,
    LoginID TEXT(50) NOT NULL,
    OperatorName TEXT(100) NOT NULL,
    PasswordHash TEXT(255) NOT NULL,
    Role TEXT(20) NOT NULL,
    IsActive YESNO NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME
);

-- 以下、同様に各テーブルを作成
-- ※MAXはMEMOに、INTはLONGに変更
```

---

## 10. データ移行・メンテナンス

### 10.1 既存データからの移行

**顧客マスタが既存DBにある場合**:
- そのまま参照するため移行不要
- config.jsonで接続情報を設定

**問合せ履歴を移行する場合**:
```sql
-- 旧システムからのデータ移行例
INSERT INTO InquiryHistory (
    ProjectID, CustomerKey, OperatorID, CategoryID, StatusID,
    InquiryContent, ResponseContent, FirstReceivedDateTime,
    CreatedAt, CreatedBy
)
SELECT 
    1,  -- ProjectID
    旧CustomerKey,
    旧OperatorID,
    旧CategoryID,
    旧StatusID,
    旧InquiryContent,
    旧ResponseContent,
    旧ReceivedDateTime,
    旧CreatedAt,
    旧CreatedBy
FROM 旧InquiryTable;
```

### 10.2 定期メンテナンス

**インデックス再構築（SQL Server）**:
```sql
-- 問合せ履歴テーブルのインデックス再構築
ALTER INDEX ALL ON InquiryHistory REBUILD;
```

**統計情報更新（SQL Server）**:
```sql
UPDATE STATISTICS InquiryHistory;
```

**古いログデータのアーカイブ**:
```sql
-- 1年以上前のログデータを別テーブルに移動
INSERT INTO InquiryHistoryLog_Archive
SELECT * FROM InquiryHistoryLog
WHERE LoggedAt < DATEADD(YEAR, -1, GETDATE());

DELETE FROM InquiryHistoryLog
WHERE LoggedAt < DATEADD(YEAR, -1, GETDATE());
```

---

## 11. バックアップ・リストア

### 11.1 SQL Server バックアップ

```sql
-- 完全バックアップ
BACKUP DATABASE CallCenter_ProjectA_Settings
TO DISK = 'C:\Backup\CallCenter_ProjectA_Settings.bak'
WITH FORMAT, NAME = 'Full Backup';

BACKUP DATABASE CallCenter_ProjectA_Inquiry
TO DISK = 'C:\Backup\CallCenter_ProjectA_Inquiry.bak'
WITH FORMAT, NAME = 'Full Backup';
```

### 11.2 SQL Server リストア

```sql
-- リストア
RESTORE DATABASE CallCenter_ProjectA_Settings
FROM DISK = 'C:\Backup\CallCenter_ProjectA_Settings.bak'
WITH REPLACE;

RESTORE DATABASE CallCenter_ProjectA_Inquiry
FROM DISK = 'C:\Backup\CallCenter_ProjectA_Inquiry.bak'
WITH REPLACE;
```

### 11.3 Access バックアップ

- Accessファイル（.accdb）を直接コピー
- 推奨: 日次または週次でファイルコピー

```
例:
CallCenter_Settings.accdb → CallCenter_Settings_20250112.accdb
CallCenter_Inquiry.accdb → CallCenter_Inquiry_20250112.accdb
```

---

## 12. パフォーマンスチューニング

### 12.1 クエリ最適化

**頻繁に使用される検索クエリ**:
```sql
-- 顧客の問合せ履歴検索（インデックス使用）
SELECT * FROM InquiryHistory
WHERE CustomerKey = @CustomerKey
ORDER BY FirstReceivedDateTime DESC;

-- 期間指定での検索（インデックス使用）
SELECT * FROM InquiryHistory
WHERE FirstReceivedDateTime BETWEEN @StartDate AND @EndDate
ORDER BY FirstReceivedDateTime DESC;
```

### 12.2 統計情報の更新（SQL Server）

```sql
-- 自動統計更新を有効化
ALTER DATABASE CallCenter_ProjectA_Inquiry
SET AUTO_UPDATE_STATISTICS ON;
```

---

## 13. トラブルシューティング

### 13.1 よくある問題

**問題1: Accessで2GB制限に達した**
- 解決策: 問合せ履歴を別ファイルに分離、または SQL Server に移行

**問題2: 同時接続エラー（Access）**
- 解決策: 接続数を減らす、または SQL Server に移行

**問題3: インデックスの肥大化**
- 解決策: インデックス再構築を実行

### 13.2 データ整合性チェック

```sql
-- 孤立した問合せ履歴のチェック
SELECT COUNT(*) FROM InquiryHistory
WHERE CustomerKey IS NOT NULL
AND CustomerKey NOT IN (SELECT CustomerID FROM [顧客マスタDB].dbo.T_Customer);

-- 未使用のカスタム項目定義のチェック
SELECT * FROM CustomFieldDefinition
WHERE IsEnabled = 0;
```

---

## 改訂履歴

| 版数 | 日付 | 改訂内容 | 作成者 |
|------|------|----------|--------|
| 1.0 | 2025/01/12 | 初版作成 | - |

---
