select *
from AspNetUserRoles

select *
from AspNetUsers

select *
from AspNetRoles


delete AspNetUserRoles
where UserId = '14dd8aa2-e191-49b4-a336-e6ff0fab94fd' and RoleId = 'b42e7386-f2b1-43b7-a8af-af6bbf34173b'  
insert into AspNetUserRoles (UserId,RoleId)
values 
('11b4c125-8221-453b-aa3e-a4208b20aeb0','c8c3119d-de32-4e81-884f-fe1045fda393')

delete AspNetUserRoles
where RoleId = 'b42e7386-f2b1-43b7-a8af-af6bbf34173b'

delete AspNetUsers
where Id = '1669dc38-d5c4-4d0b-a2b9-173bbb990e3a'