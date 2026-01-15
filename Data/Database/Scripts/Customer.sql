-- ============================================
-- コール業務管理システム
-- 顧客マスタDB 初期化スクリプト (SQLite)
-- テスト用サンプルデータベース
-- ============================================

-- 顧客マスタテーブル
CREATE TABLE IF NOT EXISTS T_Customer (
    CustomerID TEXT PRIMARY KEY,
    customer_name TEXT NOT NULL,
    kana TEXT,
    tel_no TEXT,
    email TEXT,
    postal_code TEXT,
    addr1 TEXT,
    addr2 TEXT,
    contract_date TEXT,
    contract_no TEXT,
    plan_name TEXT,
    member_rank TEXT,
    remarks TEXT,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
);

-- 画像情報テーブル（オプション）
CREATE TABLE IF NOT EXISTS T_CustomerImages (
    ImageID INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerID TEXT NOT NULL,
    ImagePath TEXT NOT NULL,
    DisplayOrder INTEGER,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (CustomerID) REFERENCES T_Customer(CustomerID)
);

-- インデックス作成
CREATE INDEX IF NOT EXISTS IX_Customer_Name ON T_Customer(customer_name);
CREATE INDEX IF NOT EXISTS IX_Customer_Kana ON T_Customer(kana);
CREATE INDEX IF NOT EXISTS IX_Customer_Tel ON T_Customer(tel_no);
CREATE INDEX IF NOT EXISTS IX_Customer_Email ON T_Customer(email);

-- ============================================
-- テストデータ投入
-- ============================================

INSERT OR IGNORE INTO T_Customer (
    CustomerID, customer_name, kana, tel_no, email,
    postal_code, addr1, addr2, contract_date, contract_no,
    plan_name, member_rank, remarks
) VALUES
    ('C-00001', '田中 花子', 'タナカ ハナコ', '03-1234-5678', 'tanaka@example.com',
     '100-0001', '東京都千代田区千代田1-1-1', 'マンション101', '2024-01-15', 'CNT-2024-0001',
     'スタンダードプラン', 'ゴールド', '優良顧客'),

    ('C-00002', '佐藤 一郎', 'サトウ イチロウ', '03-2345-6789', 'sato@example.com',
     '100-0002', '東京都千代田区丸の内1-2-3', 'ビル5F', '2024-02-20', 'CNT-2024-0002',
     'プレミアムプラン', 'プラチナ', 'VIP顧客'),

    ('C-00003', '鈴木 次郎', 'スズキ ジロウ', '03-3456-7890', 'suzuki@example.com',
     '100-0003', '東京都千代田区大手町2-1-1', 'アパート202', '2024-03-10', 'CNT-2024-0003',
     'ベーシックプラン', 'シルバー', NULL),

    ('C-00004', '高橋 美咲', 'タカハシ ミサキ', '03-4567-8901', 'takahashi@example.com',
     '100-0004', '東京都千代田区神田3-4-5', NULL, '2024-04-05', 'CNT-2024-0004',
     'スタンダードプラン', 'ゴールド', NULL),

    ('C-00005', '伊藤 健太', 'イトウ ケンタ', '03-5678-9012', 'ito@example.com',
     '100-0005', '東京都千代田区有楽町4-5-6', 'マンション303', '2024-05-12', 'CNT-2024-0005',
     'ベーシックプラン', 'ブロンズ', NULL);
