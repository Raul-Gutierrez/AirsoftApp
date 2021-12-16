select *
from AspNetUserRoles

select *
from AspNetUsers

select *
from AspNetRoles

insert into AspNetUserRoles (UserId,RoleId)
values 
('1669dc38-d5c4-4d0b-a2b9-173bbb990e3a','b42e7386-f2b1-43b7-a8af-af6bbf34173b')

delete AspNetUserRoles
where RoleId = 'b42e7386-f2b1-43b7-a8af-af6bbf34173b'

delete AspNetUsers
where Id = '1669dc38-d5c4-4d0b-a2b9-173bbb990e3a'