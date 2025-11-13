Create database ThuGom;
USE ThuGom;
GO


------------------------------------------------------------
-- 🔐 TẠO CƠ CHẾ MÃ HÓA DỮ LIỆU
------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.symmetric_keys WHERE name = '##MS_DatabaseMasterKey##')
BEGIN
    CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'VanLam@2504';
END
GO

IF NOT EXISTS (SELECT * FROM sys.certificates WHERE name = 'Cert_PheLieu')
BEGIN
    CREATE CERTIFICATE Cert_PheLieu
    WITH SUBJECT = 'Chung nhan ma hoa PheLieu';
END
GO

IF NOT EXISTS (SELECT * FROM sys.symmetric_keys WHERE name = 'SymKey_PheLieu')
BEGIN
    CREATE SYMMETRIC KEY SymKey_PheLieu
    WITH ALGORITHM = AES_256
    ENCRYPTION BY CERTIFICATE Cert_PheLieu;
END
GO


------------------------------------------------------------
-- 🧩 BẢNG TÀI KHOẢN (CÓ MÃ HÓA + TRẠNG THÁI HOẠT ĐỘNG)
------------------------------------------------------------

CREATE TABLE TaiKhoan (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email VARBINARY(MAX) NOT NULL,
    MatKhauHash VARBINARY(MAX) NOT NULL,
    SoDienThoai VARBINARY(MAX) NOT NULL,
    VaiTro VARCHAR(20) NOT NULL,
    TrangThaiHoatDong BIT DEFAULT 1, -- 1=hoạt động, 0=đã xóa
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

--------------------------------------------------------------
CREATE TABLE KhachHang (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaiKhoanId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES TaiKhoan(Id) ON DELETE CASCADE,
    HoTen NVARCHAR(150) NOT NULL,
    DiaChiText NVARCHAR(255),
    GhiChu NVARCHAR(255),
    TrangThaiHoatDong BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

------------------------------------------------------------
-- 🧩 BẢNG DOANH NGHIỆP
------------------------------------------------------------

CREATE TABLE DoanhNghiep (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaiKhoanId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES TaiKhoan(Id) ON DELETE CASCADE,
    TenDoanhNghiep NVARCHAR(200) NOT NULL,
    MaSoThue VARBINARY(MAX),
    DiaChiText NVARCHAR(255),
    Website NVARCHAR(255),
    MoTa NVARCHAR(255),
    Verified BIT DEFAULT 0,
    TrangThaiHoatDong BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

------------------------------------------------------------
-- 🧩 BẢNG NHÂN VIÊN
------------------------------------------------------------
CREATE TABLE NhanVien (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaiKhoanId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES TaiKhoan(Id) ON DELETE CASCADE,
    DoanhNghiepId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES DoanhNghiep(Id),
    HoTen NVARCHAR(150) NOT NULL,
    TrangThaiSanSang BIT DEFAULT 1,
    TrangThaiHoatDong BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

------------------------------------------------------------
-- 🧩 BẢNG ADMIN
------------------------------------------------------------
CREATE TABLE AdminUser (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaiKhoanId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES TaiKhoan(Id) ON DELETE CASCADE,
    HoTen NVARCHAR(150),
    GhiChu NVARCHAR(255),
    TrangThaiHoatDong BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

CREATE TABLE OTP_ResetPassword (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaiKhoanId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES TaiKhoan(Id) ON DELETE CASCADE,
    OTPCode NVARCHAR(10) NOT NULL,
    ThoiGianTao DATETIME2 DEFAULT SYSDATETIME(),
    ThoiGianHetHan DATETIME2 NOT NULL,
    DaSuDung BIT DEFAULT 0
);
GO

GO
CREATE TABLE ThongBaoHeThong (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaiKhoanId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES TaiKhoan(Id),
    NoiDung NVARCHAR(500),
    DaDoc BIT DEFAULT 0,
    NgayGui DATETIME2 DEFAULT SYSDATETIME()
);
GO

------------------------------------------------------------
-- 🧩 TRIGGER: TỰ CẬP NHẬT THỜI GIAN SỬA
------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_TaiKhoan_Update
ON TaiKhoan
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE TaiKhoan
    SET UpdatedAt = SYSDATETIME()
    WHERE Id IN (SELECT Id FROM inserted);
END
GO


------------------------------------------------------------
-- 🧠 PROC: THÊM TÀI KHOẢN (MÃ HÓA)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_TaiKhoan_Insert
    @Email NVARCHAR(255),
    @MatKhau NVARCHAR(255),
    @SoDienThoai NVARCHAR(50),
    @VaiTro VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;

    INSERT INTO TaiKhoan (Email, MatKhauHash, SoDienThoai, VaiTro)
    VALUES (
        EncryptByKey(Key_GUID('SymKey_PheLieu'), @Email),
        EncryptByKey(Key_GUID('SymKey_PheLieu'), @MatKhau),
        EncryptByKey(Key_GUID('SymKey_PheLieu'), @SoDienThoai),
        @VaiTro
    );

    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO


------------------------------------------------------------
-- 🧠 PROC: CẬP NHẬT THÔNG TIN
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_TaiKhoan_Update
    @Id UNIQUEIDENTIFIER,
    @Email NVARCHAR(255),
    @MatKhau NVARCHAR(255),
    @SoDienThoai NVARCHAR(50),
    @VaiTro VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;

    UPDATE TaiKhoan
    SET
        Email = EncryptByKey(Key_GUID('SymKey_PheLieu'), @Email),
        MatKhauHash = EncryptByKey(Key_GUID('SymKey_PheLieu'), @MatKhau),
        SoDienThoai = EncryptByKey(Key_GUID('SymKey_PheLieu'), @SoDienThoai),
        VaiTro = @VaiTro
    WHERE Id = @Id;

    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO


------------------------------------------------------------
-- 🧠 PROC: XÓA MỀM (KHÔNG XÓA DỮ LIỆU)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_TaiKhoan_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    -- KHÔNG dùng SET NOCOUNT ON ở đây
    UPDATE TaiKhoan
    SET TrangThaiHoatDong = 0
    WHERE Id = @Id;

    -- Trả về số dòng bị ảnh hưởng cho .NET
    RETURN @@ROWCOUNT;
END
GO



------------------------------------------------------------
-- 🧠 PROC: LẤY DANH SÁCH (ĐÃ GIẢI MÃ)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_TaiKhoan_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;

    SELECT 
        Id,
        CONVERT(NVARCHAR(255), DecryptByKey(Email)) AS Email,
        CONVERT(NVARCHAR(255), DecryptByKey(MatKhauHash)) AS MatKhau,
        CONVERT(NVARCHAR(50), DecryptByKey(SoDienThoai)) AS SoDienThoai,
        VaiTro,
        TrangThaiHoatDong,
        CreatedAt,
        UpdatedAt
    FROM TaiKhoan;

    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO


------------------------------------------------------------
-- 🧠 PROC: LẤY THEO ID
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_TaiKhoan_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;

    SELECT 
        Id,
        CONVERT(NVARCHAR(255), DecryptByKey(Email)) AS Email,
        CONVERT(NVARCHAR(255), DecryptByKey(MatKhauHash)) AS MatKhau,
        CONVERT(NVARCHAR(50), DecryptByKey(SoDienThoai)) AS SoDienThoai,
        VaiTro,
        TrangThaiHoatDong,
        CreatedAt,
        UpdatedAt
    FROM TaiKhoan
    WHERE Id = @Id;

    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO

----------------------------------------
DROP PROCEDURE sp_TaiKhoan_Login


CREATE OR ALTER PROCEDURE sp_TaiKhoan_Login
    @TaiKhoan NVARCHAR(255),
    @MatKhau NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;

    DECLARE @TaiKhoanId UNIQUEIDENTIFIER, @VaiTro VARCHAR(20);

    -- 🔍 Tìm tài khoản
    SELECT TOP 1
        @TaiKhoanId = t.Id,
        @VaiTro = t.VaiTro
    FROM TaiKhoan t
    WHERE (
            CONVERT(NVARCHAR(255), DecryptByKey(t.Email)) = @TaiKhoan
         OR CONVERT(NVARCHAR(255), DecryptByKey(t.SoDienThoai)) = @TaiKhoan
        )
        AND CONVERT(NVARCHAR(255), DecryptByKey(t.MatKhauHash)) = @MatKhau
        AND t.TrangThaiHoatDong = 1;

    IF @TaiKhoanId IS NULL
    BEGIN
        CLOSE SYMMETRIC KEY SymKey_PheLieu;
        RAISERROR (N'Sai tài khoản hoặc mật khẩu!', 16, 1);
        RETURN;
    END

    -- 1️⃣ Trả về tài khoản chính
    SELECT 
        t.Id,
        CONVERT(NVARCHAR(255), DecryptByKey(t.Email)) AS Email,
        CONVERT(NVARCHAR(255), DecryptByKey(t.MatKhauHash)) AS MatKhau,
        CONVERT(NVARCHAR(255), DecryptByKey(t.SoDienThoai)) AS SoDienThoai,
        t.VaiTro,
        t.TrangThaiHoatDong
    FROM TaiKhoan t
    WHERE t.Id = @TaiKhoanId;

    -- 2️⃣ Trả chi tiết theo vai trò
    IF @VaiTro = 'KHACH_HANG'
        SELECT 
            k.Id,
            k.TaiKhoanId,
            k.HoTen,
            k.DiaChiText,
            k.GhiChu,
            k.TrangThaiHoatDong,
            CONVERT(NVARCHAR(255), DecryptByKey(t.Email)) AS Email,
            CONVERT(NVARCHAR(255), DecryptByKey(t.SoDienThoai)) AS SoDienThoai,
            k.CreatedAt,
            k.UpdatedAt
        FROM KhachHang k
        JOIN TaiKhoan t ON t.Id = k.TaiKhoanId
        WHERE k.TaiKhoanId = @TaiKhoanId;

    IF @VaiTro = 'DOANH_NGHIEP'
        SELECT 
            d.Id, d.TaiKhoanId, d.TenDoanhNghiep,
            CONVERT(NVARCHAR(100), DecryptByKey(d.MaSoThue)) AS MaSoThue,
            d.DiaChiText, d.Website, d.MoTa, d.Verified
        FROM DoanhNghiep d WHERE TaiKhoanId = @TaiKhoanId;

    IF @VaiTro = 'NHAN_VIEN'
        SELECT * FROM NhanVien WHERE TaiKhoanId = @TaiKhoanId;

    IF @VaiTro = 'ADMIN'
        SELECT * FROM AdminUser WHERE TaiKhoanId = @TaiKhoanId;

    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO

-----------------------------------------------
CREATE OR ALTER PROCEDURE sp_TaiKhoan_Register
    @Email NVARCHAR(255),
    @MatKhau NVARCHAR(255),
    @SoDienThoai NVARCHAR(50),
    @VaiTro VARCHAR(20),
    @HoTen NVARCHAR(150) = NULL,
    @DiaChiText NVARCHAR(255) = NULL,
    @TenDoanhNghiep NVARCHAR(200) = NULL,
    @MaSoThue NVARCHAR(100) = NULL,
    @Website NVARCHAR(255) = NULL,
    @MoTa NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewId UNIQUEIDENTIFIER = NEWID();
    DECLARE @OTP NVARCHAR(6) = RIGHT(CONVERT(VARCHAR(6), ABS(CHECKSUM(NEWID()))), 6);
    DECLARE @NoiDung NVARCHAR(500);

    -- 🔑 Mở khóa để mã hóa dữ liệu
    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;

    -- ⚠️ Kiểm tra trùng email hoặc số điện thoại
    IF EXISTS (
        SELECT 1 
        FROM TaiKhoan
        WHERE CONVERT(NVARCHAR(255), DecryptByKey(Email)) = @Email
           OR CONVERT(NVARCHAR(50), DecryptByKey(SoDienThoai)) = @SoDienThoai
    )
    BEGIN
        CLOSE SYMMETRIC KEY SymKey_PheLieu;
        RAISERROR(N'❌ Email hoặc số điện thoại đã tồn tại!', 16, 1);
        RETURN;
    END

    -- 🧩 Thêm vào bảng TaiKhoan
    INSERT INTO TaiKhoan (Id, Email, MatKhauHash, SoDienThoai, VaiTro)
    VALUES (
        @NewId,
        EncryptByKey(Key_GUID('SymKey_PheLieu'), @Email),
        EncryptByKey(Key_GUID('SymKey_PheLieu'), @MatKhau),
        EncryptByKey(Key_GUID('SymKey_PheLieu'), @SoDienThoai),
        @VaiTro
    );

    -- 🧠 Phân nhánh thêm thông tin theo vai trò
    IF UPPER(@VaiTro) = 'KHACH_HANG'
    BEGIN
        INSERT INTO KhachHang (TaiKhoanId, HoTen, DiaChiText, GhiChu)
        VALUES (@NewId, ISNULL(@HoTen, N'Khách hàng mới'), @DiaChiText, @MoTa);
    END
    ELSE IF UPPER(@VaiTro) = 'DOANH_NGHIEP'
    BEGIN
        INSERT INTO DoanhNghiep (TaiKhoanId, TenDoanhNghiep, MaSoThue, DiaChiText, Website, MoTa)
        VALUES (
            @NewId,
            ISNULL(@TenDoanhNghiep, N'Doanh nghiệp mới'),
            EncryptByKey(Key_GUID('SymKey_PheLieu'), @MaSoThue),
            @DiaChiText,
            @Website,
            @MoTa
        );
    END
    ELSE IF UPPER(@VaiTro) = 'NHAN_VIEN'
    BEGIN
        INSERT INTO NhanVien (TaiKhoanId, HoTen)
        VALUES (@NewId, ISNULL(@HoTen, N'Nhân viên mới'));
    END
    ELSE IF UPPER(@VaiTro) = 'ADMIN'
    BEGIN
        INSERT INTO AdminUser (TaiKhoanId, HoTen, GhiChu)
        VALUES (@NewId, ISNULL(@HoTen, N'Quản trị viên mới'), @MoTa);
    END

    -- 📨 Gửi thông báo chào mừng
    SET @NoiDung = N'🎉 Chào mừng bạn đã đăng ký tài khoản ' + @Email 
                 + N'. Mã xác thực OTP của bạn là: ' + @OTP
                 + N'. Vui lòng nhập mã này trong 10 phút để kích hoạt tài khoản.';

    INSERT INTO ThongBaoHeThong (TaiKhoanId, NoiDung)
    VALUES (@NewId, @NoiDung);

    -- 🧾 Lưu OTP xác minh tài khoản (có hiệu lực 10 phút)
    INSERT INTO OTP_ResetPassword (TaiKhoanId, OTPCode, ThoiGianHetHan)
    VALUES (@NewId, @OTP, DATEADD(MINUTE, 10, SYSDATETIME()));

    CLOSE SYMMETRIC KEY SymKey_PheLieu;

    -- ✅ Trả về cho backend: Id + Vai trò + Mã OTP để gửi Gmail hoặc SMS
    SELECT 
        @NewId AS TaiKhoanId, 
        @VaiTro AS VaiTro,
        @OTP AS MaOTP,
        N'Đăng ký thành công. Mã OTP đã được gửi về Gmail hoặc số điện thoại của bạn.' AS ThongBao;
END
GO


---------------------------------------

-- 🔑 MỞ KHÓA TRƯỚC
OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;
GO

-- ➕ THÊM DỮ LIỆU MẪU
EXEC sp_TaiKhoan_Insert N'admin@phelieu.vn', N'admin@2504', N'0944456478', 'ADMIN';
EXEC sp_TaiKhoan_Insert N'nguyenvanan@gmail.com', N'khachhang123', N'0909111222', 'KHACH_HANG';
EXEC sp_TaiKhoan_Insert N'tranthibinh@yahoo.com', N'matkhau456', N'0978123456', 'KHACH_HANG';
EXEC sp_TaiKhoan_Insert N'congtyhn@company.vn', N'doanhnghiep789', N'0933334444', 'DOANH_NGHIEP';
EXEC sp_TaiKhoan_Insert N'hana@phelieu.vn', N'nhanvien999', N'0966667777', 'NHAN_VIEN';
GO

-- 🔓 Mở khóa trước khi chạy
OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;
GO

-- 🧩 Thêm tài khoản + khách hàng mới bằng thủ tục đăng ký
EXEC sp_TaiKhoan_Register 
    @Email = N'lethilam@gmail.com', 
    @MatKhau = N'lam@123', 
    @SoDienThoai = N'0901123456', 
    @VaiTro = 'KHACH_HANG',
    @HoTen = N'Lê Thị Lam', 
    @DiaChiText = N'25 Nguyễn Đình Chiểu, Quận 3, TP.HCM', 
    @MoTa = N'Khách thân thiết mới';
GO

-- 🔒 Đóng khóa lại sau khi chạy
CLOSE SYMMETRIC KEY SymKey_PheLieu;
GO


-- 🔒 ĐÓNG KHÓA
CLOSE SYMMETRIC KEY SymKey_PheLieu;
GO

----------------------------

-----------------------------------
CREATE OR ALTER PROCEDURE sp_TaiKhoan_QuenMatKhau
    @TaiKhoan NVARCHAR(255) -- Email hoặc SĐT
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;

    DECLARE @TaiKhoanId UNIQUEIDENTIFIER;

    SELECT TOP 1 @TaiKhoanId = Id
    FROM TaiKhoan
    WHERE TrangThaiHoatDong = 1 AND
        (CONVERT(NVARCHAR(255), DecryptByKey(Email)) = @TaiKhoan
         OR CONVERT(NVARCHAR(50), DecryptByKey(SoDienThoai)) = @TaiKhoan);

    IF @TaiKhoanId IS NULL
    BEGIN
        CLOSE SYMMETRIC KEY SymKey_PheLieu;
        RAISERROR(N'Không tìm thấy tài khoản hợp lệ!', 16, 1);
        RETURN;
    END

    -- 🔐 Tạo mã OTP ngẫu nhiên
    DECLARE @OTP NVARCHAR(6) = RIGHT(CONVERT(VARCHAR(6), ABS(CHECKSUM(NEWID()))), 6);

    -- 🔢 Xóa các OTP cũ chưa dùng
    DELETE FROM OTP_ResetPassword WHERE TaiKhoanId = @TaiKhoanId AND DaSuDung = 0;

    -- ➕ Thêm OTP mới (hiệu lực 10 phút)
    INSERT INTO OTP_ResetPassword (TaiKhoanId, OTPCode, ThoiGianHetHan)
    VALUES (@TaiKhoanId, @OTP, DATEADD(MINUTE, 10, SYSDATETIME()));

    CLOSE SYMMETRIC KEY SymKey_PheLieu;

    -- ✅ Trả về OTP cho backend để gửi qua Gmail hoặc SMS
    SELECT @TaiKhoanId AS TaiKhoanId, @OTP AS MaOTP;
END
GO

----
CREATE OR ALTER PROCEDURE sp_TaiKhoan_ResetPassword
    @TaiKhoanId UNIQUEIDENTIFIER,
    @OTP NVARCHAR(10),
    @MatKhauMoi NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsValid BIT = 0;

    SELECT @IsValid = 1
    FROM OTP_ResetPassword
    WHERE TaiKhoanId = @TaiKhoanId
      AND OTPCode = @OTP
      AND DaSuDung = 0
      AND ThoiGianHetHan > SYSDATETIME();

    IF @IsValid = 0
    BEGIN
        RAISERROR(N'Mã OTP không hợp lệ hoặc đã hết hạn!', 16, 1);
        RETURN;
    END

    -- 🔒 Mở khóa mã hóa
    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;

    UPDATE TaiKhoan
    SET MatKhauHash = EncryptByKey(Key_GUID('SymKey_PheLieu'), @MatKhauMoi)
    WHERE Id = @TaiKhoanId;

    -- ✅ Đánh dấu OTP đã dùng
    UPDATE OTP_ResetPassword
    SET DaSuDung = 1
    WHERE TaiKhoanId = @TaiKhoanId AND OTPCode = @OTP;

    CLOSE SYMMETRIC KEY SymKey_PheLieu;

    PRINT N'✅ Đổi mật khẩu thành công!';
END



---------------------------------------------------

-- 🧩 TRIGGER TỰ CẬP NHẬT UpdatedAt
------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_Entity_Update
ON KhachHang
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE KhachHang SET UpdatedAt = SYSDATETIME() WHERE Id IN (SELECT Id FROM inserted);
END
GO

CREATE OR ALTER TRIGGER trg_DoanhNghiep_Update
ON DoanhNghiep
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE DoanhNghiep SET UpdatedAt = SYSDATETIME() WHERE Id IN (SELECT Id FROM inserted);
END
GO

CREATE OR ALTER TRIGGER trg_NhanVien_Update
ON NhanVien
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE NhanVien SET UpdatedAt = SYSDATETIME() WHERE Id IN (SELECT Id FROM inserted);
END
GO

CREATE OR ALTER TRIGGER trg_Admin_Update
ON AdminUser
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE AdminUser SET UpdatedAt = SYSDATETIME() WHERE Id IN (SELECT Id FROM inserted);
END
GO


------------------------------------------------------------
-- 🧠 PROC KHÁCH HÀNG
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_KhachHang_Insert
    @TaiKhoanId UNIQUEIDENTIFIER,
    @HoTen NVARCHAR(150),
    @DiaChiText NVARCHAR(255) = NULL,
    @GhiChu NVARCHAR(255) = NULL
AS
INSERT INTO KhachHang (TaiKhoanId, HoTen, DiaChiText, GhiChu)
VALUES (@TaiKhoanId, @HoTen, @DiaChiText, @GhiChu);
GO

CREATE OR ALTER PROCEDURE sp_KhachHang_Update
    @Id UNIQUEIDENTIFIER,
    @HoTen NVARCHAR(150),
    @DiaChiText NVARCHAR(255),
    @GhiChu NVARCHAR(255)
AS
UPDATE KhachHang SET HoTen=@HoTen, DiaChiText=@DiaChiText, GhiChu=@GhiChu WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_KhachHang_Delete
    @Id UNIQUEIDENTIFIER
AS
UPDATE KhachHang SET TrangThaiHoatDong=0 WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_KhachHang_GetAll
AS
BEGIN
    OPEN SYMMETRIC KEY SymKey_PheLieu
    DECRYPTION BY CERTIFICATE Cert_PheLieu;

    SELECT 
        k.*,
        CONVERT(NVARCHAR(255), DecryptByKey(t.Email)) AS Email,
        CONVERT(NVARCHAR(50), DecryptByKey(t.SoDienThoai)) AS SoDienThoai
    FROM KhachHang k
    JOIN TaiKhoan t ON t.Id = k.TaiKhoanId;

    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO


CREATE OR ALTER PROCEDURE sp_KhachHang_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    OPEN SYMMETRIC KEY SymKey_PheLieu 
        DECRYPTION BY CERTIFICATE Cert_PheLieu;

    SELECT 
        k.*,
        CONVERT(NVARCHAR(255), DecryptByKey(t.Email)) AS Email,
        CONVERT(NVARCHAR(50), DecryptByKey(t.SoDienThoai)) AS SoDienThoai
    FROM KhachHang k
    JOIN TaiKhoan t ON t.Id = k.TaiKhoanId
    WHERE k.Id = @Id;

    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO

-------------------------------------------------

-- 🧩 NHÂN VIÊN
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_NhanVien_Insert
    @TaiKhoanId UNIQUEIDENTIFIER,
    @DoanhNghiepId UNIQUEIDENTIFIER = NULL,
    @HoTen NVARCHAR(150)
AS
INSERT INTO NhanVien (TaiKhoanId, DoanhNghiepId, HoTen)
VALUES (@TaiKhoanId, @DoanhNghiepId, @HoTen);
GO

CREATE OR ALTER PROCEDURE sp_NhanVien_Update
    @Id UNIQUEIDENTIFIER,
    @DoanhNghiepId UNIQUEIDENTIFIER = NULL,
    @HoTen NVARCHAR(150),
    @TrangThaiSanSang BIT
AS
UPDATE NhanVien
SET DoanhNghiepId = @DoanhNghiepId,
    HoTen = @HoTen,
    TrangThaiSanSang = @TrangThaiSanSang
WHERE Id = @Id;
GO

CREATE OR ALTER PROCEDURE sp_NhanVien_Delete
    @Id UNIQUEIDENTIFIER
AS
UPDATE NhanVien SET TrangThaiHoatDong = 0 WHERE Id = @Id;
GO

CREATE OR ALTER PROCEDURE sp_NhanVien_GetAll
AS
SELECT nv.*, dn.TenDoanhNghiep,
       CONVERT(NVARCHAR(255), DecryptByKeyAutoCert(Cert_ID('Cert_PheLieu'), NULL, tk.Email)) AS Email
FROM NhanVien nv
LEFT JOIN DoanhNghiep dn ON nv.DoanhNghiepId = dn.Id
JOIN TaiKhoan tk ON tk.Id = nv.TaiKhoanId;
GO

CREATE OR ALTER PROCEDURE sp_NhanVien_GetById
    @Id UNIQUEIDENTIFIER
AS
SELECT nv.*, dn.TenDoanhNghiep,
       CONVERT(NVARCHAR(255), DecryptByKeyAutoCert(Cert_ID('Cert_PheLieu'), NULL, tk.Email)) AS Email
FROM NhanVien nv
LEFT JOIN DoanhNghiep dn ON nv.DoanhNghiepId = dn.Id
JOIN TaiKhoan tk ON tk.Id = nv.TaiKhoanId
WHERE nv.Id = @Id;
GO


------------------------------------------------------------
-- 🧩 DOANH NGHIỆP
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DoanhNghiep_Insert
    @TaiKhoanId UNIQUEIDENTIFIER,
    @TenDoanhNghiep NVARCHAR(200),
    @MaSoThue NVARCHAR(50),
    @DiaChiText NVARCHAR(255),
    @Website NVARCHAR(255),
    @MoTa NVARCHAR(255)
AS
BEGIN
    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;
    INSERT INTO DoanhNghiep (TaiKhoanId, TenDoanhNghiep, MaSoThue, DiaChiText, Website, MoTa)
    VALUES (@TaiKhoanId, @TenDoanhNghiep,
            EncryptByKey(Key_GUID('SymKey_PheLieu'), @MaSoThue),
            @DiaChiText, @Website, @MoTa);
    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO

CREATE OR ALTER PROCEDURE sp_DoanhNghiep_Update
    @Id UNIQUEIDENTIFIER,
    @TenDoanhNghiep NVARCHAR(200),
    @MaSoThue NVARCHAR(50),
    @DiaChiText NVARCHAR(255),
    @Website NVARCHAR(255),
    @MoTa NVARCHAR(255)
AS
BEGIN
    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;
    UPDATE DoanhNghiep
    SET TenDoanhNghiep = @TenDoanhNghiep,
        MaSoThue = EncryptByKey(Key_GUID('SymKey_PheLieu'), @MaSoThue),
        DiaChiText = @DiaChiText,
        Website = @Website,
        MoTa = @MoTa
    WHERE Id = @Id;
    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO

CREATE OR ALTER PROCEDURE sp_DoanhNghiep_Delete
    @Id UNIQUEIDENTIFIER
AS
UPDATE DoanhNghiep SET TrangThaiHoatDong = 0 WHERE Id = @Id;
GO

CREATE OR ALTER PROCEDURE sp_DoanhNghiep_GetAll
AS
BEGIN
    OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;
    SELECT d.*, CONVERT(NVARCHAR(50), DecryptByKey(d.MaSoThue)) AS MaSoThueGiaiMa,
           CONVERT(NVARCHAR(255), DecryptByKeyAutoCert(Cert_ID('Cert_PheLieu'), NULL, t.Email)) AS Email
    FROM DoanhNghiep d
    JOIN TaiKhoan t ON t.Id = d.TaiKhoanId
    WHERE d.TrangThaiHoatDong = 1;
    CLOSE SYMMETRIC KEY SymKey_PheLieu;
END
GO

--------------------------------------------

-- 🧩 ADMIN
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Admin_Insert
    @TaiKhoanId UNIQUEIDENTIFIER,
    @HoTen NVARCHAR(150),
    @GhiChu NVARCHAR(255)
AS
INSERT INTO AdminUser (TaiKhoanId, HoTen, GhiChu)
VALUES (@TaiKhoanId, @HoTen, @GhiChu);
GO

CREATE OR ALTER PROCEDURE sp_Admin_Update
    @Id UNIQUEIDENTIFIER,
    @HoTen NVARCHAR(150),
    @GhiChu NVARCHAR(255)
AS
UPDATE AdminUser SET HoTen=@HoTen, GhiChu=@GhiChu WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_Admin_Delete
    @Id UNIQUEIDENTIFIER
AS
UPDATE AdminUser SET TrangThaiHoatDong=0 WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_Admin_GetAll
AS
SELECT a.*, 
       CONVERT(NVARCHAR(255), DecryptByKeyAutoCert(Cert_ID('Cert_PheLieu'), NULL, t.Email)) AS Email
FROM AdminUser a
JOIN TaiKhoan t ON a.TaiKhoanId = t.Id
WHERE a.TrangThaiHoatDong = 1;
GO

-------------------------------------------------
OPEN SYMMETRIC KEY SymKey_PheLieu DECRYPTION BY CERTIFICATE Cert_PheLieu;
GO

------------------------------------------------------------
-- 🧩 1️⃣ ADMIN
------------------------------------------------------------
DECLARE @TaiKhoan_Admin UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM TaiKhoan WHERE VaiTro = 'ADMIN'
);

EXEC sp_Admin_Insert 
    @TaiKhoanId = @TaiKhoan_Admin,
    @HoTen = N'Trần Văn Lâm',
    @GhiChu = N'Quản trị hệ thống';

------------------------------------------------------------
-- 🧩 2️⃣ KHÁCH HÀNG
------------------------------------------------------------
DECLARE @TaiKhoan_KH1 UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM TaiKhoan WHERE VaiTro = 'KHACH_HANG' 
    ORDER BY CreatedAt ASC
);

DECLARE @TaiKhoan_KH2 UNIQUEIDENTIFIER = (
    SELECT Id FROM (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt ASC) AS RowNum
        FROM TaiKhoan WHERE VaiTro = 'KHACH_HANG'
    ) t WHERE RowNum = 2
);

EXEC sp_KhachHang_Insert 
    @TaiKhoanId = @TaiKhoan_KH1,
    @HoTen = N'Nguyễn Văn An',
    @DiaChiText = N'123 Lê Lợi, Quận 1, TP.HCM',
    @GhiChu = N'Khách thân thiết';

EXEC sp_KhachHang_Insert 
    @TaiKhoanId = @TaiKhoan_KH2,
    @HoTen = N'Trần Thị Bình',
    @DiaChiText = N'45 Nguyễn Trãi, Quận 5, TP.HCM',
    @GhiChu = N'Khách mới đăng ký';
GO

------------------------------------------------------------
-- 🧩 3️⃣ DOANH NGHIỆP
------------------------------------------------------------
DECLARE @TaiKhoan_DN UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM TaiKhoan WHERE VaiTro = 'DOANH_NGHIEP'
);

EXEC sp_DoanhNghiep_Insert 
    @TaiKhoanId = @TaiKhoan_DN,
    @TenDoanhNghiep = N'Công ty Thu gom HN',
    @MaSoThue = N'0315671234',
    @DiaChiText = N'456 Nguyễn Huệ, Quận 1, TP.HCM',
    @Website = N'www.thugomhn.vn',
    @MoTa = N'Doanh nghiệp chuyên thu gom phế liệu các khu vực phía Nam';
GO

------------------------------------------------------------
-- 🧩 4️⃣ NHÂN VIÊN
------------------------------------------------------------
DECLARE @TaiKhoan_NV UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM TaiKhoan WHERE VaiTro = 'NHAN_VIEN'
);

