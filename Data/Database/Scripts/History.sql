-- ============================================
-- コール業務管理システム
-- 問合せ履歴DB 初期化スクリプト (SQLite)
-- ============================================

-- 問合せ履歴
CREATE TABLE IF NOT EXISTS InquiryHistory (
    InquiryID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectID INTEGER NOT NULL,
    CustomerKey TEXT,
    OperatorID INTEGER NOT NULL,
    CategoryID INTEGER,
    StatusID INTEGER,
    InquiryContent TEXT NOT NULL,
    ResponseContent TEXT,
    FirstReceivedDateTime TEXT NOT NULL,
    UpdatedDateTime TEXT,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    CreatedBy INTEGER NOT NULL,
    UpdatedBy INTEGER,
    CustomCol01 TEXT,
    CustomCol02 TEXT,
    CustomCol03 TEXT,
    CustomCol04 TEXT,
    CustomCol05 TEXT,
    CustomCol06 TEXT,
    CustomCol07 TEXT,
    CustomCol08 TEXT,
    CustomCol09 TEXT,
    CustomCol10 TEXT
);

-- インデックス作成
CREATE INDEX IF NOT EXISTS IX_InquiryHistory_CustomerKey ON InquiryHistory(CustomerKey);
CREATE INDEX IF NOT EXISTS IX_InquiryHistory_ReceivedDateTime ON InquiryHistory(FirstReceivedDateTime DESC);
CREATE INDEX IF NOT EXISTS IX_InquiryHistory_Category ON InquiryHistory(CategoryID);
CREATE INDEX IF NOT EXISTS IX_InquiryHistory_Status ON InquiryHistory(StatusID);
CREATE INDEX IF NOT EXISTS IX_InquiryHistory_Operator ON InquiryHistory(OperatorID);
CREATE INDEX IF NOT EXISTS IX_InquiryHistory_Project ON InquiryHistory(ProjectID, FirstReceivedDateTime DESC);

-- 問合せ履歴更新履歴
CREATE TABLE IF NOT EXISTS InquiryHistoryLog (
    LogID INTEGER PRIMARY KEY AUTOINCREMENT,
    InquiryID INTEGER NOT NULL,
    ProjectID INTEGER NOT NULL,
    CustomerKey TEXT,
    OperatorID INTEGER NOT NULL,
    CategoryID INTEGER,
    StatusID INTEGER,
    InquiryContent TEXT,
    ResponseContent TEXT,
    FirstReceivedDateTime TEXT,
    UpdatedDateTime TEXT,
    CustomCol01 TEXT,
    CustomCol02 TEXT,
    CustomCol03 TEXT,
    CustomCol04 TEXT,
    CustomCol05 TEXT,
    CustomCol06 TEXT,
    CustomCol07 TEXT,
    CustomCol08 TEXT,
    CustomCol09 TEXT,
    CustomCol10 TEXT,
    UpdatedBy INTEGER NOT NULL,
    LoggedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (InquiryID) REFERENCES InquiryHistory(InquiryID) ON DELETE CASCADE
);

-- 顧客メモ
CREATE TABLE IF NOT EXISTS CustomerMemo (
    MemoID INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectID INTEGER NOT NULL,
    CustomerKey TEXT NOT NULL,
    MemoContent TEXT NOT NULL,
    CreatedBy INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
    UpdatedBy INTEGER,
    UpdatedAt TEXT,
    UNIQUE (ProjectID, CustomerKey)
);

-- インデックス作成
CREATE INDEX IF NOT EXISTS IX_CustomerMemo_CustomerKey ON CustomerMemo(CustomerKey);

-- ============================================
-- テストデータ投入
-- ============================================

-- 問合せ履歴サンプルデータ
INSERT OR IGNORE INTO InquiryHistory (
    InquiryID, ProjectID, CustomerKey, OperatorID, CategoryID, StatusID,
    InquiryContent, ResponseContent, FirstReceivedDateTime, CreatedBy
) VALUES
    (1, 1, 'C-00001', 1, 1, 1,
     '今月の請求金額について確認したい。前月より高くなっている理由を教えてほしい。',
     'オプションサービスの追加により金額が増加していることを説明。お客様ご納得いただきました。',
     '2025-01-10 09:30:00', 1),

    (2, 1, 'C-00001', 2, 2, 1,
     '契約プランの変更を希望。スタンダードからプレミアムへ変更したい。',
     'プラン変更手続きを実施。来月から新プラン適用となることを案内。',
     '2025-01-12 14:15:00', 2),

    (3, 1, 'C-00002', 1, 1, 1,
     '支払い方法をクレジットカードから口座振替に変更したい。',
     '口座振替依頼書を郵送。届き次第返送いただくよう案内。',
     '2025-01-08 10:45:00', 1),

    (4, 1, 'C-00002', 2, 4, 2,
     'サービスが繋がらないとのクレーム。昨日から利用できない状態。',
     '技術部門にエスカレーション。折り返し連絡予定。',
     '2025-01-14 16:30:00', 2),

    (5, 1, 'C-00003', 1, 3, 3,
     '解約を検討中。他社サービスへの乗り換えを考えている。',
     '継続特典を案内。検討いただき、後日ご連絡いただくことに。',
     '2025-01-13 11:00:00', 1),

    (6, 1, 'C-00003', 2, 5, 1,
     '請求書の再発行を依頼。紛失してしまったとのこと。',
     '再発行手続き完了。3営業日以内に届くことを案内。',
     '2025-01-11 13:20:00', 2),

    (7, 1, 'C-00004', 1, 1, 1,
     '料金プランの詳細を確認したい。現在のプランのサービス内容について。',
     'プラン内容を詳しく説明。追加オプションの案内も実施。',
     '2025-01-09 15:45:00', 1),

    (8, 1, 'C-00004', 2, 2, 4,
     '住所変更の手続きをしたい。来月引っ越し予定。',
     '変更届を郵送予定。お客様から書類が届き次第処理を行う。',
     '2025-01-15 09:00:00', 2),

    (9, 1, 'C-00005', 1, 5, 1,
     'サービス利用方法について質問。初めての利用で操作が分からない。',
     '操作方法を詳しく説明。マニュアルのPDFをメールで送付。',
     '2025-01-07 10:30:00', 1),

    (10, 1, 'C-00005', 2, 4, 2,
     'サービス品質に不満。遅延が頻繁に発生している。',
     '状況を確認中。技術部門と連携して原因調査を行う。',
     '2025-01-14 14:00:00', 2);
