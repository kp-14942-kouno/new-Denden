-- ============================================
-- コール業務管理システム
-- 設定・マスタDB 初期化スクリプト (SQLite)
-- ============================================

-- 案件マスタ
CREATE TABLE IF NOT EXISTS ProjectMaster (
    ProjectID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectCode TEXT NOT NULL UNIQUE,
    ProjectName TEXT NOT NULL,
    Description TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT
);

-- オペレーターマスタ
CREATE TABLE IF NOT EXISTS OperatorMaster (
    OperatorID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectID INTEGER NOT NULL,
    LoginID TEXT NOT NULL,
    OperatorName TEXT NOT NULL,
    PasswordHash TEXT NOT NULL,
    Role TEXT NOT NULL DEFAULT 'General',
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT,
    FOREIGN KEY (ProjectID) REFERENCES ProjectMaster(ProjectID),
    UNIQUE (ProjectID, LoginID)
);

-- カテゴリーマスタ
CREATE TABLE IF NOT EXISTS CategoryMaster (
    CategoryID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectID INTEGER NOT NULL,
    CategoryName TEXT NOT NULL,
    DisplayOrder INTEGER NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (ProjectID) REFERENCES ProjectMaster(ProjectID)
);

-- ステータスマスタ
CREATE TABLE IF NOT EXISTS StatusMaster (
    StatusID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectID INTEGER NOT NULL,
    StatusName TEXT NOT NULL,
    DisplayOrder INTEGER NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (ProjectID) REFERENCES ProjectMaster(ProjectID)
);

-- カスタム項目定義
CREATE TABLE IF NOT EXISTS CustomFieldDefinition (
    FieldID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectID INTEGER NOT NULL,
    ColumnNumber INTEGER NOT NULL,
    FieldName TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    FieldType TEXT NOT NULL,
    IsRequired INTEGER NOT NULL DEFAULT 0,
    IsEnabled INTEGER NOT NULL DEFAULT 1,
    DisplayOrder INTEGER NOT NULL,
    SelectOptions TEXT,
    IsSearchable INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (ProjectID) REFERENCES ProjectMaster(ProjectID),
    UNIQUE (ProjectID, ColumnNumber),
    CHECK (ColumnNumber BETWEEN 1 AND 10)
);

-- 顧客マスタ表示設定グループ
CREATE TABLE IF NOT EXISTS CustomerMasterDisplayConfig (
    ConfigID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectID INTEGER NOT NULL,
    GroupID INTEGER NOT NULL,
    GroupName TEXT NOT NULL,
    DisplayOrder INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (ProjectID) REFERENCES ProjectMaster(ProjectID),
    UNIQUE (ProjectID, GroupID),
    CHECK (GroupID BETWEEN 1 AND 5)
);

-- 顧客マスタ項目設定
CREATE TABLE IF NOT EXISTS CustomerMasterColumnConfig (
    ColumnConfigID INTEGER PRIMARY KEY AUTOINCREMENT,
    ConfigID INTEGER NOT NULL,
    ColumnName TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    DisplayOrder INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (ConfigID) REFERENCES CustomerMasterDisplayConfig(ConfigID) ON DELETE CASCADE
);

-- 履歴保存設定
CREATE TABLE IF NOT EXISTS HistoryLogSettings (
    SettingID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectID INTEGER NOT NULL,
    EnableHistoryLog INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    UpdatedAt TEXT,
    FOREIGN KEY (ProjectID) REFERENCES ProjectMaster(ProjectID),
    UNIQUE (ProjectID)
);

-- ============================================
-- 初期データ投入
-- ============================================

-- 案件マスタ
INSERT OR IGNORE INTO ProjectMaster (ProjectCode, ProjectName, Description, IsActive)
VALUES ('ProjectA', '案件A - サポートセンター', 'テスト用サポートセンター業務', 1);