DECLARE @DoanhNghiepId UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM DoanhNghiep
);

EXEC sp_NhanVien_Insert 
    @TaiKhoanId = @TaiKhoan_NV,
    @DoanhNghiepId = @DoanhNghiepId,
    @HoTen = N'Hà Thị Hạnh';
GO

-- 🔒 Đóng khóa sau khi thêm
CLOSE SYMMETRIC KEY SymKey_PheLieu;
GO



/////////////////////////////////////////////////////
-------- Loại 2 -------------------------------------
/////////////////////////////////////////////////////
CREATE TABLE LoaiPheLieu (
    MaLoai VARCHAR(20) PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255)
);
GO

CREATE TABLE PheLieu (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenPheLieu NVARCHAR(150) NOT NULL,
    MaLoai VARCHAR(20) FOREIGN KEY REFERENCES LoaiPheLieu(MaLoai),
    KhoiLuong DECIMAL(10,2),
    DonGia DECIMAL(18,2),
    MoTa NVARCHAR(255),
    HinhAnh NVARCHAR(255),
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

------------------------------------------------------------
-- 🔧 TRIGGER TỰ CẬP NHẬT UpdatedAt
------------------------------------------------------------
CREATE OR ALTER TRIGGER trg_PheLieu_UpdateTime
ON PheLieu
AFTER UPDATE
AS
BEGIN
    UPDATE PheLieu SET UpdatedAt = SYSDATETIME()
    WHERE Id IN (SELECT Id FROM inserted);
END;
GO

------------------------------------------------------------
-- 🧩 PROCEDURE CRUD
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_PheLieu_Insert
    @TenPheLieu NVARCHAR(150),
    @MaLoai VARCHAR(20),
    @KhoiLuong DECIMAL(10,2),
    @DonGia DECIMAL(18,2),
    @MoTa NVARCHAR(255),
    @HinhAnh NVARCHAR(255)
AS
INSERT INTO PheLieu (TenPheLieu, MaLoai, KhoiLuong, DonGia, MoTa, HinhAnh)
VALUES (@TenPheLieu, @MaLoai, @KhoiLuong, @DonGia, @MoTa, @HinhAnh);
GO

CREATE OR ALTER PROCEDURE sp_PheLieu_Update
    @Id UNIQUEIDENTIFIER,
    @TenPheLieu NVARCHAR(150),
    @MaLoai VARCHAR(20),
    @KhoiLuong DECIMAL(10,2),
    @DonGia DECIMAL(18,2),
    @MoTa NVARCHAR(255),
    @HinhAnh NVARCHAR(255)
AS
UPDATE PheLieu
SET TenPheLieu=@TenPheLieu, MaLoai=@MaLoai, KhoiLuong=@KhoiLuong, DonGia=@DonGia,
    MoTa=@MoTa, HinhAnh=@HinhAnh
WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_PheLieu_Delete
    @Id UNIQUEIDENTIFIER
AS
DELETE FROM PheLieu WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_PheLieu_GetAll
AS
SELECT p.*, l.TenLoai FROM PheLieu p
LEFT JOIN LoaiPheLieu l ON p.MaLoai = l.MaLoai
ORDER BY p.CreatedAt DESC;
GO


------------------------------------------------------------
-- 🧩 CRUD cho bảng LoaiPheLieu
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_LoaiPheLieu_GetAll
AS
BEGIN
    SELECT MaLoai, TenLoai, MoTa
    FROM LoaiPheLieu
    ORDER BY TenLoai ASC;
END
GO

CREATE OR ALTER PROCEDURE sp_LoaiPheLieu_GetById
    @MaLoai VARCHAR(20)
AS
BEGIN
    SELECT MaLoai, TenLoai, MoTa
    FROM LoaiPheLieu
    WHERE MaLoai = @MaLoai;
END
GO

CREATE OR ALTER PROCEDURE sp_LoaiPheLieu_Insert
    @MaLoai VARCHAR(20),
    @TenLoai NVARCHAR(100),
    @MoTa NVARCHAR(255)
AS
BEGIN
    INSERT INTO LoaiPheLieu (MaLoai, TenLoai, MoTa)
    VALUES (@MaLoai, @TenLoai, @MoTa);
END
GO

CREATE OR ALTER PROCEDURE sp_LoaiPheLieu_Update
    @MaLoai VARCHAR(20),
    @TenLoai NVARCHAR(100),
    @MoTa NVARCHAR(255)
AS
BEGIN
    UPDATE LoaiPheLieu
    SET TenLoai = @TenLoai,
        MoTa = @MoTa
    WHERE MaLoai = @MaLoai;
END
GO

CREATE OR ALTER PROCEDURE sp_LoaiPheLieu_Delete
    @MaLoai VARCHAR(20)
AS
BEGIN
    DELETE FROM LoaiPheLieu WHERE MaLoai = @MaLoai;
END
GO


-- Bảng loại phế liệu
INSERT INTO LoaiPheLieu (MaLoai, TenLoai, MoTa)
VALUES
('S01', N'Sắt thép', N'Phế liệu sắt, thép từ công trình'),
('N01', N'Nhôm', N'Phế liệu nhôm từ cửa và vỏ lon'),
('D01', N'Đồng', N'Dây điện, lõi đồng'),
('GI01', N'Giấy', N'Giấy, carton cũ'),
('NH01', N'Nhựa', N'Vỏ chai, can nhựa'),
('K01', N'Kim loại khác', N'Hợp kim, inox'),
('CH01', N'Chì', N'Ắc quy cũ'),
('TH01', N'Thiếc', N'Vỏ đồ hộp thiếc'),
('VE01', N'Ve chai', N'Chai thủy tinh'),
('DT01', N'Điện tử', N'Linh kiện điện tử cũ');
GO

-- Bảng phế liệu
INSERT INTO PheLieu (TenPheLieu, MaLoai, KhoiLuong, DonGia, MoTa, HinhAnh)
VALUES
(N'Sắt công trình', 'S01', 120.5, 9000, N'Sắt vụn từ công trình xây dựng', 'satthep1.jpg'),
(N'Đồng dây điện', 'D01', 15.2, 120000, N'Đồng từ dây điện cũ', 'dong1.jpg'),
(N'Nhôm cửa cũ', 'N01', 25.0, 35000, N'Cửa nhôm thanh lý', 'nhom1.jpg'),
(N'Giấy carton', 'GI01', 40.3, 4000, N'Carton từ kho hàng', 'giay1.jpg'),
(N'Nhựa pet', 'NH01', 60.0, 8000, N'Nhựa từ chai nước suối', 'nhua1.jpg'),
(N'Inox phế liệu', 'K01', 18.4, 28000, N'Inox nhà bếp cũ', 'inox1.jpg'),
(N'Ắc quy chì', 'CH01', 30.5, 25000, N'Ắc quy xe máy hư', 'chi1.jpg'),
(N'Thiếc hộp sữa', 'TH01', 12.0, 10000, N'Hộp sữa thiếc cũ', 'thiec1.jpg'),
(N'Chai bia thủy tinh', 'VE01', 100.0, 1500, N'Chai thủy tinh tái chế', 'vechai1.jpg'),
(N'Bo mạch cũ', 'DT01', 5.5, 80000, N'Linh kiện điện tử cũ', 'dientu1.jpg');
GO

/////////////////////////////////////////////////////
-------- Loại 3 -------------------------------------
/////////////////////////////////////////////////////

CREATE TABLE TrangThaiLichHen (
    Code VARCHAR(20) PRIMARY KEY,
    Ten NVARCHAR(50)
);
GO

INSERT INTO TrangThaiLichHen VALUES
('DA_DAT',N'Đã đặt'),
('DA_XAC_NHAN',N'Đã xác nhận'),
('DANG_THU_GOM',N'Đang thu gom'),
('HOAN_THANH',N'Hoàn thành'),
('HUY',N'Huỷ');
GO
CREATE OR ALTER PROCEDURE sp_TrangThaiLichHen_GetAll
AS
BEGIN
    SELECT Code, Ten
    FROM TrangThaiLichHen
    ORDER BY Ten ASC;
END
GO


CREATE TABLE LichHen (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    KhachHangId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES KhachHang(Id),
    DiaChi NVARCHAR(255),
    ThoiGianHen DATETIME2 NOT NULL,
    TrangThaiCode VARCHAR(20) FOREIGN KEY REFERENCES TrangThaiLichHen(Code),
    GhiChu NVARCHAR(255),
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

------------------------------------------------------------
-- 🧩 PROCEDURE CRUD
------------------------------------------------------------
-- 🔍 LẤY DANH SÁCH TOÀN BỘ
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_LichHen_GetAll
AS
BEGIN
    SELECT 
        lh.Id,
        lh.KhachHangId,
        kh.HoTen AS TenKhachHang,
        lh.DiaChi,
        lh.ThoiGianHen,
        lh.TrangThaiCode,
        tt.Ten AS TenTrangThai,
        lh.GhiChu,
        lh.CreatedAt,
        lh.UpdatedAt
    FROM LichHen lh
    JOIN KhachHang kh ON lh.KhachHangId = kh.Id
    JOIN TrangThaiLichHen tt ON lh.TrangThaiCode = tt.Code
    ORDER BY lh.CreatedAt DESC;
END
GO

------------------------------------------------------------
-- 🔍 LẤY CHI TIẾT THEO ID
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_LichHen_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SELECT 
        lh.Id,
        lh.KhachHangId,
        kh.HoTen AS TenKhachHang,
        lh.DiaChi,
        lh.ThoiGianHen,
        lh.TrangThaiCode,
        tt.Ten AS TenTrangThai,
        lh.GhiChu,
        lh.CreatedAt,
        lh.UpdatedAt
    FROM LichHen lh
    JOIN KhachHang kh ON lh.KhachHangId = kh.Id
    JOIN TrangThaiLichHen tt ON lh.TrangThaiCode = tt.Code
    WHERE lh.Id = @Id;
END
GO

------------------------------------------------------------
-- ➕ THÊM MỚI
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_LichHen_Insert
    @KhachHangId UNIQUEIDENTIFIER,
    @DiaChi NVARCHAR(255),
    @ThoiGianHen DATETIME2,
    @TrangThaiCode VARCHAR(20),
    @GhiChu NVARCHAR(255)
AS
BEGIN
    INSERT INTO LichHen (KhachHangId, DiaChi, ThoiGianHen, TrangThaiCode, GhiChu)
    VALUES (@KhachHangId, @DiaChi, @ThoiGianHen, @TrangThaiCode, @GhiChu);
END
GO

------------------------------------------------------------
-- ✏️ CẬP NHẬT TRẠNG THÁI / GHI CHÚ
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_LichHen_Update
    @Id UNIQUEIDENTIFIER,
    @TrangThaiCode VARCHAR(20),
    @GhiChu NVARCHAR(255)
AS
BEGIN
    UPDATE LichHen
    SET TrangThaiCode=@TrangThaiCode, GhiChu=@GhiChu, UpdatedAt=SYSDATETIME()
    WHERE Id=@Id;
END
GO

------------------------------------------------------------
-- ✏️ CẬP NHẬT TOÀN BỘ (ADMIN)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_LichHen_UpdateFull
    @Id UNIQUEIDENTIFIER,
    @DiaChi NVARCHAR(255),
    @ThoiGianHen DATETIME2,
    @TrangThaiCode VARCHAR(20),
    @GhiChu NVARCHAR(255)
AS
BEGIN
    UPDATE LichHen
    SET DiaChi=@DiaChi,
        ThoiGianHen=@ThoiGianHen,
        TrangThaiCode=@TrangThaiCode,
        GhiChu=@GhiChu,
        UpdatedAt=SYSDATETIME()
    WHERE Id=@Id;
END
GO

------------------------------------------------------------
-- ❌ XÓA
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_LichHen_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    DELETE FROM LichHen WHERE Id=@Id;
END
GO
-- Lưu ý: giả sử đã có 2 khách hàng trong bảng KhachHang
DECLARE @KH1 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM KhachHang);
DECLARE @KH2 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM KhachHang ORDER BY CreatedAt DESC);

