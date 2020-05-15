use v3x
update dbo.Job
set BasePay=800
where PeopleId = 2

go
select * from dbo.Job