-- オペレーターマスタ（パスワードは後でBCryptハッシュに更新）
-- 管理者: admin / admin123
INSERT OR IGNORE INTO OperatorMaster (ProjectID, LoginID, OperatorName, PasswordHash, Role, IsActive)
VALUES (1, 'admin', '管理者', '$2a$11$placeholder', 'Admin', 1);

-- 一般オペレーター: operator001 / operator123
INSERT OR IGNORE INTO OperatorMaster (ProjectID, LoginID, OperatorName, PasswordHash, Role, IsActive)
VALUES (1, 'operator001', '山田 太郎', '$2a$11$placeholder', 'General', 1);

-- カテゴリーマスタ
INSERT OR IGNORE INTO CategoryMaster (ProjectID, CategoryName, DisplayOrder, IsActive)
VALUES
    (1, '料金に関する問合せ', 1, 1),
    (1, '契約内容変更', 2, 1),
    (1, '解約', 3, 1),
    (1, 'クレーム', 4, 1),
    (1, 'その他', 5, 1);

-- ステータスマスタ
INSERT OR IGNORE INTO StatusMaster (ProjectID, StatusName, DisplayOrder, IsActive)
VALUES
    (1, '対応完了', 1, 1),
    (1, '対応中', 2, 1),
    (1, '折り返し予定', 3, 1),
    (1, '保留', 4, 1);

-- カスタム項目定義
INSERT OR IGNORE INTO CustomFieldDefinition (ProjectID, ColumnNumber, FieldName, DisplayName, FieldType, IsRequired, IsEnabled, DisplayOrder, SelectOptions, IsSearchable)
VALUES
    (1, 1, 'caller_name', '発信者名', 'Text', 0, 1, 1, NULL, 1),
    (1, 2, 'caller_tel', '発信者電話番号', 'Text', 0, 1, 2, NULL, 1),
    (1, 3, 'priority', '優先度', 'Select', 0, 1, 3, '["低","中","高"]', 0);

-- 顧客マスタ表示設定
INSERT OR IGNORE INTO CustomerMasterDisplayConfig (ProjectID, GroupID, GroupName, DisplayOrder)
VALUES
    (1, 1, '基本情報', 1),
    (1, 2, '契約情報', 2),
    (1, 3, '住所', 3),
    (1, 4, 'その他', 4),
    (1, 5, '画像・メモ', 5);

-- 顧客マスタ項目設定 - グループ1: 基本情報
INSERT OR IGNORE INTO CustomerMasterColumnConfig (ConfigID, ColumnName, DisplayName, DisplayOrder)
VALUES
    (1, 'CustomerID', '顧客ID', 1),
    (1, 'customer_name', '顧客名', 2),
    (1, 'kana', 'フリガナ', 3),
    (1, 'tel_no', '電話番号', 4),
    (1, 'email', 'メールアドレス', 5);

-- 顧客マスタ項目設定 - グループ2: 契約情報
INSERT OR IGNORE INTO CustomerMasterColumnConfig (ConfigID, ColumnName, DisplayName, DisplayOrder)
VALUES
    (2, 'contract_date', '契約日', 1),
    (2, 'contract_no', '契約番号', 2),
    (2, 'plan_name', '契約プラン', 3);

-- 顧客マスタ項目設定 - グループ3: 住所
INSERT OR IGNORE INTO CustomerMasterColumnConfig (ConfigID, ColumnName, DisplayName, DisplayOrder)
VALUES
    (3, 'postal_code', '郵便番号', 1),
    (3, 'addr1', '住所1', 2),
    (3, 'addr2', '住所2', 3);

-- 顧客マスタ項目設定 - グループ4: その他
INSERT OR IGNORE INTO CustomerMasterColumnConfig (ConfigID, ColumnName, DisplayName, DisplayOrder)
VALUES
    (4, 'member_rank', '会員ランク', 1),
    (4, 'remarks', '備考', 2);

-- 履歴保存設定
INSERT OR IGNORE INTO HistoryLogSettings (ProjectID, EnableHistoryLog)
VALUES (1, 1);