INSERT INTO LichHen (KhachHangId, DiaChi, ThoiGianHen, TrangThaiCode, GhiChu)
VALUES
(@KH1, N'12 Nguyễn Văn Cừ, Q5, TP.HCM', '2025-10-31 08:00', 'DA_DAT', N'Thu gom sắt vụn'),
(@KH1, N'14 Trần Phú, Q5', '2025-11-01 09:00', 'DA_XAC_NHAN', N'Nhôm lon'),
(@KH2, N'25 Nguyễn Thị Minh Khai, Q1', '2025-11-02 13:00', 'DANG_THU_GOM', N'Giấy vụn'),
(@KH1, N'45 Hoàng Diệu, Q4', '2025-11-03 07:30', 'DA_DAT', N'Đồng dây điện'),
(@KH2, N'27 Nguyễn Huệ, Q1', '2025-11-04 14:00', 'HOAN_THANH', N'Nhựa PET'),
(@KH2, N'91 CMT8, Q3', '2025-11-05 15:00', 'HUY', N'Hủy lịch do mưa'),
(@KH1, N'2 Lý Thường Kiệt, Q10', '2025-11-06 09:30', 'DA_XAC_NHAN', N'Phế liệu hỗn hợp'),
(@KH2, N'78 Nguyễn Văn Linh, Q7', '2025-11-07 08:00', 'DANG_THU_GOM', N'Thu gom nhanh'),
(@KH1, N'13 Lê Lợi, Q1', '2025-11-08 10:00', 'DA_DAT', N'Thu gom inox'),
(@KH2, N'89 Điện Biên Phủ, Q3', '2025-11-09 11:00', 'DA_DAT', N'Phế liệu nhà máy');
GO

