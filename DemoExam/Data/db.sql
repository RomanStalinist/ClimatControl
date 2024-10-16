-- we don't know how to generate root <with-no-name> (class Root) :(

if db_id('ClimateControl') is null
    create database ClimateControl collate Cyrillic_General_CI_AS
go

use ClimateControl
go

grant connect on database :: ClimateControl to dbo
go

grant view any column encryption key definition, view any column master key definition on database :: ClimateControl to [public]
go

create table dbo.Customers
(
    Id       int identity
        constraint PkCustomersId
            primary key,
    Name     nvarchar(100) not null,
    Phone    nvarchar(15)  not null
        constraint UqCustomersPhone
            unique,
    Password nvarchar(255) not null
)
go

create table dbo.Requests
(
    Id                 int identity
        constraint PkRequestsId
            primary key,
    CustomerId         int                         not null
        constraint FkRequestsCustomers
            references dbo.Customers
            on delete cascade,
    CreatedAt          datetime2 default getdate() not null,
    EquipmentType      nvarchar(100),
    DeviceModel        nvarchar(100),
    ProblemDescription nvarchar(max)
)
go

create table dbo.Specialists
(
    Id          int identity
        constraint PkSpecialistsId
            primary key,
    Name        nvarchar(100) not null,
    ContactInfo nvarchar(255),
    Phone       nvarchar(15)  not null
        constraint UqSpecialistsPhone
            unique,
    Password    nvarchar(255) not null
)
go

create table dbo.Comments
(
    Id           int identity,
    RequestId    int not null
        constraint FkCommentsRequests
            references dbo.Requests
            on delete cascade,
    SpecialistId int not null
        constraint FkCommentsSpecialists
            references dbo.Specialists
            on delete cascade,
    Content      nvarchar(max),
    CreatedAt    datetime2 default getdate()
)
go

create table dbo.RequestSpecialists
(
    RequestId    int not null
        constraint FkRequestSpecialistsRequests
            references dbo.Requests
            on delete cascade,
    SpecialistId int not null
        constraint FkRequestSpecialistsSpecialists
            references dbo.Specialists
            on delete cascade,
    UpdatedAt    datetime2 default getdate(),
    constraint PkRequestSpecialistsRequestIdSpecialistId
        primary key (RequestId, SpecialistId)
)
go

create table dbo.Statuses
(
    Name nvarchar(100) not null,
    Id   int identity
        constraint PkStatusesId
            primary key
)
go

create table dbo.RequestStatuses
(
    Id        int identity
        constraint PkRequestStatuses
            primary key,
    RequestId int not null
        constraint FkRequestStatusesRequests
            references dbo.Requests
            on delete cascade,
    StatusId  int not null
        constraint FkRequestStatusesStatuses
            references dbo.Statuses
            on delete cascade,
    UpdatedAt datetime2 default getdate(),
    CreatedAt datetime2 default getdate(),
    constraint UqRequestStatusesRequestIdStatusId
        unique (RequestId, StatusId)
)
go

deny delete, insert, update on dbo.Statuses to [public]
go

create proc ADDREQUESTATUS(@RequestId int, @StatusId int)
as
    begin
        begin try
            begin tran
            if @RequestId is null
                raiserror('RequestId is null', 16, 1)
            if not exists(select 1 from Requests where Id = @RequestId)
                raiserror('Request with Id %s not found', 16, 1, @RequestId)
            if @StatusId is null
                raiserror('StatusId is null', 16, 1)
            if not exists(select 1 from Statuses where Id = @StatusId)
                raiserror('Status with Id %s not found', 16, 1, @StatusId)

            insert into RequestStatuses(RequestId, StatusId)
            values (@RequestId, @StatusId)

            commit
        end try

        begin catch
            rollback
            print(ERROR_MESSAGE())
        end catch
    end
go

create procedure dbo.ADDREQUESTSPECIALIST(@RequestId int, @SpecialistId int) as
-- missing source code
go

create function GETAVGREQUESTCOMPLETIONSPEED()
returns time
as
begin
    declare @AvgCompletionSpeed time;

    select @AvgCompletionSpeed =
        cast(dateadd(minute, avg(datediff(minute, InProgress.UpdatedAt, Completed.UpdatedAt)), 0) as time)
    from
        RequestStatuses InProgress
    join
        RequestStatuses Completed
    on
        InProgress.RequestId = Completed.RequestId
    where
        InProgress.StatusId = 1 and  -- Статус "в процессе"
        Completed.StatusId = 2;      -- Статус "завершена"

    return @AvgCompletionSpeed;
end
go

create function GETCOMPLETEDREQUESTSCOUNT()
returns int
as
    begin
        declare @Count int
        select @Count = count(*) from RequestStatuses where StatusId = 2  -- Статус "завершена"
        return @Count
    end
go

create function GETREQUESTBYID(@Id int)
returns @Request table
    (
        Id int,
        CustomerId int,
        CreatedAt date,
        EquipmentType nvarchar(100),
        DeviceModel nvarchar(100),
        ProblemDescription nvarchar(max)
    )
as
    begin
        insert into @Request
        select * from Requests where Id = @Id
        return
    end
go

create function GETREQUESTS()
    returns @Requests table
    (
        Id int,
        CustomerId int,
        CreatedAt date,
        EquipmentType nvarchar(100),
        DeviceModel nvarchar(100),
        ProblemDescription nvarchar(max)
    )
as
    begin
        insert into @Requests
        select * from Requests
        return
    end
go
