CREATE SYMMETRIC KEY Posts_Key
    WITH ALGORITHM = AES_256
    ENCRYPTION BY CERTIFICATE PostConntentCertificate;  
GO  