/////////////////////////////////////////////////////
-------- Loại 4 -------------------------------------
/////////////////////////////////////////////////////

CREATE TABLE TrangThaiDon (
    Code VARCHAR(20) PRIMARY KEY,
    Ten NVARCHAR(50)
);
GO

INSERT INTO TrangThaiDon VALUES
('TAO_MOI',N'Tạo mới'),
('DA_BAO_GIA',N'Đã báo giá'),
('DA_THOA_THUAN',N'Đã thỏa thuận'),
('DA_THANH_TOAN',N'Đã thanh toán'),
('DA_HUY',N'Đã hủy');
GO

CREATE TABLE PaymentMethod (
    Code VARCHAR(20) PRIMARY KEY,
    Ten NVARCHAR(50)
);
GO

INSERT INTO PaymentMethod VALUES
('TIEN_MAT',N'Tiền mặt'),
('CHUYEN_KHOAN',N'Chuyển khoản'),
('VI_DIEN_TU',N'Ví điện tử');
GO

CREATE TABLE DonThuGom (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LichHenId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES LichHen(Id),
    NhanVienId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES NhanVien(Id),
    DoanhNghiepId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES DoanhNghiep(Id),
    TrangThaiCode VARCHAR(20) FOREIGN KEY REFERENCES TrangThaiDon(Code),
    TongTien DECIMAL(18,2),
    PhuongThucTT VARCHAR(20) FOREIGN KEY REFERENCES PaymentMethod(Code),
    GhiChu NVARCHAR(255),
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO


------------------------------------------------------------
-- 🧩 PROCEDURE CRUD
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DonThuGom_Insert
    @LichHenId UNIQUEIDENTIFIER,
    @NhanVienId UNIQUEIDENTIFIER,
    @DoanhNghiepId UNIQUEIDENTIFIER,
    @TrangThaiCode VARCHAR(20),
    @TongTien DECIMAL(18,2),
    @PhuongThucTT VARCHAR(20),
    @GhiChu NVARCHAR(255)
AS
INSERT INTO DonThuGom (LichHenId, NhanVienId, DoanhNghiepId, TrangThaiCode, TongTien, PhuongThucTT, GhiChu)
VALUES (@LichHenId, @NhanVienId, @DoanhNghiepId, @TrangThaiCode, @TongTien, @PhuongThucTT, @GhiChu);
GO

CREATE OR ALTER PROCEDURE sp_DonThuGom_Update
    @Id UNIQUEIDENTIFIER,
    @TrangThaiCode VARCHAR(20),
    @TongTien DECIMAL(18,2),
    @PhuongThucTT VARCHAR(20),
    @GhiChu NVARCHAR(255)
AS
UPDATE DonThuGom
SET TrangThaiCode=@TrangThaiCode, TongTien=@TongTien, PhuongThucTT=@PhuongThucTT,
    GhiChu=@GhiChu, UpdatedAt=SYSDATETIME()
WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_DonThuGom_Delete
    @Id UNIQUEIDENTIFIER
AS
DELETE FROM DonThuGom WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_DonThuGom_GetAll
AS
SELECT d.*, n.HoTen AS TenNhanVien, dn.TenDoanhNghiep
FROM DonThuGom d
LEFT JOIN NhanVien n ON d.NhanVienId = n.Id
LEFT JOIN DoanhNghiep dn ON d.DoanhNghiepId = dn.Id
ORDER BY d.CreatedAt DESC;
GO

------------------------------------------------------------
-- 🔍 LẤY CHI TIẾT 1 ĐƠN
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DonThuGom_GetById
    @Id UNIQUEIDENTIFIER
AS
SELECT d.*, 
       n.HoTen AS TenNhanVien, 
       dn.TenDoanhNghiep,
       lh.DiaChi,
       lh.ThoiGianHen
FROM DonThuGom d
LEFT JOIN NhanVien n ON d.NhanVienId = n.Id
LEFT JOIN DoanhNghiep dn ON d.DoanhNghiepId = dn.Id
LEFT JOIN LichHen lh ON d.LichHenId = lh.Id
WHERE d.Id = @Id;
GO

------------------------------------------------------------
-- ✏️ CẬP NHẬT NHANH TRẠNG THÁI
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DonThuGom_UpdateTrangThai
    @Id UNIQUEIDENTIFIER,
    @TrangThaiCode VARCHAR(20),
    @GhiChu NVARCHAR(255)
AS
UPDATE DonThuGom
SET TrangThaiCode=@TrangThaiCode, 
    GhiChu=@GhiChu,
    UpdatedAt = SYSDATETIME()
WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_TrangThaiDon_GetAll
AS
BEGIN
    SELECT Code, Ten FROM TrangThaiDon ORDER BY Ten ASC;
END;
GO


-- Giả sử đã có dữ liệu LichHen, NhanVien, DoanhNghiep
DECLARE @NV1 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM NhanVien);
DECLARE @DN1 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM DoanhNghiep);
DECLARE @LH1 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM LichHen);

