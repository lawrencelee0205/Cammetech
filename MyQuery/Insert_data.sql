use v3x
BULK INSERT dbo.People
    FROM 'C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Sample_Data\People.csv' --Input CSV file path
    WITH
    (
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',  --CSV field delimiter
    ROWTERMINATOR = '\n',   --Use to shift the control to next row
    ERRORFILE = 'C:\Users\lawre\Desktop\Assignment\AppDev\Cammetech\Sample_Data\PeopleErrorRows.csv', -- Error log
    TABLOCK
    )
go