INSERT INTO DonThuGom (LichHenId, NhanVienId, DoanhNghiepId, TrangThaiCode, TongTien, PhuongThucTT, GhiChu)
VALUES
(@LH1, @NV1, @DN1, 'TAO_MOI', 0, 'TIEN_MAT', N'Chờ báo giá'),
(@LH1, @NV1, @DN1, 'DA_BAO_GIA', 1500000, 'CHUYEN_KHOAN', N'Báo giá xác nhận'),
(@LH1, @NV1, @DN1, 'DA_THOA_THUAN', 2000000, 'CHUYEN_KHOAN', N'Khách đồng ý'),
(@LH1, @NV1, @DN1, 'DA_THANH_TOAN', 2200000, 'CHUYEN_KHOAN', N'Đã thanh toán'),
(@LH1, @NV1, @DN1, 'DA_HUY', 0, 'TIEN_MAT', N'Khách hủy'),
(@LH1, @NV1, @DN1, 'TAO_MOI', 0, 'TIEN_MAT', N'Chờ xác nhận'),
(@LH1, @NV1, @DN1, 'DA_BAO_GIA', 1000000, 'TIEN_MAT', N'Đã báo giá lại'),
(@LH1, @NV1, @DN1, 'DA_THOA_THUAN', 1300000, 'CHUYEN_KHOAN', N'Đã chốt đơn'),
(@LH1, @NV1, @DN1, 'DA_THANH_TOAN', 1400000, 'CHUYEN_KHOAN', N'Thanh toán đủ'),
(@LH1, @NV1, @DN1, 'DA_HUY', 0, 'TIEN_MAT', N'Lịch hủy');
GO

/////////////////////////////////////////////////////
-------- Loại 5 -------------------------------------
/////////////////////////////////////////////////////

CREATE TABLE ViTriNguoiDung (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaiKhoanId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES TaiKhoan(Id),
    KinhDo FLOAT,
    ViDo FLOAT,
    ThoiGianCapNhat DATETIME2 DEFAULT SYSDATETIME()
);
GO

CREATE TABLE LichSuViTriNhanVien (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    NhanVienId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES NhanVien(Id),
    KinhDo FLOAT,
    ViDo FLOAT,
    ThoiGian DATETIME2 DEFAULT SYSDATETIME()
);
GO

------------------------------------------------------------
-- 🧩 PROCEDURE CRUD
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ViTriNguoiDung_Insert
    @TaiKhoanId UNIQUEIDENTIFIER,
    @KinhDo FLOAT,
    @ViDo FLOAT
AS
INSERT INTO ViTriNguoiDung (TaiKhoanId, KinhDo, ViDo)
VALUES (@TaiKhoanId, @KinhDo, @ViDo);
GO

CREATE OR ALTER PROCEDURE sp_ViTriNguoiDung_GetById
    @TaiKhoanId UNIQUEIDENTIFIER
AS
SELECT v.*, 
       CONVERT(NVARCHAR(255), DecryptByKeyAutoCert(Cert_ID('Cert_PheLieu'), NULL, t.Email)) AS Email
FROM ViTriNguoiDung v
JOIN TaiKhoan t ON t.Id = v.TaiKhoanId
WHERE v.TaiKhoanId = @TaiKhoanId;
GO

CREATE OR ALTER PROCEDURE sp_ViTriNguoiDung_Delete
    @TaiKhoanId UNIQUEIDENTIFIER
AS
DELETE FROM ViTriNguoiDung WHERE TaiKhoanId=@TaiKhoanId;
GO

CREATE OR ALTER PROCEDURE sp_ViTriNguoiDung_Update
    @TaiKhoanId UNIQUEIDENTIFIER,
    @KinhDo FLOAT,
    @ViDo FLOAT
AS
UPDATE ViTriNguoiDung
SET KinhDo=@KinhDo, ViDo=@ViDo, ThoiGianCapNhat=SYSDATETIME()
WHERE TaiKhoanId=@TaiKhoanId;
GO

CREATE OR ALTER PROCEDURE sp_ViTriNguoiDung_GetAll
AS
SELECT v.*, CONVERT(NVARCHAR(255), DecryptByKeyAutoCert(Cert_ID('Cert_PheLieu'), NULL, t.Email)) AS Email
FROM ViTriNguoiDung v
JOIN TaiKhoan t ON t.Id = v.TaiKhoanId;
GO

------------------------------------------------------------
-- 📍 CRUD CHO LICH SU VI TRI NHAN VIEN
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_LichSuViTriNhanVien_Insert
    @NhanVienId UNIQUEIDENTIFIER,
    @KinhDo FLOAT,
    @ViDo FLOAT
AS
INSERT INTO LichSuViTriNhanVien (NhanVienId, KinhDo, ViDo)
VALUES (@NhanVienId, @KinhDo, @ViDo);
GO

CREATE OR ALTER PROCEDURE sp_LichSuViTriNhanVien_Update
    @Id UNIQUEIDENTIFIER,
    @KinhDo FLOAT,
    @ViDo FLOAT
AS
UPDATE LichSuViTriNhanVien
SET KinhDo=@KinhDo, ViDo=@ViDo, ThoiGian=SYSDATETIME()
WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_LichSuViTriNhanVien_Delete
    @Id UNIQUEIDENTIFIER
AS
DELETE FROM LichSuViTriNhanVien WHERE Id=@Id;
GO

CREATE OR ALTER PROCEDURE sp_LichSuViTriNhanVien_GetAll
AS
SELECT l.*, n.HoTen AS TenNhanVien
FROM LichSuViTriNhanVien l
JOIN NhanVien n ON n.Id = l.NhanVienId
ORDER BY l.ThoiGian DESC;
GO

CREATE OR ALTER PROCEDURE sp_LichSuViTriNhanVien_GetByNhanVien
    @NhanVienId UNIQUEIDENTIFIER
AS
SELECT l.*, n.HoTen AS TenNhanVien
FROM LichSuViTriNhanVien l
JOIN NhanVien n ON n.Id = l.NhanVienId
WHERE l.NhanVienId = @NhanVienId
ORDER BY l.ThoiGian DESC;
GO


DECLARE @TK1 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TaiKhoan);
DECLARE @TK2 UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM TaiKhoan ORDER BY CreatedAt DESC);

INSERT INTO ViTriNguoiDung (TaiKhoanId, KinhDo, ViDo)
VALUES
(@TK1, 106.700, 10.776),
(@TK1, 106.702, 10.779),
(@TK2, 106.709, 10.772),
(@TK2, 106.711, 10.770),
(@TK1, 106.703, 10.773),
(@TK1, 106.705, 10.776),
(@TK2, 106.710, 10.774),
(@TK2, 106.712, 10.775),
(@TK1, 106.704, 10.777),
(@TK1, 106.706, 10.778);
GO


/////////////////////////////////////////////////////
-------- Loại 6-------------------------------------
/////////////////////////////////////////////////////

------------------------------------------------------------
-- 🔧 BẢNG ĐƠN BÁN PHẾ LIỆU (CÓ CẢ KHÁCH HÀNG VÀ DOANH NGHIỆP)
------------------------------------------------------------

CREATE TABLE DonBanPheLieu (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    KhachHangId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES KhachHang(Id),
    DoanhNghiepId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES DoanhNghiep(Id),
    TenPheLieu NVARCHAR(150),
    KhoiLuong DECIMAL(10,2),
    DonGia DECIMAL(18,2),
    MoTa NVARCHAR(255),
    TrangThai NVARCHAR(50) DEFAULT N'Chờ giao dịch',
    CreatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO


------------------------------------------------------------
-- 🔧 BẢNG ĐƠN MUA PHẾ LIỆU (CÓ CẢ KHÁCH HÀNG & DOANH NGHIỆP)
------------------------------------------------------------
CREATE TABLE DonMuaPheLieu (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    KhachHangId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES KhachHang(Id),
    DoanhNghiepId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES DoanhNghiep(Id),
    TenPheLieu NVARCHAR(150),
    KhoiLuong DECIMAL(10,2),
    DonGiaDeXuat DECIMAL(18,2),
    MoTa NVARCHAR(255),
    TrangThai NVARCHAR(50) DEFAULT N'Đang tìm nguồn cung',
    CreatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO


------------------------------------------------------------
-- 🧩 THỦ TỤC: ĐĂNG BÁN PHẾ LIỆU
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DonBanPheLieu_Insert
    @KhachHangId UNIQUEIDENTIFIER = NULL,
    @DoanhNghiepId UNIQUEIDENTIFIER = NULL,
    @TenPheLieu NVARCHAR(150),
    @KhoiLuong DECIMAL(10,2),
    @DonGia DECIMAL(18,2),
    @MoTa NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF @KhachHangId IS NULL AND @DoanhNghiepId IS NULL
    BEGIN
        RAISERROR(N'Phải có ít nhất Khách hàng hoặc Doanh nghiệp!', 16, 1);
        RETURN;
    END;

    DECLARE @NewId UNIQUEIDENTIFIER;

    INSERT INTO DonBanPheLieu (KhachHangId, DoanhNghiepId, TenPheLieu, KhoiLuong, DonGia, MoTa)
    VALUES (@KhachHangId, @DoanhNghiepId, @TenPheLieu, @KhoiLuong, @DonGia, @MoTa);

    SELECT @NewId = Id 
    FROM DonBanPheLieu 
    WHERE TenPheLieu = @TenPheLieu 
    ORDER BY CreatedAt DESC;

    EXEC sp_ThongBao_TaoChoDonBan @NewId;

    SELECT @NewId AS Id;  -- 🔥 BẮT BUỘC PHẢI CÓ
END;
GO



------------------------------------------------------------
-- 🧩 CẬP NHẬT / XÓA / LẤY DỮ LIỆU
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DonBanPheLieu_Update
    @Id UNIQUEIDENTIFIER,
    @KhoiLuong DECIMAL(10,2),
    @DonGia DECIMAL(18,2),
    @MoTa NVARCHAR(255),
    @TrangThai NVARCHAR(50)
AS
UPDATE DonBanPheLieu
SET KhoiLuong=@KhoiLuong, DonGia=@DonGia, MoTa=@MoTa, TrangThai=@TrangThai
WHERE Id=@Id;
GO


CREATE OR ALTER PROCEDURE sp_DonBanPheLieu_Delete
    @Id UNIQUEIDENTIFIER
AS
DELETE FROM DonBanPheLieu WHERE Id=@Id;
GO


CREATE OR ALTER PROCEDURE sp_DonBanPheLieu_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        db.Id,
        db.KhachHangId,
        db.DoanhNghiepId,
        db.TenPheLieu,
        db.KhoiLuong,
        db.DonGia,
        db.MoTa,
        db.TrangThai,
        db.CreatedAt,
        ISNULL(kh.HoTen, dn.TenDoanhNghiep) AS NguoiDang,
        CASE 
            WHEN db.KhachHangId IS NOT NULL THEN N'Khách hàng'
            WHEN db.DoanhNghiepId IS NOT NULL THEN N'Doanh nghiệp'
            ELSE N'Không rõ'
        END AS LoaiNguoiDang
    FROM DonBanPheLieu db
    LEFT JOIN KhachHang kh ON db.KhachHangId = kh.Id
    LEFT JOIN DoanhNghiep dn ON db.DoanhNghiepId = dn.Id
    ORDER BY db.CreatedAt DESC;
END;
GO



------------------------------------------------------------
-- 🧩 THỦ TỤC: ĐƠN MUA PHẾ LIỆU (TƯƠNG TỰ)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DonMuaPheLieu_Insert
    @KhachHangId UNIQUEIDENTIFIER = NULL,
    @DoanhNghiepId UNIQUEIDENTIFIER = NULL,
    @TenPheLieu NVARCHAR(150),
    @KhoiLuong DECIMAL(10,2),
    @DonGiaDeXuat DECIMAL(18,2),
    @MoTa NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF @KhachHangId IS NULL AND @DoanhNghiepId IS NULL
    BEGIN
        RAISERROR(N'Phải có ít nhất Khách hàng hoặc Doanh nghiệp!', 16, 1);
        RETURN;
    END;

    DECLARE @NewId UNIQUEIDENTIFIER;

    INSERT INTO DonMuaPheLieu (KhachHangId, DoanhNghiepId, TenPheLieu, KhoiLuong, DonGiaDeXuat, MoTa)
    VALUES (@KhachHangId, @DoanhNghiepId, @TenPheLieu, @KhoiLuong, @DonGiaDeXuat, @MoTa);

    SELECT @NewId = Id 
    FROM DonMuaPheLieu 
    WHERE TenPheLieu = @TenPheLieu 
    ORDER BY CreatedAt DESC;

    EXEC sp_ThongBao_TaoChoDonMua @NewId;

    SELECT @NewId AS Id;  -- 🔥 BẮT BUỘC PHẢI CÓ
END;
GO



CREATE OR ALTER PROCEDURE sp_DonMuaPheLieu_Update
    @Id UNIQUEIDENTIFIER,
    @TrangThai NVARCHAR(50)
AS
UPDATE DonMuaPheLieu SET TrangThai=@TrangThai WHERE Id=@Id;
GO


CREATE OR ALTER PROCEDURE sp_DonMuaPheLieu_Delete
    @Id UNIQUEIDENTIFIER
AS
DELETE FROM DonMuaPheLieu WHERE Id=@Id;
GO


CREATE OR ALTER PROCEDURE sp_DonMuaPheLieu_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        dm.Id,
        dm.KhachHangId,
        dm.DoanhNghiepId,
        dm.TenPheLieu,
        dm.KhoiLuong,
        dm.DonGiaDeXuat,
        dm.MoTa,
        dm.TrangThai,
        dm.CreatedAt,
        ISNULL(kh.HoTen, dn.TenDoanhNghiep) AS NguoiDang,
        CASE 
            WHEN dm.KhachHangId IS NOT NULL THEN N'Khách hàng'
            WHEN dm.DoanhNghiepId IS NOT NULL THEN N'Doanh nghiệp'
            ELSE N'Không rõ'
        END AS LoaiNguoiDang
    FROM DonMuaPheLieu dm
    LEFT JOIN KhachHang kh ON dm.KhachHangId = kh.Id
    LEFT JOIN DoanhNghiep dn ON dm.DoanhNghiepId = dn.Id
    ORDER BY dm.CreatedAt DESC;
END;
GO


------------------------------------------------------------
-- 🔍 LẤY CHI TIẾT TỪNG ĐƠN
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_DonBanPheLieu_GetById
    @Id UNIQUEIDENTIFIER
AS
SELECT 
    db.*,
    ISNULL(kh.HoTen, dn.TenDoanhNghiep) AS NguoiDang,
    CASE 
        WHEN db.KhachHangId IS NOT NULL THEN N'Khách hàng'
        WHEN db.DoanhNghiepId IS NOT NULL THEN N'Doanh nghiệp'
    END AS LoaiNguoiDang
FROM DonBanPheLieu db
LEFT JOIN KhachHang kh ON db.KhachHangId = kh.Id
LEFT JOIN DoanhNghiep dn ON db.DoanhNghiepId = dn.Id
WHERE db.Id = @Id;
GO


CREATE OR ALTER PROCEDURE sp_DonMuaPheLieu_GetById
    @Id UNIQUEIDENTIFIER
AS
SELECT 
    dm.*,
    ISNULL(kh.HoTen, dn.TenDoanhNghiep) AS NguoiDang,
    CASE 
        WHEN dm.KhachHangId IS NOT NULL THEN N'Khách hàng'
        WHEN dm.DoanhNghiepId IS NOT NULL THEN N'Doanh nghiệp'
    END AS LoaiNguoiDang
FROM DonMuaPheLieu dm
LEFT JOIN KhachHang kh ON dm.KhachHangId = kh.Id
LEFT JOIN DoanhNghiep dn ON dm.DoanhNghiepId = dn.Id
WHERE dm.Id = @Id;
GO


------------------------------------------------------------
-- 🌱 DỮ LIỆU 
------------------------------------------------------------
DECLARE @DN UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM DoanhNghiep);
DECLARE @KH UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM KhachHang);

-- Đơn bán từ Doanh nghiệp
INSERT INTO DonBanPheLieu (DoanhNghiepId, TenPheLieu, KhoiLuong, DonGia, MoTa)
VALUES
(@DN, N'Đồng cáp điện', 20, 120000, N'Đồng loại 1'),
(@DN, N'Inox chảo', 30, 27000, N'Đồ inox nhà bếp');

-- Đơn bán từ Khách hàng
INSERT INTO DonBanPheLieu (KhachHangId, TenPheLieu, KhoiLuong, DonGia, MoTa)
VALUES
(@KH, N'Nhựa PET', 50, 8000, N'Chai nước suối cũ'),
(@KH, N'Giấy carton', 100, 4000, N'Carton kho hàng');

-- Đơn mua phế liệu từ Doanh nghiệp
INSERT INTO DonMuaPheLieu (DoanhNghiepId, TenPheLieu, KhoiLuong, DonGiaDeXuat, MoTa)
VALUES
(@DN, N'Sắt thép', 500, 9500, N'Thu mua sắt tại kho'),
(@DN, N'Nhôm lon', 200, 32000, N'Thu mua lon bia');

-- Đơn mua phế liệu từ Khách hàng
INSERT INTO DonMuaPheLieu (KhachHangId, TenPheLieu, KhoiLuong, DonGiaDeXuat, MoTa)
VALUES
(@KH, N'Đồng vụn', 10, 85000, N'Tìm mua dây điện đồng nhỏ'),
(@KH, N'Ve chai bia', 300, 1500, N'Tìm mua chai thủy tinh');
GO

-------------------------------------------

CREATE TABLE ThongBao (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DoanhNghiepId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES DoanhNghiep(Id),
    Loai NVARCHAR(50), -- DON_BAN / DON_MUA / HE_THONG
    DonBanId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES DonBanPheLieu(Id),
    DonMuaId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES DonMuaPheLieu(Id),
    NoiDung NVARCHAR(255),
    DaXem BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

CREATE OR ALTER PROCEDURE sp_ThongBao_TaoChoDonBan
    @DonBanId UNIQUEIDENTIFIER
AS
BEGIN
    DECLARE @TenPheLieu NVARCHAR(150), 
            @KhoiLuong DECIMAL(10,2), 
            @NguoiDang NVARCHAR(100);

    SELECT 
        @TenPheLieu = TenPheLieu, 
        @KhoiLuong = KhoiLuong,
        @NguoiDang = ISNULL(kh.HoTen, dn.TenDoanhNghiep)
    FROM DonBanPheLieu db
    LEFT JOIN KhachHang kh ON db.KhachHangId = kh.Id
    LEFT JOIN DoanhNghiep dn ON db.DoanhNghiepId = dn.Id
    WHERE db.Id = @DonBanId;

    INSERT INTO ThongBao (DoanhNghiepId, Loai, DonBanId, NoiDung)
    SELECT 
        d.Id,
        'DON_BAN',
        @DonBanId,
        CONCAT(N'📦 ', @NguoiDang, N' đăng bán ', @TenPheLieu, 
               N' (', @KhoiLuong, ' kg). Kiểm tra để giao dịch.')
    FROM DoanhNghiep d
    WHERE d.TrangThaiHoatDong = 1;
END;
GO
CREATE OR ALTER PROCEDURE sp_ThongBao_TaoChoDonMua
    @DonMuaId UNIQUEIDENTIFIER
AS
BEGIN
    DECLARE @TenPheLieu NVARCHAR(150), 
            @KhoiLuong DECIMAL(10,2),
            @NguoiDang NVARCHAR(100);

    SELECT 
        @TenPheLieu = TenPheLieu, 
        @KhoiLuong = KhoiLuong,
        @NguoiDang = ISNULL(kh.HoTen, dn.TenDoanhNghiep)
    FROM DonMuaPheLieu dm
    LEFT JOIN KhachHang kh ON dm.KhachHangId = kh.Id
    LEFT JOIN DoanhNghiep dn ON dm.DoanhNghiepId = dn.Id
    WHERE dm.Id = @DonMuaId;

    INSERT INTO ThongBao (DoanhNghiepId, Loai, DonMuaId, NoiDung)
    SELECT 
        d.Id,
        'DON_MUA',
        @DonMuaId,
        CONCAT(N'🛒 ', @NguoiDang, N' cần mua ', @TenPheLieu, 
               N' (', @KhoiLuong, ' kg). Bạn có nguồn cung không?')
    FROM DoanhNghiep d
    WHERE d.TrangThaiHoatDong = 1;
END;
GO

CREATE OR ALTER PROCEDURE sp_ThongBao_GetByDoanhNghiep
    @DoanhNghiepId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT 
        tb.Id,
        tb.NoiDung,
        tb.Loai,
        tb.DaXem,
        tb.CreatedAt,
        db.TenPheLieu AS TenBan,
        dm.TenPheLieu AS TenMua
    FROM ThongBao tb
    LEFT JOIN DonBanPheLieu db ON tb.DonBanId = db.Id
    LEFT JOIN DonMuaPheLieu dm ON tb.DonMuaId = dm.Id
    WHERE tb.DoanhNghiepId = @DoanhNghiepId
    ORDER BY tb.CreatedAt DESC;
END;
GO
CREATE OR ALTER PROCEDURE sp_ThongBao_DanhDauDaXem
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE ThongBao SET DaXem = 1 WHERE Id = @Id;
END;
GO




---------7 phan cong--------------
---------------------------------------------------
CREATE TABLE LichPhanCong (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DoanhNghiepId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES DoanhNghiep(Id),
    NhanVienId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES NhanVien(Id),
    CongViec NVARCHAR(255),
    DiaDiem NVARCHAR(255),
    KinhDo FLOAT,
    ViDo FLOAT,
    ThoiGianBatDau DATETIME2,
    ThoiGianKetThuc DATETIME2,
    TrangThai NVARCHAR(50) DEFAULT N'CHO_XAC_NHAN',  -- CHO_XAC_NHAN, DANG_THUC_HIEN, TU_CHOI, HUY, HET_HAN, HOAN_THANH
    HanPhanHoi DATETIME2,
    CreatedAt DATETIME2 DEFAULT SYSDATETIME()
);
GO

------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_LichPhanCong_InsertAuto
    @DoanhNghiepId UNIQUEIDENTIFIER,
    @CongViec NVARCHAR(255),
    @DiaDiem NVARCHAR(255),
    @KinhDo FLOAT,
    @ViDo FLOAT,
    @ThoiGianBatDau DATETIME2,
    @ThoiGianKetThuc DATETIME2
AS
BEGIN
    DECLARE @NhanVienGanNhat UNIQUEIDENTIFIER;

    SELECT TOP 1 @NhanVienGanNhat = nv.Id
    FROM NhanVien nv
    JOIN ViTriNguoiDung vt ON vt.TaiKhoanId = nv.TaiKhoanId
    WHERE nv.DoanhNghiepId = @DoanhNghiepId
      AND nv.TrangThaiSanSang = 1
      AND nv.TrangThaiHoatDong = 1
    ORDER BY SQRT(POWER(vt.KinhDo - @KinhDo, 2) + POWER(vt.ViDo - @ViDo, 2));

    INSERT INTO LichPhanCong 
    (DoanhNghiepId, NhanVienId, CongViec, DiaDiem, KinhDo, ViDo, 
     ThoiGianBatDau, ThoiGianKetThuc, HanPhanHoi)
    VALUES
    (@DoanhNghiepId, @NhanVienGanNhat, @CongViec, @DiaDiem, 
     @KinhDo, @ViDo, @ThoiGianBatDau, @ThoiGianKetThuc, DATEADD(MINUTE,5,GETDATE()));
END;
GO


----------------------------------------
CREATE OR ALTER PROCEDURE sp_LichPhanCong_Nhan
    @Id UNIQUEIDENTIFIER
AS
UPDATE LichPhanCong
SET TrangThai = 'DANG_THUC_HIEN'
WHERE Id = @Id AND TrangThai = 'CHO_XAC_NHAN';
GO
CREATE OR ALTER PROCEDURE sp_LichPhanCong_TuChoi
    @Id UNIQUEIDENTIFIER
AS
UPDATE LichPhanCong 
SET TrangThai = 'TU_CHOI'
WHERE Id = @Id;
GO
CREATE OR ALTER PROCEDURE sp_LichPhanCong_CheckExpired
AS
UPDATE LichPhanCong
SET TrangThai = 'HET_HAN'
WHERE TrangThai = 'CHO_XAC_NHAN'
AND HanPhanHoi < GETDATE();
GO
